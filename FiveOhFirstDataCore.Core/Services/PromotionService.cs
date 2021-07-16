using FiveOhFirstDataCore.Core.Account;
using FiveOhFirstDataCore.Core.Data;
using FiveOhFirstDataCore.Core.Data.Promotions;
using FiveOhFirstDataCore.Core.Database;
using FiveOhFirstDataCore.Core.Extensions;
using FiveOhFirstDataCore.Core.Structures;
using FiveOhFirstDataCore.Core.Structures.Updates;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Services
{
    public class PromotionService : IPromotionService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
        private readonly UserManager<Trooper> _userManager;
        private readonly IWebsiteSettingsService _webSettings;

        public PromotionService(IDbContextFactory<ApplicationDbContext> dbContextFactory, UserManager<Trooper> userManager,
            IWebsiteSettingsService webSettings)
        {
            _dbContextFactory = dbContextFactory;
            _userManager = userManager;
            _webSettings = webSettings;
        }

        public async Task<ResultBase> CancelPromotion(Promotion promotion, Trooper denier)
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

        public async Task<ResultBase> ElevatePromotion(Promotion promotion, Trooper approver, int levels = 1)
        {
            using var _dbContext = _dbContextFactory.CreateDbContext();
            var actual = await _dbContext.Promotions.FindAsync(promotion.Id);

            if(actual is null)
            {
                return new(false, new() { "Promotion was not found in the database." });
            }

            var user = await _dbContext.FindAsync<Trooper>(approver.Id);
            await _dbContext.Entry(actual).Collection(e => e.ApprovedBy).LoadAsync();

            actual.CurrentBoard++;

            if (actual.CurrentBoard > actual.NeededBoard)
            {
                await _dbContext.SaveChangesAsync();
                return await FinalizePromotion(promotion, approver);
            }

            if (actual.ApprovedBy is null) actual.ApprovedBy = new();

            if(user is not null && !actual.ApprovedBy.Any(x => x.Id == user.Id))
                user.ApprovedPendingPromotions.Add(actual);

            await _dbContext.SaveChangesAsync();

            return new(true, null);
        }

        public async Task<ResultBase> FinalizePromotion(Promotion promotion, Trooper approver)
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

            if(user is null)
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
            switch(rank)
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

            return new(true, null);
        }

        public async Task<PromotionResult> StartPromotionProcess(ClaimsPrincipal invoker, Trooper promotionFor,
            PromotionBoardLevel currentBoard,
            int promotionFrom, int promotionTo, string reason)
        {
            var invoked = await _userManager.GetUserAsync(invoker);
            var trooper = await _userManager.FindByIdAsync(promotionFor.Id.ToString());

            var promoReq = await _webSettings.GetPromotionRequirementsAsync(promotionTo);

            if (promoReq is null)
            {
                return new(false, null, new() { "No promotion board data for the rank this trooper is being promoted to." });
            }

            var promotion = new Promotion()
            {
                CurrentBoard = currentBoard,
                NeededBoard = promoReq.NeededLevel,

                PromoteFrom = promotionFrom,
                PromoteTo = promotionTo,
                Reason = reason,

                RequestedById = invoked.Id
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

        public async Task<PromotionResult> StartPromotionProcess(ClaimsPrincipal invoker, Promotion promotion)
            => await StartPromotionProcess(invoker, promotion.PromotionFor,
                promotion.CurrentBoard, promotion.PromoteFrom, promotion.PromoteTo,
                promotion.Reason);
    }
}
