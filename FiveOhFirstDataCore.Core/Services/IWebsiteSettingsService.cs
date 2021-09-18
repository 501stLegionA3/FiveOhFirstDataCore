using FiveOhFirstDataCore.Data.Account;
using FiveOhFirstDataCore.Data.Structures;
using FiveOhFirstDataCore.Data.Structures.Promotions;
using FiveOhFirstDataCore.Data.Structures;
using FiveOhFirstDataCore.Core.Structures.Auth;
using FiveOhFirstDataCore.Core.Structures.Policy;

using System.Collections.Concurrent;
using System.Security.Claims;

namespace FiveOhFirstDataCore.Data.Services
{
    public interface IWebsiteSettingsService
    {
        /// <summary>
        /// Initalizes this service.
        /// </summary>
        /// <returns>A task represeting this action.</returns>
        public Task InitalizeAsync();
        /// <summary>
        /// Populate the database with default settings.
        /// </summary>
        /// <remarks>
        /// This action will remove all other database settings before adding the defaults back. It is a destructive action.
        /// </remarks>
        /// <returns>A <see cref="Task"/> representing this action.</returns>
        public Task SetDefaultSettingsAsync();
        /// <summary>
        /// Get the promotion requirements for a rank.
        /// </summary>
        /// <param name="rank">A <see cref="int"/> that represents a rank.</param>
        /// <returns>A <see cref="Task"/> that returns <see cref="PromotionDetails?"/> for a rank.</returns>
        public Task<PromotionDetails?> GetPromotionRequirementsAsync(int rank);
        /// <summary>
        /// Get the eligible promotions for a <see cref="Trooper"/>
        /// </summary>
        /// <param name="forTrooper">The <see cref="Trooper"/> to get promotions for.</param>
        /// <param name="filterOutTrooperToWarrant">A <see cref="bool"/> indicating if this action should 
        /// filter out promtions into a <see cref="WarrantRank"/></param>
        /// <param name="filterOutPendingPromotions">A <see cref="bool"/> indicating if this action should 
        /// filter out current pending promotions.</param>
        /// <returns>A <see cref="Task"/> that returns <see cref="IReadOnlyList{T}"/> of 
        /// type <see cref="Promotion"/> for a <see cref="Trooper"/></returns>
        public Task<IReadOnlyList<Promotion>> GetEligiblePromotionsAsync(Trooper forTrooper, bool filterOutTrooperToWarrant = false, bool filterOutPendingPromotions = true);
        /// <summary>
        /// Get the saved promotion details from the database.
        /// </summary>
        /// <returns>A <see cref="Task"/> that returns a <see cref="Dictionary{TKey, TValue}"/> of key <see cref="int"/> 
        /// and value <see cref="PromotionDetails"/> representing the requirements of promotions.</returns>
        public Task<Dictionary<int, PromotionDetails>> GetSavedPromotionDetails();
        /// <summary>
        /// Get the full C-Shop claims tree from the database.
        /// </summary>
        /// <returns>A <see cref="Task"/> that returns a <see cref="Dictionary{TKey, TValue}"/> of key <see cref="CShop"/> 
        /// and value <see cref="CShopClaim"/> representing the full C-Shop claim tree.</returns>
        public Task<Dictionary<CShop, CShopClaim>> GetFullClaimsTreeAsync();
        /// <summary>
        /// Get the claim data for a single <see cref="CShop"/>
        /// </summary>
        /// <param name="key">The <see cref="CShop"/> to get claim data for.</param>
        /// <returns>A <see cref="Task"/> that returns the claim data for the requested <see cref="CShop"/></returns>
        public Task<Dictionary<string, List<string>>> GetClaimDataForCShopAsync(CShop key);
        /// <summary>
        /// Reload the Cached claim tree.
        /// </summary>
        /// <returns>A <see cref="Task"/> represenitng this action.</returns>
        public Task ReloadClaimTreeAsync();
        /// <summary>
        /// Get the cached claim tree.
        /// </summary>
        /// <remarks>
        /// This method will only get cached data that was saved with <see cref="ReloadClaimTreeAsync"/> and not
        /// the current data from the database. This is meant to be used in claim/permission checks and not in 
        /// standard website operation. Use <seealso cref="GetFullClaimsTreeAsync"/> unless you are checking permissions.
        /// </remarks>
        /// <returns>A <see cref="Task"/> that returns a <see cref="Dictionary{TKey, TValue}"/> of key <see cref="CShop"/> 
        /// and value <see cref="CShopClaim"/> representing the full C-Shop claim tree from the cache.</returns>
        public Task<Dictionary<CShop, CShopClaim>> GetCachedCShopClaimTreeAsync();
        /// <summary>
        /// Get the Discord role IDs for a C-Shop <see cref="Claim"/>
        /// </summary>
        /// <param name="claim">The <see cref="Claim"/> to get Discord role IDs for.</param>
        /// <returns>A <see cref="Task"/> that returns <see cref="IReadOnlyList{T}"/> of type <see cref="ulong"/> where each <see cref="ulong"/> is a Discord role ID.</returns>
        public Task<IReadOnlyList<ulong>?> GetCShopDiscordRolesAsync(Claim claim);
        /// <summary>
        /// Get the Discord role details for a rank.
        /// </summary>
        /// <param name="key">The <see cref="Enum"/> value of a rank.</param>
        /// <returns>A <see cref="Task"/> that returns a <see cref="DiscordRoleDetails"/> for the requested rank.</returns>
        public Task<DiscordRoleDetails?> GetDiscordRoleDetailsAsync(Enum key);
        /// <summary>
        /// Override current C-Shop claim settings with the inputed data.
        /// </summary>
        /// <remarks>
        /// This action is destructive and will make the database match the values in <paramref name="claimTree"/>. Any
        /// values that are in the database but not in <paramref name="claimTree"/> will be removed from the
        /// database.
        /// </remarks>
        /// <param name="claimTree">A <see cref="Dictionary{TKey, TValue}"/> of key <see cref="CShop"/> and value <see cref="CShopClaim"/> to update the database with.</param>
        /// <returns>A <see cref="Task"/> representing this action.</returns>
        public Task OverrideCShopClaimSettingsAsync(Dictionary<CShop, CShopClaim> claimTree);
        /// <summary>
        /// Override the current Promotion Requrement settings with the inputed data.
        /// </summary>
        /// <remarks>
        /// This action is destructive and will make the database match the values in <paramref name="details"/>. Any
        /// values that are in the database but not in <paramref name="details"/> will be removed from the
        /// database.
        /// </remarks>
        /// <param name="details">A <see cref="Dictionary{TKey, TValue}"/> of key <see cref="int"/> and value <see cref="PromotionDetails"/> to update the database with.</param>
        /// <returns>A <see cref="Task"/> representing this action.</returns>
        public Task OverridePromotionRequirementsAsync(Dictionary<int, PromotionDetails> details);
        /// <summary>
        /// Sets the forced value on a <see cref="Promotion"/> to false.
        /// </summary>
        /// <param name="promotion">The <see cref="Promotion"/> to modify.</param>
        /// <returns>A <see cref="Task"/> representing this action.</returns>
        public Task RemoveForcedTagAsync(Promotion promotion);

        #region Dynamic Policies
        public Task ReloadPolicyCacheAsync();
        public Task<DynamicPolicyAuthorizationPolicyBuilder?> GetPolicyBuilder(string sectionName, bool forceCacheReload = false);
        public Task<ResultBase> CreatePolicy(DynamicPolicy policy);
        public Task<ResultBase> UpdatePolicy(DynamicPolicy policy);
        public Task<ResultBase> UpdateOrCreatePolicy(DynamicPolicy policy);
        public Task<ResultBase> UpdatePolicySection(PolicySection policySection);
        public Task<ResultBase> DeletePolicy(DynamicPolicy policy, DynamicPolicy? assignFloatingSectionsTo = null);
        #endregion
    }
}
