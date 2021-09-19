using FiveOhFirstDataCore.Data.Account;
using FiveOhFirstDataCore.Data.Account.Detail;
using FiveOhFirstDataCore.Data.Structures;
using FiveOhFirstDataCore.Data.Structures;

namespace FiveOhFirstDataCore.Data.Services
{
    public interface IReportService
    {
        /// <summary>
        /// Create a new report.
        /// </summary>
        /// <param name="submitter">THe ID of the <see cref="Trooper"/> who submitted the report.</param>
        /// <param name="report">The report object.</param>
        /// <returns>A Task that returns <see cref="ResultBase"/></returns>
        public Task<ResultBase> CreateReportAsync(int submitter, TrooperReport report);
        /// <summary>
        /// Get all trooper reports in a range.
        /// </summary>
        /// <param name="start">The start of the range.</param>
        /// <param name="end">The end of the range.</param>
        /// <param name="args">Optional args. Expectes 0 or 1 items. Item should be a <see cref="Slot"/> to filter results by.</param>
        /// <returns>A task that returns an <see cref="IReadOnlyList{T}"/> of type <see cref="TrooperReport"/>.</returns>
        public Task<IReadOnlyList<TrooperReport>> GetTrooperReportsAsync(int start, int end, object[] args);
        /// <summary>
        /// Gets the ammount of active trooper reports.
        /// </summary>
        /// <param name="args">Optional args. Expectes 0 or 1 items. Item should be a <see cref="Slot"/> to filter results by.</param>
        /// <returns>A task the returns an <see cref="int"/> that indicates the ammount of reports that are active.</returns>
        public Task<int> GetTrooperReportCountsAsync(object[] args);

        public Task<IReadOnlyList<TrooperReport>> GetPersonalReportsAsync(int start, int end, object[] args);
        public Task<int> GetPersonalReportCountsAsync(object[] args);

        public Task<IReadOnlyList<TrooperReport>> GetNotifyingReportsAsync(int start, int end, object[] args);
        public Task<int> GetNotifyingReportCountsAsync(object[] args);

        public Task<IReadOnlyList<TrooperReport>> GetParticipatingReportsAsync(int start, int end, object[] args);
        public Task<int> GetParticipatingReportCountsAsync(object[] args);

        /// <summary>
        /// Get a trooper report for a user to view if they have permission to do so.
        /// </summary>
        /// <param name="report">The <see cref="Guid"/> of the report to view.</param>
        /// <param name="viewer">The <see cref="int"/> ID of the trooper wanting to view the report.</param>
        /// <param name="manager"></param>
        /// <returns>the <see cref="TrooperReport"/> for the provided <paramref name="report"/> ID
        /// if the viewer has authorization to view the report.</returns>
        public Task<TrooperReport?> GetTrooperReportIfAuthorized(Guid report, int viewer, bool manager = false);
        /// <summary>
        /// Sets the last update for a report to <see cref="DateTime"/>.UtcNow
        /// </summary>
        /// <param name="report">The report to update.</param>
        /// <returns>A task for this action.</returns>
        public Task UpdateReportLastUpdateAsync(Guid report);
        /// <summary>
        /// Toggles the resolved state of a report.
        /// </summary>
        /// <param name="report">The report to toggle.</param>
        /// <returns>A task the retruns the report object after modification.</returns>
        public Task<TrooperReport?> ToggleResolvedAsync(Guid report);
        /// <summary>
        /// Elevates a trooper report to battalion.
        /// </summary>
        /// <param name="report">The report to elevate.</param>
        /// <returns>A task that returns the report object after modification.</returns>
        public Task<TrooperReport?> ElevateToBattalionAsync(Guid report);
    }
}
