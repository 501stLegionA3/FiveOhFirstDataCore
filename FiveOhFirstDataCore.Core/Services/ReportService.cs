using FiveOhFirstDataCore.Core.Account;
using FiveOhFirstDataCore.Core.Account.Detail;
using FiveOhFirstDataCore.Core.Database;
using FiveOhFirstDataCore.Core.Structures;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Services
{
    public class ReportService : IReportService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

        public ReportService(IDbContextFactory<ApplicationDbContext> dbContextFactory)
            => (_dbContextFactory) = (dbContextFactory);

        public async Task<ResultBase> CreateReportAsync(int submitter, TrooperReport report)
        {
            await using var _dbContext = _dbContextFactory.CreateDbContext();
            var actual = await _dbContext.FindAsync<Trooper>(submitter);
            if (actual is null)
                return new(false, new List<string>() { $"Unable to find a trooper for {submitter}" });

            report.LastUpdate = DateTime.UtcNow;

            actual.FiledReports.Add(report);

            await _dbContext.SaveChangesAsync();

            return new(true, null);
        }

        public Task<List<TrooperReport>> GetTrooperReportsAsync(int start, int end, object[] args)
        {
            throw new NotImplementedException();
        }
    }
}
