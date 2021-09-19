using FiveOhFirstDataCore.Data.Account;
using FiveOhFirstDataCore.Data.Structures;

using System.Security.Claims;

namespace FiveOhFirstDataCore.Data.Services
{
    public interface IEparService
    {
        /// <summary>
        /// Create a new change request and save it to the database.
        /// </summary>
        /// <returns>A task for this action.</returns>
        public Task<ResultBase> CreateChangeRequest(TrooperChangeRequestData request, int submitterId);
        /// <summary>
        /// Get all active change requets.
        /// </summary>
        /// <returns>A task for this action that returns a <see cref="IReadOnlyList{T}"/> of type
        /// <see cref="TrooperChangeRequestData"/></returns>
        public Task<IReadOnlyList<TrooperChangeRequestData>> GetActiveChangeRequests();
        /// <summary>
        /// Get recent change requests within this range.
        /// </summary>
        /// <param name="start">Starting <see cref="int"/>, where 0 is the first item submitted.</param>
        /// <param name="end">Ending <see cref="int"/>, where 0 is the first item submitted.</param>
        /// <returns>A task for this action that returns a <see cref="IReadOnlyList{T}"/> of type
        /// <see cref="TrooperChangeRequestData"/></returns>
        public Task<IReadOnlyList<TrooperChangeRequestData>> GetActiveChangeRequests(int start, int end, object[] args);
        /// <summary>
        /// Gets the count of active change requests in the system.
        /// </summary>
        /// <param name="args">Optional Arguments</param>
        /// <returns>A task that returns a <see cref="int"/> value for the ammount of active change requests.</returns>
        public Task<int> GetActiveChangeRequestCount(object[] args);
        /// <summary>
        /// Gets a specified change request.
        /// </summary>
        /// <param name="id">The <see cref="Guid"/> of the request to retreive.</param>
        /// <returns>A task that returns a <see cref="TrooperChangeRequestData"/> or <see cref="null"/> 
        /// if no item was found for the <paramref name="id"/></returns>
        public Task<TrooperChangeRequestData?> GetChangeRequestAsync(Guid id);

        /// <summary>
        /// Finalizes a change request.
        /// </summary>
        /// <param name="requestId">The <see cref="Guid"/> of the request to finalize.</param>
        /// <param name="approve">A <see cref="bool"/> value that determins if this was approved or not.</param>
        /// <param name="finalizer">The <see cref="int"/> ID of the person who finalized this request.</param>
        /// <param name="finalizerClaim">The <see cref="ClaimsPrincipal"/> for the <paramref name="finalizer"/></param>
        /// <returns>A task that returns a <see cref="ResultBase"/> for this action.</returns>
        public Task<ResultBase> FinalizeChangeRequest(Guid requestId, bool approve, int finalizer, ClaimsPrincipal finalizerClaim);
    }
}
