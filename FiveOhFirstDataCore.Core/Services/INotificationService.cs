namespace FiveOhFirstDataCore.Data.Services
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
        /// <summary>
        /// Update the last viewed date time for report notification trackers.
        /// </summary>
        /// <param name="report">The report to find trackers in.</param>
        /// <param name="user">The user to update notification trackers for.</param>
        /// <returns>A task for this action.</returns>
        public Task UpdateReportViewDateTimeAsync(Guid report, int user);
        /// <summary>
        /// Update the last viewed date time for report notification trackers.
        /// </summary>
        /// <param name="report">The report to find trackers in.</param>
        /// <param name="user">The user to update notification trackers for.</param>
        /// <param name="view">The date time to set the last view time to.</param>
        /// <returns>A task for this action.</returns>
        public Task UpdateReportViewDateTimeAsync(Guid report, int user, DateTime view);
        /// <summary>
        /// Toggles a report listener for a report.
        /// </summary>
        /// <param name="report">The report record notification data for.</param>
        /// <param name="user">The user to toggle the notification tracker for.</param>
        /// <returns>A task for this action that returns a <see cref="bool"/> that is true
        /// if the notification is being tracked.</returns>
        public Task<bool> ToggleReportNotificationTracker(Guid report, int user);
        /// <summary>
        /// Gets if the trooper is currently notified about a report.
        /// </summary>
        /// <param name="report">Report to check.</param>
        /// <param name="user">User to check.</param>
        /// <returns>A task that returns a <see cref="bool"/> that is true if the 
        /// <see cref="Account.Trooper"/> is 
        /// notified about the <see cref="Account.Detail.TrooperReport"/></returns>
        public Task<bool> IsTrooperNotifiedForReportAsync(Guid report, int user);
    }
}
