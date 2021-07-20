using FiveOhFirstDataCore.Core.Account;
using FiveOhFirstDataCore.Core.Data.Promotions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Services
{
    public interface IWebsiteSettingsService
    {
        public Task SetDefaultSettings();
        public Task<PromotionDetails?> GetPromotionRequirementsAsync(int rank);
        public Task<IReadOnlyList<Promotion>> GetEligiblePromotionsAsync(Trooper forTrooper);
    }
}
