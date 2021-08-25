using FiveOhFirstDataCore.Core.Account.Detail;
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
        public Task<ResultBase> CreateReportAsync(int submitter, TrooperReport report);
        public Task<List<TrooperReport>> GetTrooperReportsAsync(int start, int end, object[] args);
    }
}
