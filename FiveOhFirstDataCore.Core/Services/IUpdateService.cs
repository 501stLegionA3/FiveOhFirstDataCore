using FiveOhFirstDataCore.Core.Account;
using FiveOhFirstDataCore.Core.Structures;
using FiveOhFirstDataCore.Core.Structures.Updates;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Services
{
    public interface IUpdateService
    {
        public Task<List<UpdateBase>> GetRosterUpdatesAsync();
        public Task<List<RecruitmentUpdate>> GetRecruitmentChangesAsync();
        public Task<List<SlotUpdate>> GetReturningMemberChangesAsync();
        public Task<List<UpdateBase>> GetAllUpdatesAsync();

        /// <summary>
        /// Reverts a change.
        /// </summary>
        /// <param name="update">The update details for the change to revert.</param>
        /// <returns>A <see cref="ResultBase"/> for if the operation succeeded.</returns>
        public Task<ResultBase> RevertUpdateAsync(Trooper manager, UpdateBase update);
    }
}
