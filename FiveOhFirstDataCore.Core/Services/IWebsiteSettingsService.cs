using FiveOhFirstDataCore.Data.Account;
using FiveOhFirstDataCore.Data.Structures;
using FiveOhFirstDataCore.Data.Structures.Promotions;
using FiveOhFirstDataCore.Data.Structures;
using FiveOhFirstDataCore.Core.Structures.Auth;
using FiveOhFirstDataCore.Core.Structures.Policy;

using System.Collections.Concurrent;
using System.Security.Claims;
using FiveOhFirstDataCore.Data.Structures.Discord;

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
        /// Get the Discord role details for a rank.
        /// </summary>
        /// <param name="qualifiedKey">The <see cref="string"/> value of a rank.</param>
        /// <returns>A <see cref="Task"/> that returns a <see cref="DiscordRoleDetails"/> for the requested rank.</returns>
        public Task<DiscordRoleDetails?> GetDiscordRoleDetailsAsync(string qualifiedKey);
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
        /// <summary>
        /// Reloads the polic cahce used by the permissions system.
        /// </summary>
        /// <returns>A task representing this action.</returns>
        public Task ReloadPolicyCacheAsync();
        /// <summary>
        /// Get a specific policy builder by section name.
        /// </summary>
        /// <param name="sectionName">The section name to get a policy builder for.</param>
        /// <param name="forceCacheReload">Set to true to force a cache to reload before reteriving the policy builder.</param>
        /// <returns>A task that returns a <see cref="DynamicPolicyAuthorizationPolicyBuilder"/> for the provided <paramref name="sectionName"/></returns>
        public Task<DynamicPolicyAuthorizationPolicyBuilder?> GetPolicyBuilderAsync(string sectionName, bool forceCacheReload = false);
        /// <summary>
        /// Create a new policy.
        /// </summary>
        /// <param name="policy">The <see cref="DynamicPolicy"/> to create.</param>
        /// <returns>A task that returns a <see cref="ResultBase"/> for this action.</returns>
        public Task<ResultBase> CreatePolicyAsync(DynamicPolicy policy);
        /// <summary>
        /// Update an existing policy.
        /// </summary>
        /// <param name="policy">The <see cref="DynamicPolicy"/> to update.</param>
        /// <returns>A task that returns a <see cref="ResultBase"/> for this action.</returns>
        public Task<ResultBase> UpdatePolicyAsync(DynamicPolicy policy);
        /// <summary>
        /// Gets all <see cref="DynamicPolicy"/>s in the database.
        /// </summary>
        /// <returns>A task that returns a <see cref="List{T}"/> of <see cref="DynamicPolicy"/>s.</returns>
        public Task<List<DynamicPolicy>> GetDynamicPoliciesAsync();
        /// <summary>
        /// Gets all <see cref="PolicySection"/>s in the database.
        /// </summary>
        /// <returns>A task that returns a <see cref="List{T}"/> of <see cref="PolicySection"/>s.</returns>
        public Task<List<PolicySection>> GetAllPolicySectionsAsync();
        /// <summary>
        /// Retreive a sepcific <see cref="DynamicPolicy"/> from the database.
        /// </summary>
        /// <param name="policyName">The name of the policy to get.</param>
        /// <returns>A task that returns a <see cref="DynamicPolicy"/> for the provided <paramref name="policyName"/></returns>
        public Task<DynamicPolicy?> GetDynamicPolicyAsync(string policyName);
        /// <summary>
        /// Updates or creates a new <see cref="DynamicPolicy"/>.
        /// </summary>
        /// <param name="policy">The <see cref="DynamicPolicy"/> to update or create.</param>
        /// <returns>A task that returns a <see cref="ResultBase"/> for this action.</returns>
        public Task<ResultBase> UpdateOrCreatePolicyAsync(DynamicPolicy policy);
        /// <summary>
        /// Update a <see cref="PolicySection"/> in the database.
        /// </summary>
        /// <param name="policySection">The <see cref="PolicySection"/> to update.</param>
        /// <returns>A task that returns a <see cref="ResultBase"/> for this action.</returns>
        public Task<ResultBase> UpdatePolicySectionAsync(PolicySection policySection);
        /// <summary>
        /// Get or create a <see cref="PolicySection"/>.
        /// </summary>
        /// <param name="sectionName">The section name for the <see cref="PolicySection"/> to get or create.</param>
        /// <returns>A task that returns a <see cref="PolicySectionResult"/> for this action.</returns>
        public Task<PolicySectionResult> GetOrCreatePolicySectionAsync(string sectionName);
        /// <summary>
        /// Delete a <see cref="DynamicPolicy"/> from the database.
        /// </summary>
        /// <param name="policy">The <see cref="DynamicPolicy"/> to delete.</param>
        /// <param name="assignOrphanedSectionsTo">A <see cref="DynamicPolicy"/> to assign all sections that were assigned to the <paramref name="policy"/>
        /// after it has been deleted.</param>
        /// <returns>A task that returns a <see cref="ResultBase"/> for this action.</returns>
        public Task<ResultBase> DeletePolicyAsync(DynamicPolicy policy, DynamicPolicy? assignOrphanedSectionsTo = null);
        /// <summary>
        /// Delete a <see cref="PolicySection"/> from the database.
        /// </summary>
        /// <param name="section">The <see cref="PolicySection"/> to delete.</param>
        /// <returns>A task that returns a <see cref="ResultBase"/> for this action.</returns>
        public Task<ResultBase> DeletePolicySectionAsync(PolicySection section);
        #endregion

        #region Discord Bindings
        public Task<ResultBase> AddOrUpdateDiscordBindingsAsync(DiscordRoleDetails roleDetails);
        public Task<ResultBase> AddOrUpdateCShopRoleBindingAsync(CShopRoleBindingData data);
        public Task<List<DiscordRoleDetails>> GetAllDiscordBindingsAsync();
        public Task<List<CShopRoleBindingData>> GetAllCShopRoleBindingDataAsync();
        public Task<ResultBase> DeleteDiscordBindingAsync(DiscordRoleDetails roleDetails);
        public Task<ResultBase> DeleteCShopRoleBindingDataAsync(CShopRoleBindingData data);
        public Task<CShopRoleBindingData> GetCShopRoleBindingAsync(Guid key);
        public Task<CShop?> ValidateCShopRoleBindClusterAsync(CShop cluster);
        public Task<Guid?> ValidateCShopRoleBindDepartmentAsync(string department, CShop forCluster);
        #endregion

        public Task<DiscordPostActionConfiguration?> GetDiscordPostActionConfigurationAsync(DiscordAction action);
        public Task<ResultBase> UpdateDiscordPostActionConfigurationAsync(DiscordAction action, ulong channelId, string message);
    }
}
