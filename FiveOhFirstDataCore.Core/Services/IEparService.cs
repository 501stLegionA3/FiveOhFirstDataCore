using FiveOhFirstDataCore.Core.Account;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Services
{
    public interface IEparService
    {
        /// <summary>
        /// Create a new change request and save it to the database.
        /// </summary>
        /// <returns>A task for this action.</returns>
        public Task CreateChangeRequest();
        /// <summary>
        /// Get all active change requets.
        /// </summary>
        /// <returns>A task for this action that returns a <see cref="IReadOnlyList{T}"/> of type
        /// <see cref="TrooperChangeRequestData"/></returns>
        public Task<IReadOnlyList<TrooperChangeRequestData>> GetActiveChangeRequests();
    }
}
