using FiveOhFirstDataCore.Core.Account;
using FiveOhFirstDataCore.Core.Structures;
using FiveOhFirstDataCore.Core.Structures.Updates;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Services
{
    public interface IUpdateService
    {
        /// <summary>
        /// Get <see cref="UpdateBase"/> objcets related to roster updates.
        /// </summary>
        /// <returns>A <see cref="Task"/> that returns a <see cref="List{T}"/> of <see cref="UpdateBase"/> objects.</returns>
        public Task<List<UpdateBase>> GetRosterUpdatesAsync();
        /// <summary>
        /// Get <see cref="RecruitmentUpdate"/> objects.
        /// </summary>
        /// <returns>A <see cref="Task"/> that returns a <see cref="List{T}"/> of <see cref="RecruitmentUpdate"/> objects.</returns>
        public Task<List<RecruitmentUpdate>> GetRecruitmentChangesAsync();
        /// <summary>
        /// Get <see cref="SlotUpdate"/> objects.
        /// </summary>
        /// <returns>A <see cref="Task"/> that returns a <see cref="List{T}"/> of <see cref="SlotUpdate"/> objects.</returns>
        public Task<List<SlotUpdate>> GetReturningMemberChangesAsync();
        /// <summary>
        /// Get all updates in the database.
        /// </summary>
        /// <returns>A <see cref="Task"/> that returns a <see cref="List{T}"/> of <see cref="UpdateBase"/> objects.</returns>
        public Task<List<UpdateBase>> GetAllUpdatesAsync();

        /// <summary>
        /// Reverts a change.
        /// </summary>
        /// <param name="update">The update details for the change to revert.</param>
        /// <returns>A <see cref="ResultBase"/> for if the operation succeeded.</returns>
        public Task<ResultBase> RevertUpdateAsync(Trooper manager, UpdateBase update);
    }
}
