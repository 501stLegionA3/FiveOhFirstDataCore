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
        public Task<ResultBase> ElevatePromotion(Promotion promotion, int levels = 1);
        public Task<ResultBase> FinalizePromotion(Promotion promotion);
        public Task<ResultBase> CancelPromotion(Promotion promotion);
    }
}
