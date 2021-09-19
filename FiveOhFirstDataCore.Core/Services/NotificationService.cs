using FiveOhFirstDataCore.Data.Structuresbase;

using Microsoft.EntityFrameworkCore;

using System.Collections.Concurrent;

namespace FiveOhFirstDataCore.Data.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

        public NotificationService(IDbContextFactory<ApplicationDbContext> dbContextFactory)
            => (_dbContextFactory) = dbContextFactory;

        public async Task<IReadOnlyDictionary<Guid, int>> GetReportNotificationCountsAsync(int notificationFor, Guid[]? idFilter = null)
        {
            await using var _dbContext = _dbContextFactory.CreateDbContext();
            var query = _dbContext.ReportNotificationTrackers
                .Where(x => x.NotificationForId == notificationFor);
            if (idFilter is not null)
                query.Where(x => idFilter.Contains(x.Key));

            var data = query
                .Include(e => e.Report)
                .ThenInclude(e => e.Responses)
                .AsAsyncEnumerable();

            var notif = new ConcurrentDictionary<Guid, int>();

            await foreach (var item in data)
            {
                for (int i = item.Report.Responses.Count - 1; i >= 0; i--)
                {
                    if (item.Report.Responses[i].CreatedOn > item.LastView)
                    {
                        notif.AddOrUpdate(item.ReportId, 1, (x, y) => y + 1);
                    }
                }
            }

            return notif;
        }

        public async Task<bool> IsTrooperNotifiedForReportAsync(Guid report, int user)
        {
            await using var _dbContext = _dbContextFactory.CreateDbContext();
            var notices = await _dbContext.ReportNotificationTrackers
                .Where(x => x.NotificationForId == user)
                .Where(x => x.ReportId == report)
                .ToListAsync();

            return notices.Count > 0;
        }

        public async Task<bool> ToggleReportNotificationTracker(Guid report, int user)
        {
            await using var _dbContext = _dbContextFactory.CreateDbContext();
            var notice = await _dbContext.ReportNotificationTrackers
                .Where(x => x.NotificationForId == user)
                .Where(x => x.ReportId == report)
                .ToListAsync();

            bool tracking;
            if (notice.Count <= 0)
            {
                await _dbContext.ReportNotificationTrackers.AddAsync(new()
                {
                    NotificationForId = user,
                    ReportId = report,
                    LastView = DateTime.UtcNow,
                });
                tracking = true;
            }
            else
            {
                foreach (var i in notice)
                    _dbContext.Remove(i);
                tracking = false;
            }

            await _dbContext.SaveChangesAsync();
            return tracking;
        }

        public async Task UpdateReportViewDateTimeAsync(Guid report, int user)
            => await UpdateReportViewDateTimeAsync(report, user, DateTime.UtcNow);

        public async Task UpdateReportViewDateTimeAsync(Guid report, int user, DateTime view)
        {
            await using var _dbContext = _dbContextFactory.CreateDbContext();
            await _dbContext.ReportNotificationTrackers
                .Where(x => x.NotificationForId == user)
                .Where(x => x.ReportId == report)
                .ForEachAsync(x => x.LastView = view);

            await _dbContext.SaveChangesAsync();
        }
    }
}
