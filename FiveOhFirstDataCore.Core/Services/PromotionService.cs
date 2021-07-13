using FiveOhFirstDataCore.Core.Account;
using FiveOhFirstDataCore.Core.Data;
using FiveOhFirstDataCore.Core.Data.Promotions;
using FiveOhFirstDataCore.Core.Database;
using FiveOhFirstDataCore.Core.Extensions;
using FiveOhFirstDataCore.Core.Structures;
using FiveOhFirstDataCore.Core.Structures.Updates;

using Microsoft.AspNetCore.Identity;

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
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<Trooper> _userManager;

        public PromotionService(ApplicationDbContext dbContext, UserManager<Trooper> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public async Task<ResultBase> CancelPromotion(Promotion promotion, Trooper denier)
        {
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
            var actual = await _dbContext.Promotions.FindAsync(promotion.Id);
            var user = await _userManager.FindByIdAsync(approver.Id.ToString());

            if(actual is null)
            {
                return new(false, new() { "Promotion was not found in the database." });
            }

            actual.CurrentBoard++;

            if (actual.CurrentBoard > actual.NeededBoard)
            {
                await _dbContext.SaveChangesAsync();
                return await FinalizePromotion(promotion, approver);
            }

            if (actual.ApprovedBy is null) actual.ApprovedBy = new();

            user.ApprovedPendingPromotions.Add(actual);

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

        public async Task<ResultBase> FinalizePromotion(Promotion promotion, Trooper approver)
        {
            var actual = await _dbContext.Promotions.FindAsync(promotion.Id);
            var user = await _userManager.FindByIdAsync(approver.Id.ToString());

            if (actual is null)
            {
                return new(false, new() { "Promotion was not found in the database." });
            }

            await _dbContext.Entry(actual).Collection(e => e.ApprovedBy).LoadAsync();
            await _dbContext.Entry(actual).Reference(e => e.PromotionFor).LoadAsync();
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

        public Task<Promotion> StartPromotionProcess(ClaimsPrincipal invoker, Trooper promotionFor, int promotionFrom, int promotionTo)
        {
            throw new NotImplementedException();
        }
    }
}
