using FiveOhFirstDataCore.Core.Account;
using FiveOhFirstDataCore.Core.Account.Detail;
using FiveOhFirstDataCore.Core.Data;
using FiveOhFirstDataCore.Core.Structures;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Services
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

        public Task<IReadOnlyList<TrooperReport>> GetAllReportsAsync(int start, int end, object[] args);
        public Task<int> GetAllReportCountsAsync(object[] args);
    }
}
