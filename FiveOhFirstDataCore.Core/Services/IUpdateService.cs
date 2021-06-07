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

        /// <summary>
        /// Reverts a change.
        /// </summary>
        /// <param name="update">The update details for the change to revert.</param>
        /// <returns>A boolean value for if the operation succeded.</returns>
        public Task<bool> RevertUpdateAsync(UpdateBase update);
    }
}
