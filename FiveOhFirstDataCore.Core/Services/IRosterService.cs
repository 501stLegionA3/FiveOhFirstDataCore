using FiveOhFirstDataCore.Data.Account;
using FiveOhFirstDataCore.Data.Structures;
using FiveOhFirstDataCore.Data.Structures.Roster;
using FiveOhFirstDataCore.Data.Structures.Updates;

using System.Security.Claims;

namespace FiveOhFirstDataCore.Data.Services
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
        /// non-archived nicknames for the unit.
        /// </remarks>
        /// <returns></returns>
        public Task<(HashSet<int>, HashSet<string>)> GetInUseUserDataAsync();

        /// <summary>
        /// Gets a <see cref="Trooper"/> from a <see cref="ClaimsPrincipal"/>
        /// </summary>
        /// <param name="claims">The <see cref="ClaimsPrincipal"/> to get a trooper for.</param>
        /// <param name="loadNotificationTrackers">If this action should load trooper notification trackers.</param>
        /// <returns>A task that returns a <see cref="Trooper"/> object.</returns>
        public Task<Trooper?> GetTrooperFromClaimsPrincipalAsync(ClaimsPrincipal claims, bool loadNotificationTrackers = false);

        /// <summary>
        /// Gets a <see cref="Trooper"/> from their ID.
        /// </summary>
        /// <param name="id">The ID of the trooper.</param>
        /// <param name="loadNotificationTrackers">If this action should load trooper notification trackers.</param>
        /// <returns>A task that returns a <see cref="Trooper"/> object.</returns>
        public Task<Trooper?> GetTrooperFromIdAsync(int id, bool loadNotificationTrackers = false);

        /// <summary>
        /// Gets a <see cref="Trooper"/> from their Birth Number.
        /// </summary>
        /// <param name="birthNumber">The Birth Number of the trooper.</param>
        /// <param name="loadNotificationTrackers">If this action should load trooper notification trackers.</param>
        /// <returns>A task that returns a <see cref="Trooper"/> object.</returns>
        public Task<Trooper?> GetTrooperFromBirthNumberAsync(int birthNumber, bool loadNotificationTrackers = false);

        /// <summary>
        /// Get the direct subordinates of a <see cref="Trooper"/>
        /// </summary>
        /// <param name="t">The <see cref="Trooper"/> to get subordinates for.</param>
        /// <returns>A task that returns a <see cref="List{T}"/> of <see cref="Trooper"/>s.</returns>
        public Task<List<Trooper>> GetDirectSubordinates(Trooper t);

        /// <summary>
        /// Get the direct superior of a <see cref="Trooper"/>
        /// </summary>
        /// <param name="t">The <see cref="Trooper"/> to get the superior of.</param>
        /// <returns>A task that return a <see cref="Trooper"/>.</returns>
        public Task<Trooper?> GetDirectSuperior(Trooper t);

        /// <summary>
        /// Get all the troopers in a squad.
        /// </summary>
        /// <param name="slot">The <see cref="Slot"/> to get troopers for.</param>
        /// <param name="manager">A <see cref="bool"/> value indicating if the requester is a manager.</param>
        /// <returns>A task that returns an <see cref="IAssignable{T}"/> of <see cref="Trooper"/>s in a squad.</returns>
        public Task<IAssignable<Trooper>?> GetSquadDataAsync(Slot slot, bool manager);

        /// <summary>
        /// Get all the troopers in a squad.
        /// </summary>
        /// <param name="claims">The <see cref="ClaimsPrincipal"/> of the requester.</param>
        /// <returns>A task that returns an <see cref="IAssignable{T}"/> of <see cref="Trooper"/>s in a squad.</returns>
        public Task<IAssignable<Trooper>?> GetSquadDataAsync(ClaimsPrincipal claims);

        /// <summary>
        /// Get all the troopers in a platoon.
        /// </summary>
        /// <param name="slot">The <see cref="Slot"/> to get troopers for.</param>
        /// <param name="manager">A <see cref="bool"/> value indicating if the requester is a manager.</param>
        /// <returns>A task that returns an <see cref="IAssignable{T}"/> of <see cref="Trooper"/>s in a platoon.</returns>
        public Task<IAssignable<Trooper>?> GetPlatoonDataAsync(Slot slot, bool manager);

        /// <summary>
        /// Get all the troopers in a platoon.
        /// </summary>
        /// <param name="claims">The <see cref="ClaimsPrincipal"/> of the requester.</param>
        /// <returns>A task that returns an <see cref="IAssignable{T}"/> of <see cref="Trooper"/>s in a platoon.</returns>
        public Task<IAssignable<Trooper>?> GetPlatoonDataAsync(ClaimsPrincipal claims);

        /// <summary>
        /// Get all the troopers in a company.
        /// </summary>
        /// <param name="slot">The <see cref="Slot"/> to get troopers for.</param>
        /// <param name="manager">A <see cref="bool"/> value indicating if the requester is a manager.</param>
        /// <returns>A task that returns an <see cref="IAssignable{T}"/> of <see cref="Trooper"/>s in a company.</returns>
        public Task<IAssignable<Trooper>?> GetCompanyDataAsync(Slot slot, bool manager);

        /// <summary>
        /// Get all the troopers in a company.
        /// </summary>
        /// <param name="claims">The <see cref="ClaimsPrincipal"/> of the requester.</param>
        /// <returns>A task that returns an <see cref="IAssignable{T}"/> of <see cref="Trooper"/>s in a company.</returns>
        public Task<IAssignable<Trooper>?> GetCompanyDataAsync(ClaimsPrincipal claims);

        /// <summary>
        /// Gets the <see cref="HailstormData"/> for the unit.
        /// </summary>
        /// <returns>A task that returns a <see cref="HailstormData"/> object.</returns>
        public Task<HailstormData> GetHailstormDataAsync();

        /// <summary>
        /// Get all troopers that have pending promotions.
        /// </summary>
        /// <returns>A <see cref="List{T}"/> of <see cref="Trooper"/>s where each 
        /// <see cref="Trooper"/> has at least one value in <see cref="Trooper.PendingPromotions"/></returns>
        public Task<List<Trooper>> GetTroopersWithPendingPromotionsAsync();

        /// <summary>
        /// Gets the <see cref="RazorSquadronData"/> for the unit.
        /// </summary>
        /// <returns>A task that returns a <see cref="RazorSquadronData"/> object.</returns>
        public Task<RazorSquadronData> GetRazorDataAsync();

        /// <summary>
        /// Gets the <see cref="WardenData"/> for the unit.
        /// </summary>
        /// <returns>A task that returns a <see cref="WardenData"/> object.</returns>
        public Task<WardenData> GetWardenDataAsync();

        /// <summary>
        /// Gets the <see cref="MynockDetachmentData"/> for the unit.
        /// </summary>
        /// <returns>A task that returns a <see cref="MynockDetachmentData"/> object.</returns>
        public Task<MynockDetachmentData> GetMynockDataAsync();

        /// <summary>
        /// Gets mynock section data for the given <see cref="Slot"/>
        /// </summary>
        /// <param name="slot">The <see cref="Slot"/> to get data for.</param>
        /// <param name="manager">A <see cref="bool"/> value that indicates if the requester is a manager.</param>
        /// <returns>A task that returns <see cref="MynockSectionData"/> for the provided <paramref name="slot"/></returns>
        public Task<MynockSectionData?> GetMynockSectionDataAsync(Slot slot, bool manager);

        /// <summary>
        /// Gets mynock section data for the given <see cref="Slot"/>
        /// </summary>
        /// <param name="claims">The <see cref="ClaimsPrincipal"/> of the requester.</param>
        /// <returns>A task that returns <see cref="MynockSectionData"/> for the provided <paramref name="slot"/></returns>
        public Task<MynockSectionData?> GetMynockSectionDataAsync(ClaimsPrincipal claims);

        /// <summary>
        /// Get all Zeta UTC section data.
        /// </summary>
        /// <returns>A task that returns <see cref="ZetaUTCSectionData"/> for the unit.</returns>
        public Task<ZetaUTCSectionData> GetZetaUTCSectionDataAsync();

        /// <summary>
        /// Get Zeta UTC squad data from the given <paramref name="slot"/>
        /// </summary>
        /// <param name="slot">The <see cref="Slot"/> to get data for.</param>
        /// <returns>A task that returns <see cref="ZetaUTCSquadData"/> for the given <paramref name="slot"/></returns>
        public Task<ZetaUTCSquadData> GetZetaUTCSquadFromSlotAsync(Slot slot);

        /// <summary>
        /// Gets all new recruits, and their recruitment data, for the unit.
        /// </summary>
        /// <returns>A task that has a <see cref="List{T}"/> of <see cref="Trooper"/> values.</returns>
        public Task<List<Trooper>> GetNewRecruitsAsync();

        /// <summary>
        /// Gets the current UTC Cadets and their recruitment data.
        /// </summary>
        /// <returns>A task that has a <see cref="List{T}"/> of <see cref="Trooper"/> values.</returns>
        public Task<List<Trooper>> GetCurrentUTCCadets();
        /// <summary>
        /// Gets the current Acklay Adjutant
        /// </summary>
        /// <returns>A task the returns a <see cref="Trooper"/> if an adjutant is found, null if not.</returns>
        public Task<Trooper?> GetAcklayAdjutantAsync();
        #endregion

        #region Roster Registration
        /// <summary>
        /// Register a new trooper account to the website.
        /// </summary>
        /// <remarks>
        /// This method creates a new account to the database, and does not do inital checks to ensure the user ID is unique.
        /// Those checks need to be completed before this method is called, or an exception will be thrown.
        /// </remarks>
        /// <param name="trooperData">A <see cref="NewTrooperData"/> object that details the inital account information.</param>
        /// <param name="user">The <see cref="ClaimsPrincipal"/> for the user who initated this action.</param>
        /// <returns>A task that returns a <see cref="RegisterTrooperResult"/> for this action.</returns>
        public Task<RegisterTrooperResult> RegisterTrooper(NewTrooperData trooperData, ClaimsPrincipal user);
        #endregion

        #region Data Updates
        /// <summary>
        /// Get the C-Shop claim data for the provided trooper.
        /// </summary>
        /// <param name="trooper">A <see cref="Trooper"/> to get claim data from.</param>
        /// <returns>A <see cref="Task"/> that returns a <see cref="Dictionary{TKey, TValue}"/> of key <see cref="CShop"/> 
        /// and value <see cref="ClaimUpdateData"/> detailing hte claims for the <paramref name="trooper"/></returns>
        public Task<Dictionary<CShop, List<ClaimUpdateData>>> GetCShopClaimsAsync(Trooper trooper);

        /// <summary>
        /// Update a troopers 501st related data.
        /// </summary>
        /// <param name="edit">A <see cref="Trooper"/> object with edits made to it. Requires a valid ID to be set.</param>
        /// <param name="claimsToAdd">A <see cref="List{T}"/> of <see cref="ClaimUpdateData"/> that details claims to add to this <see cref="Trooper"/></param>
        /// <param name="claimsToRemove">A <see cref="List{T}"/> of <see cref="ClaimUpdateData"/> that details claims to remove from this <see cref="Trooper"/></param>
        /// <param name="submitter">The <see cref="ClaimsPrincipal"/> of the person who submitted this action.</param>
        /// <returns>A task that returns a <see cref="ResultBase"/>.</returns>
        public Task<ResultBase> UpdateAsync(Trooper edit, List<ClaimUpdateData> claimsToAdd,
            List<ClaimUpdateData> claimsToRemove, ClaimsPrincipal submitter);

        /// <summary>
        /// Saves a new flag to a Trooper.
        /// </summary>
        /// <param name="claim">The <see cref="ClaimsPrincipal"/> of the submitter.</param>
        /// <param name="trooper">The <see cref="Trooper"/> to add a flag to.</param>
        /// <param name="flag">The <see cref="TrooperFlag"/> to add to <paramref name="trooper"/></param>
        /// <returns>A task representing this action.</returns>
        public Task SaveNewFlag(ClaimsPrincipal claim, Trooper trooper, TrooperFlag flag);

        /// <summary>
        /// Saves a new description to a Trooper.
        /// </summary>
        /// <param name="claim">The <see cref="ClaimsPrincipal"/> of the submitter.</param>
        /// <param name="trooper">The <see cref="Trooper"/> to add a description to.</param>
        /// <param name="description">The <see cref="TrooperDescription"/> to add to <paramref name="trooper"/></param>
        /// <returns>A task that returns a <see cref="ResultBase"/>.</returns>
        public Task<ResultBase> SaveNewDescription(ClaimsPrincipal claim, Trooper trooper, TrooperDescription description);

        /// <summary>
        /// Moves a description item from one position to the position of another and re-do's the order.
        /// </summary>
        /// <param name="trooper">The <see cref="Trooper"/> to perform this action on.</param>
        /// <param name="desc">The <see cref="TrooperDescription"/> that you want to move.</param>
        /// <param name="index">The <see cref="int"/> that you want to move to.</param>
        /// <returns>A task that returns a <see cref="ResultBase"/>.</returns>
        public Task<ResultBase> UpdateDescriptionOrderAsync(Trooper trooper, TrooperDescription desc, int index);

        /// <summary>
        /// Delete a description from a Trooper.
        /// </summary>
        /// <param name="trooper">The <see cref="Trooper"/> to delete a description from.</param>
        /// <param name="desc">The <see cref="TrooperDescription"/> to delete.</param>
        /// <returns>A task that returns a <see cref="ResultBase"/>.</returns>
        public Task<ResultBase> DeleteDescriptionAsync(Trooper trooper, TrooperDescription desc);

        /// <summary>
        /// Update the list of allowed name changers.
        /// </summary>
        /// <param name="allowedTroopers">A new <see cref="List{T}"/> of <see cref="Trooper"/> that are allowed to change names.</param>
        /// <returns>A task that returns a <see cref="ResultBase"/>.</returns>
        public Task<ResultBase> UpdateAllowedNameChangersAsync(List<Trooper> allowedTroopers);

        /// <summary>
        /// Update the nickcname of a <see cref="Trooper"/>
        /// </summary>
        /// <param name="trooper">The <see cref="Trooper"/> to update the nickname for, with the changed nickname.</param>
        /// <param name="approver">The <see cref="int"/> ID of the submitter.</param>
        /// <returns>A task that returns a <see cref="ResultBase"/></returns>
        public Task<ResultBase> UpdateNickNameAsync(Trooper trooper, int approver);

        /// <summary>
        /// Reset an account of its personal information, but not 501st related data.
        /// </summary>
        /// <param name="trooper">The <see cref="Trooper"/> to reset.</param>
        /// <returns>A task that returns a <see cref="RegisterTrooperResult"/> that contains the 
        /// new access code for the provided <paramref name="trooper"/></returns>
        public Task<RegisterTrooperResult> ResetAccountAsync(Trooper trooper);

        #endregion

        #region Permissions
        /// <summary>
        /// Get allowed access values for the provided <paramref name="claims"/> in C1.
        /// </summary>
        /// <param name="claims">A <see cref="ClaimsPrincipal"/> of the current user.</param>
        /// <returns>A task that returns a <see cref="Array"/> of <see cref="bool"/> values.</returns>
        public Task<bool[]> GetC1PermissionsAsync(ClaimsPrincipal claims);

        /// <summary>
        /// Get allowed access values for the provided <paramref name="claims"/> in C3.
        /// </summary>
        /// <param name="claims">A <see cref="ClaimsPrincipal"/> of the current user.</param>
        /// <returns>A task that returns a <see cref="Array"/> of <see cref="bool"/> values.</returns>
        public Task<bool[]> GetC3PermissionsAsync(ClaimsPrincipal claims);

        /// <summary>
        /// Get allowed access values for the provided <paramref name="claims"/> in C4.
        /// </summary>
        /// <param name="claims">A <see cref="ClaimsPrincipal"/> of the current user.</param>
        /// <returns>A task that returns a <see cref="Array"/> of <see cref="bool"/> values.</returns>
        public Task<bool[]> GetC4PermissionsAsync(ClaimsPrincipal claims);

        /// <summary>
        /// Get allowed access values for the provided <paramref name="claims"/> in C5.
        /// </summary>
        /// <param name="claims">A <see cref="ClaimsPrincipal"/> of the current user.</param>
        /// <returns>A task that returns a <see cref="Array"/> of <see cref="bool"/> values.</returns>
        public Task<bool[]> GetC5PermissionsAsync(ClaimsPrincipal claims);

        /// <summary>
        /// Get allowed access values for the provided <paramref name="claims"/> in C6.
        /// </summary>
        /// <param name="claims">A <see cref="ClaimsPrincipal"/> of the current user.</param>
        /// <returns>A task that returns a <see cref="Array"/> of <see cref="bool"/> values.</returns>
        public Task<bool[]> GetC6PermissionsAsync(ClaimsPrincipal claims);

        /// <summary>
        /// Get allowed access values for the provided <paramref name="claims"/> in C7.
        /// </summary>
        /// <param name="claims">A <see cref="ClaimsPrincipal"/> of the current user.</param>
        /// <returns>A task that returns a <see cref="Array"/> of <see cref="bool"/> values.</returns>
        public Task<bool[]> GetC7PermissionsAsync(ClaimsPrincipal claims);

        /// <summary>
        /// Get allowed access values for the provided <paramref name="claims"/> in C8.
        /// </summary>
        /// <param name="claims">A <see cref="ClaimsPrincipal"/> of the current user.</param>
        /// <returns>A task that returns a <see cref="Array"/> of <see cref="bool"/> values.</returns>
        public Task<bool[]> GetC8PermissionsAsync(ClaimsPrincipal claims);

        /// <summary>
        /// Checks the premissions of an ID to determine its permission level.
        /// </summary>
        /// <param name="id">ID of a Trooper.</param>
        /// <returns>Two true or false values for if the user is an admin and 
        /// if they are a manager. Item1 is for Admin, Item2 is for Manager.</returns>
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
        /// <summary>
        /// Loads public profile data into the provided <paramref name="trooper"/>.
        /// </summary>
        /// <param name="trooper">The <see cref="Trooper"/> to load data for.</param>
        /// <returns>A task representing this action.</returns>
        public Task LoadPublicProfileDataAsync(Trooper trooper);

        /// <summary>
        /// Loads descriptions into the provided <paramref name="trooper"/>.
        /// </summary>
        /// <param name="trooper">The <see cref="Trooper"/> to load descriptions for.</param>
        /// <returns>A task representing this action.</returns>
        public Task LoadDescriptionsAsync(Trooper trooper);
        #endregion

        #region Account Management
        /// <summary>
        /// Update the account user name of a <paramref name="trooper"/>.
        /// </summary>
        /// <param name="trooper">The <see cref="Trooper"/> with a changed username to update.</param>
        /// <returns>A task that returns <see cref="ResultBase"/>.</returns>
        public Task<ResultBase> UpdateUserNameAsync(Trooper trooper);
        /// <summary>
        /// Update the birth number of a <paramref name="trooper"/>.
        /// </summary>
        /// <param name="trooper">The <see cref="Trooper"/> with a changed birth number to update.</param>
        /// <returns>A task that returns <see cref="ResultBase"/>.</returns>
        public Task<ResultBase> UpdateBirthNumberAsync(Trooper trooper);

        /// <summary>
        /// Deletes an account from the website.
        /// </summary>
        /// <param name="trooper">The <see cref="Trooper"/> to delete.</param>
        /// <param name="password">The password of the submitter defined in <paramref name="claims"/></param>
        /// <param name="claims">The <see cref="ClaimsPrincipal"/> of the submitter.</param>
        /// <returns>A task that returns a <see cref="ResultBase"/>.</returns>
        public Task<ResultBase> DeleteAccountAsync(Trooper trooper, string password, ClaimsPrincipal claims);

        /// <summary>
        /// Add a claim to the <paramref name="trooper"/>'s account.
        /// </summary>
        /// <param name="trooper">The <see cref="Trooper"/> to add a claim to.</param>
        /// <param name="claim">The <see cref="Claim"/> to add.</param>
        /// <param name="manager">The ID of the manager submitting this request.</param>
        /// <returns>A task that returns a <see cref="ResultBase"/>.</returns>
        public Task<ResultBase> AddClaimAsync(Trooper trooper, Claim claim, int manager);

        /// <summary>
        /// Remove a claim to the <paramref name="trooper"/>'s account.
        /// </summary>
        /// <param name="trooper">The <see cref="Trooper"/> to remove a claim from.</param>
        /// <param name="claim">The <see cref="Claim"/> to remove.</param>
        /// <param name="manager">The ID of the manager submitting this request.</param>
        /// <returns>A task that returns a <see cref="ResultBase"/>.</returns>
        public Task<ResultBase> RemoveClaimAsync(Trooper trooper, Claim claim, int manager);

        /// <summary>
        /// Get all claims the <paramref name="trooper"/> has on their account.
        /// </summary>
        /// <param name="trooper">The <see cref="Trooper"/> to get claims for.</param>
        /// <returns>A task that returns a <see cref="List{T}"/> of <see cref="Claim"/> values.</returns>
        public Task<List<Claim>> GetAllClaimsFromTrooperAsync(Trooper trooper);

        /// <summary>
        /// Ensures all archived troopers have the Archived role.
        /// </summary>
        /// <returns>A Task representing this action.</returns>
        public Task ValidateArchivedTroopersAsync();
        #endregion
    }
}
