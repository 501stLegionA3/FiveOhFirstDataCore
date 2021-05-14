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
        public Task<List<Trooper>> GetPlacedRosterAsync();
        public Task<List<Trooper>> GetActiveReservesAsync();
        public Task<List<Trooper>> GetInactiveReservesAsync();
        public Task<List<Trooper>> GetArchivedTroopersAsync();
        public Task<List<Trooper>> GetFullRosterAsync();
        public Task<List<Trooper>> GetUnregisteredTroopersAsync();
        public Task<OrbatData> GetOrbatDataAsync();
        public Task<ZetaOrbatData> GetZetaOrbatDataAsync();
        public Task<(HashSet<int>, HashSet<string>)> GetInUseUserDataAsync();
        #endregion

        #region Roster Registration
        public Task<RegisterTrooperResult> RegisterTrooper(NewTrooperData trooperData);
        #endregion

        #region Roster Updates
        public Task<Dictionary<CShop, List<ClaimUpdate>>> GetCShopClaimsAsync(Trooper trooper);
        public Task<ResultBase> UpdateAsync(Trooper edit, List<ClaimUpdate> claimsToAdd, List<ClaimUpdate> claimsToRemove, ClaimsPrincipal subitter);
        #endregion
    }
}
