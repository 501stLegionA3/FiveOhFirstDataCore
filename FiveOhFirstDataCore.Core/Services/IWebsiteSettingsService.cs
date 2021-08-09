using FiveOhFirstDataCore.Core.Account;
using FiveOhFirstDataCore.Core.Data;
using FiveOhFirstDataCore.Core.Data.Promotions;
using FiveOhFirstDataCore.Core.Structures;

using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Services
{
    public interface IWebsiteSettingsService
    {
        public Task SetDefaultSettingsAsync();
        public Task<PromotionDetails?> GetPromotionRequirementsAsync(int rank);
        public Task<IReadOnlyList<Promotion>> GetEligiblePromotionsAsync(Trooper forTrooper, bool filterOutTrooperToWarrant = false);
        public Task<Dictionary<int, PromotionDetails>> GetSavedPromotionDetails();
        public Task<Dictionary<CShop, CShopClaim>> GetFullClaimsTreeAsync();
        public Task<Dictionary<string, List<string>>> GetClaimDataForCShopAsync(CShop key);
        public Task ReloadClaimTreeAsync();
        public Task<Dictionary<CShop, CShopClaim>> GetCachedCShopClaimTreeAsync();
        public Task<IReadOnlyList<ulong>?> GetCShopDiscordRolesAsync(Claim claim);
        public Task<DiscordRoleDetails?> GetDiscordRoleDetailsAsync(Enum key);
        public Task OverrideCShopClaimSettingsAsync(Dictionary<CShop, CShopClaim> claimTree);
        public Task OverridePromotionRequirementsAsync(Dictionary<int, PromotionDetails> details);
        public Task RemoveForcedTagAsync(Promotion promotion);
    }
}
