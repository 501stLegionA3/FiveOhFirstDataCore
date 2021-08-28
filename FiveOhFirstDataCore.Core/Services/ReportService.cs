using FiveOhFirstDataCore.Core.Account;
using FiveOhFirstDataCore.Core.Account.Detail;
using FiveOhFirstDataCore.Core.Data;
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

        public async Task<IReadOnlyList<TrooperReport>> GetTrooperReportsAsync(int start, int end, object[] args)
        {
            Slot? filter = null;
            if (args.Length > 0)
            {
                try
                {
                    filter = (Slot)args[0];
                }
                catch { /* we have already set filter to null */ }
            }

            await using var _dbContext = _dbContextFactory.CreateDbContext();
            var query = _dbContext.Reports
                .Where(x => !x.Resolved);

            if (filter is not null)
                query = query
                    .Where(x => x.ReportViewableAt == filter || x.ElevatedToBattalion && filter == Slot.Hailstorm);

            return query.AsEnumerable()
                .Take(new Range(start, end))
                .ToList();
        }
    }
}
