using FiveOhFirstDataCore.Core.Account;
using FiveOhFirstDataCore.Core.Data;
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
    public interface IWebsiteSettingsService
    {
        public Task SetDefaultSettingsAsync();
        public Task<PromotionDetails?> GetPromotionRequirementsAsync(int rank);
        public Task<IReadOnlyList<Promotion>> GetEligiblePromotionsAsync(Trooper forTrooper);
        public Task<Dictionary<CShop, CShopClaim>> GetFullClaimsTreeAsync();
        public Task<Dictionary<string, List<string>>> GetClaimDataForCShopAsync(CShop key);
        public Task ReloadClaimTreeAsync();
        public Task<Dictionary<CShop, CShopClaim>> GetCachedCShopClaimTreeAsync();
        public Task<IReadOnlyList<ulong>?> GetCshopDiscordRolesAsync(Claim claim);
        public Task<DiscordRoleDetails?> GetDiscordRoleDetailsAsync(Enum key);
        public Task OverrideCShopClaimSettingsAsync(Dictionary<CShop, CShopClaim> claimTree);
    }
}
