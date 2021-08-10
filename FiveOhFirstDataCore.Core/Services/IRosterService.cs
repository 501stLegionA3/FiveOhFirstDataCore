using FiveOhFirstDataCore.Core.Account;
using FiveOhFirstDataCore.Core.Data;
using FiveOhFirstDataCore.Core.Data.Roster;
using FiveOhFirstDataCore.Core.Structures;
using FiveOhFirstDataCore.Core.Structures.Updates;

using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Services
{
    public interface IRosterService
    {
        #region Roster Data
        /// <summary>
        /// Get all troopers in the placed roster.
        /// </summary>
        /// <returns>A <see cref="Task"/> that returns <see cref="List{T}"/> of type <see cref="Trooper"/> 
        /// where each <see cref="Trooper"/> is in the placed roster.</returns>
        public Task<List<Trooper>> GetPlacedRosterAsync();
        /// <summary>
        /// Get all troopers in active reserves.
        /// </summary>
        /// <returns>A <see cref="Task"/> that returns <see cref="List{T}"/> of type <see cref="Trooper"/> 
        /// where each <see cref="Trooper"/> is in active reserves.</returns>
        public Task<List<Trooper>> GetActiveReservesAsync();
        /// <summary>
        /// Get all troopers in inactive reserves.
        /// </summary>
        /// <returns>A <see cref="Task"/> that returns <see cref="List{T}"/> of type <see cref="Trooper"/> 
        /// where each <see cref="Trooper"/> is in inactive reserves.</returns>
        public Task<List<Trooper>> GetInactiveReservesAsync();
        /// <summary>
        /// Get all troopers that are archived..
        /// </summary>
        /// <returns>A <see cref="Task"/> that returns <see cref="List{T}"/> of type <see cref="Trooper"/> 
        /// where each <see cref="Trooper"/> is archived.</returns>
        public Task<List<Trooper>> GetArchivedTroopersAsync();
        /// <summary>
        /// Get all troopers.
        /// </summary>
        /// <param name="includeAdmin">If the all trooper list should inlcude the Admin account.</param>
        /// <returns>A <see cref="Task"/> that returns <see cref="List{T}"/> of type <see cref="Trooper"/> 
        /// containing all troopers.</returns>
        public Task<List<Trooper>> GetAllTroopersAsync(bool includeAdmin = false);
        /// <summary>
        /// Get all troopers that are placed or in active reserves.
        /// </summary>
        /// <returns>A <see cref="Task"/> that returns <see cref="List{T}"/> of type <see cref="Trooper"/>
        /// <param name="includePromotions">A <see cref="bool"/> value that deterines if the 
        /// request should include promotions. Defaults to false.</param>
        /// where each <see cref="Trooper"/> is placed or in active reserves.</returns>
        public Task<List<Trooper>> GetFullRosterAsync(bool includePromotions = false);
        /// <summary>
        /// Get all troopers that have not registered their account and are not archvied.
        /// </summary>
        /// <returns>A <see cref="Task"/> that returns <see cref="List{T}"/> of type <see cref="Trooper"/> 
        /// where each <see cref="Trooper"/> has not registered their account.</returns>
        public Task<List<Trooper>> GetUnregisteredTroopersAsync();
        /// <summary>
        /// Get the ORBAT data.
        /// </summary>
        /// <returns>A <see cref="Task"/> that returns <see cref="OrbatData"/> for the ORBAT.</returns>
        public Task<OrbatData> GetOrbatDataAsync();
        /// <summary>
        /// Get the Zeta ORBAT data.
        /// </summary>
        /// <returns>A <see cref="Task"/> that retruns <see cref="ZetaOrbatData" /> for the Zeta ORBAT.</returns>
        public Task<ZetaOrbatData> GetZetaOrbatDataAsync();
        /// <summary>
        /// Get the in user user data.
        /// </summary>
        /// <remarks>
        /// This method returns a <see cref="Tuple{T1, T2}"/> value where T1 is an <see cref="HashSet{T}"/> of <see cref="int"/>s
        /// and T2 is a <see cref="HashSet{T}"/> of <see cref="string"/>s. T1 is the used User IDs for the unit, while T2 is the
        /// non-inactive nicknames for the unit.
        /// </remarks>
        /// <returns></returns>
        public Task<(HashSet<int>, HashSet<string>)> GetInUseUserDataAsync();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="claims"></param>
        /// <returns></returns>
        public Task<Trooper?> GetTrooperFromClaimsPrincipalAsync(ClaimsPrincipal claims);
        public Task<Trooper?> GetTrooperFromIdAsync(int id);
        public Task<List<Trooper>> GetDirectSubordinates(Trooper t);
        public Task<IAssignable<Trooper>?> GetSquadDataFromSlotAsync(Slot slot, bool manager);
        public Task<IAssignable<Trooper>?> GetSquadDataFromClaimPrincipalAsync(ClaimsPrincipal claims);
        public Task<IAssignable<Trooper>?> GetPlatoonDataFromSlotAsync(Slot slot, bool manager);
        public Task<IAssignable<Trooper>?> GetPlatoonDataFromClaimPrincipalAsync(ClaimsPrincipal claims);
        public Task<IAssignable<Trooper>?> GetCompanyDataFromSlotAsync(Slot slot, bool manager);
        public Task<IAssignable<Trooper>?> GetCompanyDataFromClaimPrincipalAsync(ClaimsPrincipal claims);
        public Task<HailstormData> GetHailstormDataAsync();
        public Task<List<Trooper>> GetTroopersWithPendingPromotionsAsync();
        public Task<RazorSquadronData> GetRazorDataAsync();
        public Task<WardenData> GetWardenDataAsync();
        public Task<MynockDetachmentData> GetMynockDataAsync();
        public Task<MynockSectionData?> GetMynockDataFromSlotAsync(Slot slot, bool manager);
        public Task<MynockSectionData?> GetMynockDataFromClaimPrincipalAsync(ClaimsPrincipal claims);
        public Task<ZetaUTCSectionData> GetZetaUTCSectionDataAsync();
        public Task<ZetaUTCSquadData> GetZetaUTCSquadFromSlotAsync(Slot slot);
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
        /// <summary>
        /// Ensures all archived troopers have the Archived role.
        /// </summary>
        /// <returns>A Task representing this action.</returns>
        public Task ValidateArchivedTroopersAsync();
        #endregion
    }
}
