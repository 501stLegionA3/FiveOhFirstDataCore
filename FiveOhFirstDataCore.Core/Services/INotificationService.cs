using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Services
{
    public interface INotificationService
    {
        /// <summary>
        /// Get the notification counts for reports.
        /// </summary>
        /// <param name="notificationFor">The Trooper to get counts for.</param>
        /// <param name="idFilter">The GUID values of reports to check notifications for.</param>
        /// <returns>A task that returns a read only dictionary of GUID and thier respective notification counts
        /// but only for notifications with counts above 0.</returns>
        public Task<IReadOnlyDictionary<Guid, int>> GetReportNotificationCountsAsync(int notificationFor, Guid[]? idFilter = null);
    }
}
