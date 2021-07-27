using FiveOhFirstDataCore.Core.Account;
using FiveOhFirstDataCore.Core.Data.Promotions;
using FiveOhFirstDataCore.Core.Structures;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Services
{
    public interface IPromotionService
    {
        public Task<PromotionResult> StartPromotionProcessAsync(ClaimsPrincipal invoker, Promotion promotion);
        public Task<PromotionResult> StartPromotionProcessAsync(ClaimsPrincipal invoker, Trooper promotionFor,
            PromotionBoardLevel currentBoard,
            int promotionFrom, int promotionTo, string reason);
        /// <summary>
        /// Elevates a promotion to the next stage. Will finalize a promotion if able.
        /// </summary>
        /// <param name="promotion">Promotion to elevate.</param>
        /// <param name="approver">Trooper who approved the promotion.</param>
        /// <param name="levels">Ammount of levels to elevate the promotion by.</param>
        /// <returns></returns>
        public Task<ResultBase> ElevatePromotionAsync(Promotion promotion, Trooper approver, int levels = 1);
        public Task<ResultBase> FinalizePromotionAsync(Promotion promotion, Trooper approver);
        public Task<ResultBase> CancelPromotionAsync(Promotion promotion, Trooper denier);
    }
}
