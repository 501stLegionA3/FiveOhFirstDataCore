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
        public Task<Promotion> StartPromotionProcess(ClaimsPrincipal invoker, Trooper promotionFor,
            int promotionFrom, int promotionTo);
        /// <summary>
        /// Elevates a promotion to the next stage. Will finalize a promotion if able.
        /// </summary>
        /// <param name="promotion">Promotion to elevate.</param>
        /// <param name="approver">Trooper who approved the promotion.</param>
        /// <param name="levels">Ammount of levels to elevate the promotion by.</param>
        /// <returns></returns>
        public Task<ResultBase> ElevatePromotion(Promotion promotion, Trooper approver, int levels = 1);
        public Task<ResultBase> FinalizePromotion(Promotion promotion, Trooper approver);
        public Task<ResultBase> CancelPromotion(Promotion promotion, Trooper denier);
    }
}
