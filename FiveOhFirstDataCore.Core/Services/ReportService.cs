using FiveOhFirstDataCore.Data.Account;
using FiveOhFirstDataCore.Data.Account.Detail;
using FiveOhFirstDataCore.Data.Structures;
using FiveOhFirstDataCore.Data.Structuresbase;
using FiveOhFirstDataCore.Data.Extensions;
using FiveOhFirstDataCore.Data.Structures;
using FiveOhFirstDataCore.Data.Structures.Notification;

using Microsoft.EntityFrameworkCore;

namespace FiveOhFirstDataCore.Data.Services
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
            report.SubmittedOn = report.LastUpdate;

            // Get the company for this report.
            report.ReportViewableAt = (Slot)((int)actual.Slot / 100 * 100);

            // Modify some special cases.
            switch (report.ReportViewableAt)
            {
                case Slot.AcklayCompany:
                    report.ReportViewableAt = Slot.AcklayOne;
                    break;
                case Slot.InactiveReserve:
                    report.ReportViewableAt = Slot.Hailstorm;
                    break;
            }

            switch (actual.Slot)
            {
                case Slot.AcklayReserve:
                    report.ReportViewableAt = Slot.AcklayOne;
                    break;
                case Slot.RazorReserve:
                    report.ReportViewableAt = Slot.Razor;
                    break;
                case Slot.MynockReserve:
                    report.ReportViewableAt = Slot.Mynock;
                    break;
            }

            // Move company reports to battalion.
            if (actual.Slot == report.ReportViewableAt)
                report.ReportViewableAt = Slot.Hailstorm;

            // Send inactive reserves and other like reports up to hailstorm.
            if (report.ReportViewableAt >= Slot.InactiveReserve)
                report.ReportViewableAt = Slot.Hailstorm;

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
                .ThenByDescending(x => x.LastUpdate)
                .Include(x => x.ReportedBy)
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

        public async Task<IReadOnlyList<TrooperReport>> GetPersonalReportsAsync(int start, int end, object[] args)
        {
            var id = GetPersonalArgs(args);

            if (id == 0) return Array.Empty<TrooperReport>();

            await using var _dbContext = _dbContextFactory.CreateDbContext();
            return _dbContext.Reports
                .Where(x => x.ReportedById == id)
                .OrderBy(x => x.Resolved)
                .ThenByDescending(x => x.LastUpdate)
                .Include(x => x.ReportedBy)
                .AsEnumerable()
                .Take(new Range(start, end))
                .ToList();
        }

        public async Task<int> GetPersonalReportCountsAsync(object[] args)
        {
            var id = GetPersonalArgs(args);

            if (id == 0) return 0;

            await using var _dbContext = _dbContextFactory.CreateDbContext();
            return await _dbContext.Reports
                .Where(x => x.ReportedById == id)
                .CountAsync();
        }

        private static int GetPersonalArgs(object[] args)
        {
            int id = 0;
            if (args.Length > 0)
            {
                try
                {
                    id = Convert.ToInt32(args[0]);
                }
                catch {  /* we already have a default value */ }
            }

            return id;
        }

        public async Task<IReadOnlyList<TrooperReport>> GetNotifyingReportsAsync(int start, int end, object[] args)
        {
            var id = GetPersonalArgs(args);

            if (id == 0) return Array.Empty<TrooperReport>();

            await using var _dbContext = _dbContextFactory.CreateDbContext();
            var actual = await _dbContext.FindAsync<Trooper>(id);

            if (actual is null) return Array.Empty<TrooperReport>();

            await _dbContext.Entry(actual).Collection(x => x.TrooperReportTrackers).LoadAsync();
            var participation = actual.TrooperReportTrackers.ToList(x => x.ReportId);
            return _dbContext.Reports
                .Where(x => participation.Contains(x.Id))
                .OrderBy(x => x.Resolved)
                .ThenByDescending(x => x.LastUpdate)
                .Include(x => x.ReportedBy)
                .AsEnumerable()
                .Take(new Range(start, end))
                .ToList();
        }

        public async Task<int> GetNotifyingReportCountsAsync(object[] args)
        {
            var id = GetPersonalArgs(args);

            if (id == 0) return 0;

            await using var _dbContext = _dbContextFactory.CreateDbContext();
            var actual = await _dbContext.FindAsync<Trooper>(id);

            if (actual is null) return 0;

            await _dbContext.Entry(actual).Collection(x => x.TrooperReportTrackers).LoadAsync();
            var participation = actual.TrooperReportTrackers.ToList(x => x.ReportId);
            return await _dbContext.Reports
                .Where(x => participation.Contains(x.Id))
                .CountAsync();
        }

        public async Task<IReadOnlyList<TrooperReport>> GetParticipatingReportsAsync(int start, int end, object[] args)
        {
            var id = args.GetArgument<int>(0);

            if (id == default) return Array.Empty<TrooperReport>();

            await using var _dbContext = _dbContextFactory.CreateDbContext();
            var participation = (await _dbContext.TrooperMessages
                .Where(x => x.AuthorId == id)
                .ToListAsync()).ToList(x => x.MessageFor);
            return _dbContext.Reports
                .Where(x => participation.Contains(x.Id))
                .OrderBy(x => x.Resolved)
                .ThenByDescending(x => x.LastUpdate)
                .Include(x => x.ReportedBy)
                .AsEnumerable()
                .Take(new Range(start, end))
                .ToList();
        }

        public async Task<int> GetParticipatingReportCountsAsync(object[] args)
        {
            var id = args.GetArgument<int>(0);

            if (id == default) return 0;

            await using var _dbContext = _dbContextFactory.CreateDbContext();
            var participation = (await _dbContext.TrooperMessages
                .Where(x => x.AuthorId == id)
                .ToListAsync()).ToList(x => x.MessageFor);

            return await _dbContext.Reports
                .Where(x => participation.Contains(x.Id))
                .CountAsync();
        }

        public async Task<TrooperReport?> GetTrooperReportIfAuthorized(Guid report, int viewer, bool manager = false)
        {
            await using var _dbContext = _dbContextFactory.CreateDbContext();
            var actual = await _dbContext.FindAsync<TrooperReport>(report);
            var viewerActual = await _dbContext.FindAsync<Trooper>(viewer);
            var viewerClaims = await _dbContext.UserClaims
                .Where(x => x.UserId == viewer)
                .ToListAsync();

            if (actual is null || viewerActual is null) return null;
            // If they are a maanager, pass the report up.
            if (manager) return actual;
            // It's their report.
            if (actual.ReportedById == viewer) return actual;
            // If they are MP, pass the report up.
            if (viewerActual.MilitaryPolice)
                return actual;
            // if they are battalion, and this report is at battalion,
            // pass the report up.
            if (actual.ElevatedToBattalion && viewerActual.Slot == Slot.Hailstorm)
                return actual;
            // If they are in the slot that is viewable by this report,
            // pass the report up.
            if (actual.ReportViewableAt == viewerActual.Slot)
                return actual;
            // If they have a claim to view this report level,
            // pass the report up.
            if (viewerClaims.Where(x => x.ClaimType == "Trooper.Report")
                .Any(x => x.ClaimValue == ((int)actual.ReportViewableAt).ToString()))
                return actual;
            // Otherwise return null.
            return null;
        }

        public async Task UpdateReportLastUpdateAsync(Guid report)
        {
            await using var _dbContext = _dbContextFactory.CreateDbContext();
            var actual = await _dbContext.FindAsync<TrooperReport>(report);
            if (actual is not null)
            {
                actual.LastUpdate = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<TrooperReport?> ToggleResolvedAsync(Guid report)
        {
            await using var _dbContext = _dbContextFactory.CreateDbContext();
            var actual = await _dbContext.FindAsync<TrooperReport>(report);
            if (actual is not null)
            {
                actual.Resolved = !actual.Resolved;
                await _dbContext.SaveChangesAsync();
                return actual;
            }

            return null;
        }

        public async Task<TrooperReport?> ElevateToBattalionAsync(Guid report)
        {
            await using var _dbContext = _dbContextFactory.CreateDbContext();
            var actual = await _dbContext.FindAsync<TrooperReport>(report);
            if (actual is not null)
            {
                actual.ElevatedToBattalion = true;
                await _dbContext.SaveChangesAsync();
                return actual;
            }

            return null;
        }
    }
}
