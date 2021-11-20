using FiveOhFirstDataCore.Data.Account;
using FiveOhFirstDataCore.Data.Structures;
using FiveOhFirstDataCore.Data.Structures.Promotions;
using FiveOhFirstDataCore.Data.Structuresbase;
using FiveOhFirstDataCore.Data.Extensions;
using FiveOhFirstDataCore.Data.Structures;
using FiveOhFirstDataCore.Data.Structures.Updates;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using System.Security.Claims;

namespace FiveOhFirstDataCore.Data.Services
{
    public class PromotionService : IPromotionService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
        private readonly UserManager<Trooper> _userManager;
        private readonly IWebsiteSettingsService _webSettings;
        private readonly IDiscordService _discord;

        public PromotionService(IDbContextFactory<ApplicationDbContext> dbContextFactory, UserManager<Trooper> userManager,
            IWebsiteSettingsService webSettings, IDiscordService discord)
        {
            _dbContextFactory = dbContextFactory;
            _userManager = userManager;
            _webSettings = webSettings;
            _discord = discord;
        }

        public async Task<ResultBase> CancelPromotionAsync(Promotion promotion, Trooper denier)
        {
            using var _dbContext = _dbContextFactory.CreateDbContext();
            var actual = await _dbContext.Promotions.FindAsync(promotion.Id);
            var user = await _userManager.FindByIdAsync(denier.Id.ToString());

            if (actual is null)
            {
                return new(false, new() { "Promotion was not found in the database." });
            }

            var log = new RankUpdate()
            {
                Approved = false,
                RequestedById = actual.RequestedById,
                ChangedFrom = actual.PromoteFrom,
                ChangedTo = actual.PromoteTo,
                ChangedForId = actual.PromotionForId,
                ChangedOn = DateTime.UtcNow.ToEst(),
                SubmittedByRosterClerk = false
            };

            user.DeniedRankUpdates.Add(log);

            _dbContext.Remove(actual);
            await _dbContext.SaveChangesAsync();
            var identRes = await _userManager.UpdateAsync(user);
            if (!identRes.Succeeded)
            {
                List<string> errors = new();
                foreach (var error in identRes.Errors)
                    errors.Add($"[{error.Code}] {error.Description}");

                return new(false, errors);
            }

            return new(true, null);
        }

        public async Task<ResultBase> ElevatePromotionAsync(Promotion promotion, Trooper approver, int levels = 1)
        {
            using var _dbContext = _dbContextFactory.CreateDbContext();
            var actual = await _dbContext.Promotions.FindAsync(promotion.Id);

            if (actual is null)
            {
                return new(false, new() { "Promotion was not found in the database." });
            }

            var user = await _dbContext.FindAsync<Trooper>(approver.Id);
            await _dbContext.Entry(actual).Collection(e => e.ApprovedBy).LoadAsync();

            if (actual.CurrentBoard > PromotionBoardLevel.Battalion
                    && actual.NeededBoard == PromotionBoardLevel.Battalion)
            {
                actual.CurrentBoard = PromotionBoardLevel.Battalion;
            }
            else
            {
                actual.CurrentBoard++;
            }

            if (actual.CurrentBoard > actual.NeededBoard)
            {
                await _dbContext.SaveChangesAsync();
                return await FinalizePromotionAsync(promotion, approver);
            }

            if (actual.ApprovedBy is null) actual.ApprovedBy = new();

            if (user is not null && !actual.ApprovedBy.Any(x => x.Id == user.Id))
                user.ApprovedPendingPromotions.Add(actual);

            await _dbContext.SaveChangesAsync();

            return new(true, null);
        }

