using FiveOhFirstDataCore.Core.Account;
using FiveOhFirstDataCore.Core.Data;
using FiveOhFirstDataCore.Core.Data.Roster;
using FiveOhFirstDataCore.Core.Structures;
using FiveOhFirstDataCore.Core.Structures.Updates;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Services
{
    public interface IRosterService
    {
        #region Roster Data
        public Task<List<Trooper>> GetPlacedRosterAsync();
        public Task<List<Trooper>> GetActiveReservesAsync();
        public Task<List<Trooper>> GetInactiveReservesAsync();
        public Task<List<Trooper>> GetArchivedTroopersAsync();
        public Task<List<Trooper>> GetAllTroopersAsync(bool includeAdmin = false);
        public Task<List<Trooper>> GetFullRosterAsync();
        public Task<List<Trooper>> GetUnregisteredTroopersAsync();
        public Task<OrbatData> GetOrbatDataAsync();
        public Task<ZetaOrbatData> GetZetaOrbatDataAsync();
        public Task<(HashSet<int>, HashSet<string>)> GetInUseUserDataAsync();
        public Task<Trooper?> GetTrooperFromClaimsPrincipalAsync(ClaimsPrincipal claims);
        public Task<Trooper?> GetTrooperFromIdAsync(int id);
        public Task<List<Trooper>> GetDirectSubordinates(Trooper t);
        public Task<SquadData?> GetSquadDataFromSlotAsync(Slot slot, bool manager);
        public Task<SquadData?> GetSquadDataFromClaimPrincipalAsync(ClaimsPrincipal claims);
        public Task<PlatoonData?> GetPlatoonDataFromSlotAsync(Slot slot, bool manager);
        public Task<PlatoonData?> GetPlatoonDataFromClaimPrincipalAsync(ClaimsPrincipal claims);
        public Task<CompanyData?> GetCompanyDataFromSlotAsync(Slot slot, bool manager);
        public Task<CompanyData?> GetCompanyDataFromClaimPrincipalAsync(ClaimsPrincipal claims);
        public Task<HailstormData> GetHailstormDataAsync();
        public Task<List<Trooper>> GetTroopersWithPendingPromotions();
        #endregion

        #region Roster Registration
        public Task<RegisterTrooperResult> RegisterTrooper(NewTrooperData trooperData, ClaimsPrincipal user);
        #endregion

        #region Data Updates
        public Task<Dictionary<CShop, List<ClaimUpdateData>>> GetCShopClaimsAsync(Trooper trooper);
        public Task<ResultBase> UpdateAsync(Trooper edit, List<ClaimUpdateData> claimsToAdd, List<ClaimUpdateData> claimsToRemove, ClaimsPrincipal submitter);
        public Task SaveNewFlag(ClaimsPrincipal claim, Trooper trooper, TrooperFlag flag);
        public Task<ResultBase> UpdateAllowedNameChangersAsync(List<Trooper> allowedTroopers);
        public Task<ResultBase> UpdateNickNameAsync(Trooper trooper, int approver);
        public Task<RegisterTrooperResult> ResetAccountAsync(Trooper trooper);
        #endregion

        #region Permissions
        public Task<bool[]> GetC1PermissionsAsync(ClaimsPrincipal claims);
        public Task<bool[]> GetC3PermissionsAsync(ClaimsPrincipal claims);
        public Task<bool[]> GetC4PermissionsAsync(ClaimsPrincipal claims);
        public Task<bool[]> GetC5PermissionsAsync(ClaimsPrincipal claims);
        public Task<bool[]> GetC6PermissionsAsync(ClaimsPrincipal claims);
        public Task<bool[]> GetC7PermissionsAsync(ClaimsPrincipal claims);
        public Task<bool[]> GetC8PermissionsAsync(ClaimsPrincipal claims);
        /// <summary>
        /// Checks the premissions of an ID to determine its permission level.
        /// </summary>
        /// <param name="id">ID of a Trooper.</param>
        /// <returns>Two true or false values for if the user is an admin and if they are a manager. Item1 is for Admin, Item2 is for Manager.</returns>
        public Task<(bool, bool)> GetAdminAndManagerValuesAsync(string id);
        /// <summary>
        /// Updates the permissions of an ID to match the admin and manager roles.
        /// </summary>
        /// <param name="admin">Is the ID an Admin?</param>
        /// <param name="manager">Is the ID a Manager?</param>
        /// <param name="id">The ID representing the Trooper.</param>
        /// <returns>A Task for this operation.</returns>
        public Task SaveAdminAndManagerValuesAsync(bool admin, bool manager, string id);
        /// <summary>
        /// Gets the Troopers who are allowed to change names.
        /// </summary>
        /// <returns>A list of troopers explicitly allowed to change names.</returns>
        public Task<List<Trooper>> GetAllowedNameChangersAsync();
        #endregion

        #region Data Loading
        public Task LoadPublicProfileDataAsync(Trooper trooper);
        #endregion

        #region Account Management
        public Task<ResultBase> UpdateUserNameAsync(Trooper trooper);
        public Task<ResultBase> DeleteAccountAsync(Trooper trooper, string password, ClaimsPrincipal claims);
        public Task<ResultBase> AddClaimAsync(Trooper trooper, Claim claim, int manager);
        public Task<ResultBase> RemoveClaimAsync(Trooper trooper, Claim claim, int manager);
        public Task<List<Claim>> GetAllClaimsFromTrooperAsync(Trooper trooper);
        #endregion
    }
}
