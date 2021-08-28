using FiveOhFirstDataCore.Core.Account;
using FiveOhFirstDataCore.Core.Account.Detail;
using FiveOhFirstDataCore.Core.Data;
using FiveOhFirstDataCore.Core.Database;
using FiveOhFirstDataCore.Core.Structures;
using FiveOhFirstDataCore.Core.Structures.Notification;

using Lucene.Net.Search;

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

            var tracker = new ReportNotificationTracker()
            {
                LastView = report.LastUpdate
            };

            report.NotificationTrackers.Add(tracker);
            actual.TrooperReportTrackers.Add(tracker);

            await _dbContext.SaveChangesAsync();

            return new(true, null);
        }

        public async Task<int> GetTrooperReportCountsAsync(object[] args)
        {
            Slot? filter = GetReportArgs(args);

            await using var _dbContext = _dbContextFactory.CreateDbContext();
            var query = _dbContext.Reports
                .Where(x => !x.Resolved);

            if (filter is not null)
                query = query
                    .Where(x => x.ReportViewableAt == filter || x.ElevatedToBattalion && filter == Slot.Hailstorm);

            return await query.CountAsync();
        }

        public async Task<IReadOnlyList<TrooperReport>> GetTrooperReportsAsync(int start, int end, object[] args)
        {
            Slot? filter = GetReportArgs(args);

            await using var _dbContext = _dbContextFactory.CreateDbContext();
            var query = _dbContext.Reports.AsQueryable();

            if (filter is not null)
                query = query
                    .Where(x => x.ReportViewableAt == filter || x.ElevatedToBattalion && filter == Slot.Hailstorm);

            return query
                .OrderBy(x => x.Resolved)
                .OrderBy(x => x.LastUpdate)
                .AsEnumerable()
                .Take(new Range(start, end))
                .ToList();
        }

        private static Slot? GetReportArgs(object[] args)
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

            return filter;
        }

        public Task<IReadOnlyList<TrooperReport>> GetPersonalReportsAsync(int start, int end, object[] args)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetPersonalReportCountsAsync(object[] args)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<TrooperReport>> GetAllReportsAsync(int start, int end, object[] args)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetAllReportCountsAsync(object[] args)
        {
            throw new NotImplementedException();
        }
    }
}