        public async Task<ResultBase> FinalizePromotionAsync(Promotion promotion, Trooper approver)
        {
            using var _dbContext = _dbContextFactory.CreateDbContext();
            var actual = await _dbContext.Promotions.FindAsync(promotion.Id);

            if (actual is null)
            {
                return new(false, new() { "Promotion was not found in the database." });
            }

            await _dbContext.Entry(actual).Collection(e => e.ApprovedBy).LoadAsync();
            await _dbContext.Entry(actual).Reference(e => e.PromotionFor).LoadAsync();

            var user = await _dbContext.FindAsync<Trooper>(approver.Id);

            if (user is null)
            {
                return new(false, new() { "Failed to find an account for the approver" });
            }

            var now = DateTime.UtcNow.ToEst();
            var log = new RankUpdate()
            {
                Approved = true,
                RequestedById = actual.RequestedById,
                ChangedFrom = actual.PromoteFrom,
                ChangedTo = actual.PromoteTo,
                ChangedForId = actual.PromotionForId,
                ChangedOn = now,
                SubmittedByRosterClerk = false,
                ApprovedBy = actual.ApprovedBy
            };

            user.ApprovedRankUpdates.Add(log);

            var rank = actual.PromoteTo.GetRank();
            switch (rank)
            {
                case TrooperRank r:
                    actual.PromotionFor.Rank = r;
                    actual.PromotionFor.LastPromotion = now;
                    break;
                case MedicRank r:
                    actual.PromotionFor.MedicRank = r;
                    break;
                case RTORank r:
                    actual.PromotionFor.RTORank = r;
                    break;
                case PilotRank r:
                    actual.PromotionFor.PilotRank = r;
                    actual.PromotionFor.LastPromotion = now;
                    break;
                case WardenRank r:
                    actual.PromotionFor.WardenRank = r;
                    actual.PromotionFor.LastPromotion = now;
                    break;
                case WarrantRank r:
                    actual.PromotionFor.WarrantRank = r;
                    actual.PromotionFor.LastPromotion = now;
                    break;
            }

            _dbContext.Remove(actual);

            await _dbContext.SaveChangesAsync();

            if(ulong.TryParse(actual.PromotionFor.DiscordId, out var did))
                await _discord.UpdateRankChangeAsync(log, did, actual.PromotionFor.Id);

            return new(true, null);
        }

        public async Task<PromotionResult> StartPromotionProcessAsync(ClaimsPrincipal invoker, Trooper promotionFor,
            PromotionBoardLevel currentBoard,
            int promotionFrom, int promotionTo, string reason, bool forced)
        {
            var invoked = await _userManager.GetUserAsync(invoker);
            var trooper = await _userManager.FindByIdAsync(promotionFor.Id.ToString());

            var promoReq = await _webSettings.GetPromotionRequirementsAsync(promotionTo);

            if (promoReq is null)
            {
                return new(false, null, new() { "No promotion board data for the rank this trooper is being promoted to." });
            }

            // Put this on the highest needed board in the event this person is
            // eligible.
            PromotionBoardLevel neededLevel;
            if (currentBoard > promoReq.NeededLevel)
            {
                // If the promoton needs to go to battalion and is above (aka razor/mynock)
                // let it check for a lower board.
                if (currentBoard > PromotionBoardLevel.Battalion
                    && promoReq.NeededLevel == PromotionBoardLevel.Battalion)
                {
                    neededLevel = promoReq.NeededLevel;
                }
                else 
                {
                    neededLevel = currentBoard;
                }
            }
            else
            {
                neededLevel = promoReq.NeededLevel;
            }

            var promotion = new Promotion()
            {
                StartingBoard = currentBoard,
                CurrentBoard = currentBoard,
                NeededBoard = neededLevel,

                PromoteFrom = promotionFrom,
                PromoteTo = promotionTo,
                Reason = reason,

                RequestedById = invoked.Id,
                Forced = forced
            };

            trooper.PendingPromotions.Add(promotion);

            var identRes = await _userManager.UpdateAsync(trooper);
            if (!identRes.Succeeded)
            {
                List<string> errors = new();
                foreach (var error in identRes.Errors)
                    errors.Add($"[{error.Code}] {error.Description}");

                return new(false, null, errors);
            }

            using var _dbContext = _dbContextFactory.CreateDbContext();

            var promo = await _dbContext.Promotions.FindAsync(promotion.Id);

            if (promo is null) return new(false, null, new() { "No promotion object was abled to be retrieved." });

            await _dbContext.Entry(promo).Reference(e => e.PromotionFor).LoadAsync();
            await _dbContext.Entry(promo).Reference(e => e.RequestedBy).LoadAsync();

            return new(true, promo, null);
        }

        public async Task<PromotionResult> StartPromotionProcessAsync(ClaimsPrincipal invoker, Promotion promotion)
            => await StartPromotionProcessAsync(invoker, promotion.PromotionFor,
                promotion.CurrentBoard, promotion.PromoteFrom, promotion.PromoteTo,
                promotion.Reason, promotion.Forced);

        public async Task<ResultBase> UpdatePromotionAsync(Promotion promotion, string reason)
        {
            using var _dbContext = _dbContextFactory.CreateDbContext();
            var actual = await _dbContext.Promotions.FindAsync(promotion.Id);
            if (string.IsNullOrEmpty(reason)) return new(false, new() { "Reason String Null or Empty." });
            if (actual is null) return new(false, new() { "Promotion was not found in the database." });
            actual.Reason = reason;
            var entriesModified = await _dbContext.SaveChangesAsync();
            if (entriesModified > 0) { return new(true); } else{ return new(false, new() { "SaveChangesAsync returned 0 entries modified." }); }
        }
    }
}
