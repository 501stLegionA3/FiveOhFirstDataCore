using FiveOhFirstDataCore.Core.Account.Detail;
using FiveOhFirstDataCore.Core.Structures;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Services
{
    public class ReportService : IReportService
    {
        public Task<ResultBase> CreateReportAsync(int submitter, TrooperReport report)
        {
            throw new NotImplementedException();
        }

        public Task<List<TrooperReport>> GetTrooperReportsAsync(int start, int end, object[] args)
        {
            throw new NotImplementedException();
        }
    }
}
