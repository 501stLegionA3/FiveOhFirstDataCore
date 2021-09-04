using FiveOhFirstDataCore.Core.Database;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Services
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
                .AsAsyncEnumerable();

            var notif = new ConcurrentDictionary<Guid, int>();

            await foreach (var item in data)
            {
                if(item.Report.LastUpdate > item.LastView)
                {
                    for (int i = item.Report.Responses.Count - 1;  i >= 0; i--)
                    {
                        if(item.Report.Responses[i].CreatedOn > item.LastView)
                        {
                            notif.AddOrUpdate(item.ReportId, 0, (x, y) => y + 1);
                        }
                    }
                }
            }

            return notif;
        }

        public async Task UpdateReportViewDateTimeAsync(Guid report, int user)
        {
            var view = DateTime.UtcNow;
            await using var _dbContext = _dbContextFactory.CreateDbContext();
            await _dbContext.ReportNotificationTrackers
                .Where(x => x.NotificationForId == user)
                .Where(x => x.ReportId == report)
                .ForEachAsync(x => x.LastView = view);

            await _dbContext.SaveChangesAsync();
        }
    }
}
