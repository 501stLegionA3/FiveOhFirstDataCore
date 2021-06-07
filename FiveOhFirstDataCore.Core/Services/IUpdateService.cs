using FiveOhFirstDataCore.Core.Account;
using FiveOhFirstDataCore.Core.Structures;
using FiveOhFirstDataCore.Core.Structures.Updates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Services
{
    public interface IUpdateService
    {
        public Task<IEnumerable<UpdateBase>> GetRosterUpdatesAsync();
        public Task<IEnumerable<RecruitmentUpdate>> GetRecruitmentChangesAsync();
        public Task<IEnumerable<SlotUpdate>> GetReturningMemberChangesAsync();
        public Task<IEnumerable<UpdateBase>> GetAllUpdatesAsync();

        /// <summary>
        /// Reverts a change.
        /// </summary>
        /// <param name="update">The update details for the change to revert.</param>
        /// <returns>A <see cref="ResultBase"/> for if the operation succeded.</returns>
        public Task<ResultBase> RevertUpdateAsync(Trooper manager, UpdateBase update);
    }
}
