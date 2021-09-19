using FiveOhFirstDataCore.Data.Account;
using FiveOhFirstDataCore.Data.Structures;
using FiveOhFirstDataCore.Data.Structures.Updates;

namespace FiveOhFirstDataCore.Data.Services
{
    public interface IUpdateService
    {
        /// <summary>
        /// Get <see cref="UpdateBase"/> objcets related to roster updates.
        /// </summary>
        /// <param name="range">A range of items to take from the database. Leave as null to take all values.</param>
        /// <returns>A <see cref="Task"/> that returns a <see cref="IReadOnlyList{T}"/> of <see cref="UpdateBase"/> objects.</returns>
        public IReadOnlyList<UpdateBase> GetRosterUpdates(Range? range = null);
        /// <summary>
        /// Get <see cref="UpdateBase"/> objcets related to roster updates.
        /// </summary>
        /// <param name="start">The start index, where 0 is the latest update.</param>
        /// <param name="end">The end index.</param>
        /// <param name="args">Optional arguments.</param>
        /// <returns>A <see cref="Task"/> that returns a <see cref="List{T}"/> of <see cref="UpdateBase"/> objects.</returns>
        public IReadOnlyList<UpdateBase> GetRosterUpdates(int start, int end, object[] args);
        /// <summary>
        /// Gets the ammount of roster updates saved in the database.
        /// </summary>
        /// <param name="args">Optional arguments</param>
        /// <returns>The ammount of roster updates.</returns>
        public Task<int> GetRosterUpdatesCountAsync(object[] args);

        /// <summary>
        /// Get <see cref="RecruitmentUpdate"/> objects.
        /// </summary>
        /// <param name="range">A range of items to take from the database. Leave as null to take all values.</param>
        /// <returns>A <see cref="Task"/> that returns a <see cref="IReadOnlyList{T}"/> of <see cref="RecruitmentUpdate"/> objects.</returns>
        public Task<IReadOnlyList<RecruitmentUpdate>> GetRecruitmentChangesAsync(Range? range = null);
        /// <summary>
        /// Get <see cref="SlotUpdate"/> objects.
        /// </summary>
        /// <param name="start">The start index, where 0 is the latest update.</param>
        /// <param name="end">The end index.</param>
        /// <param name="args">optional arguments</param>
        /// <returns>A <see cref="Task"/> that returns a <see cref="List{T}"/> of <see cref="SlotUpdate"/> objects.</returns>
        public Task<IReadOnlyList<RecruitmentUpdate>> GetRecruitmentChangesAsync(int start, int end, object[] args);
        /// <summary>
        /// Get the ammount of recruitment changes saved in the database.
        /// </summary>
        /// <param name="args">Optional arguments</param>
        /// <returns>The ammount of recruitment changes.</returns>
        public Task<int> GetRecruitmentChangesCountAsync(object[] args);

        /// <summary>
        /// Get <see cref="SlotUpdate"/> objects for returning members.
        /// </summary>
        /// <param name="range">A range of items to take from the database. Leave as null to take all values.</param>
        /// <returns>A <see cref="Task"/> that returns a <see cref="IReadOnlyList{T}"/> of <see cref="SlotUpdate"/> objects.</returns>
        public Task<IReadOnlyList<SlotUpdate>> GetReturningMemberChangesAsync(Range? range = null);
        /// <summary>
        /// Get <see cref="SlotUpdate"/> objects for returning members.
        /// </summary>
        /// <param name="start">The start index, where 0 is the latest update.</param>
        /// <param name="end">The end index.</param>
        /// <param name="args">Optional arguments.</param>
        /// <returns>A <see cref="Task"/> that returns a <see cref="List{T}"/> of <see cref="SlotUpdate"/> objects.</returns>
        public Task<IReadOnlyList<SlotUpdate>> GetReturningMemberChangesAsync(int start, int end, object[] args);
        /// <summary>
        /// Get the ammount of returning member changes in the database.
        /// </summary>
        /// <param name="args">Optional arguments</param>
        /// <returns>The ammount of returning member changes.</returns>
        public Task<int> GetReturningMemberChangesCountAsync(object[] args);

        /// <summary>
        /// Get all updates in the database.
        /// </summary>
        /// <param name="range">A range of items to take from the database. Leave as null to take all values.</param>
        /// <returns>A <see cref="Task"/> that returns a <see cref="IReadOnlyList{T}"/> of <see cref="UpdateBase"/> objects.</returns>
        public IReadOnlyList<UpdateBase> GetAllUpdates(Range? range = null);
        /// <summary>
        /// Get all updates in the database.
        /// </summary>
        /// <param name="start">The start index, where 0 is the latest update.</param>
        /// <param name="end">The end index.</param>
        /// <param name="args">Optional arguments.</param>
        /// <returns>A <see cref="Task"/> that returns a <see cref="List{T}"/> of <see cref="UpdateBase"/> objects.</returns>
        public IReadOnlyList<UpdateBase> GetAllUpdates(int start, int end, object[] args);
        /// <summary>
        /// Get the ammount of updates in the database.
        /// </summary>
        /// <param name="args">Optional arguments</param>
        /// <returns>The ammount of updates.</returns>
        public Task<int> GetAllUpdatesCountAsync(object[] args);

        /// <summary>
        /// Reverts a change.
        /// </summary>
        /// <param name="update">The update details for the change to revert.</param>
        /// <returns>A <see cref="ResultBase"/> for if the operation succeeded.</returns>
        public Task<ResultBase> RevertUpdateAsync(Trooper manager, UpdateBase update);
    }
}
