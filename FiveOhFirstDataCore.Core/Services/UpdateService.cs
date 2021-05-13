using FiveOhFirstDataCore.Core.Database;
using FiveOhFirstDataCore.Core.Structures.Updates;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Services
{
    public class UpdateService : IUpdateService
    {
        private readonly ApplicationDbContext _dbContext;

        public UpdateService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<UpdateBase>> GetRosterUpdatesAsync()
        {
            ConcurrentBag<UpdateBase> data = new();
            List<Task> actions = new();

            await _dbContext
                .RankChanges
                .AsNoTracking()
                .ForEachAsync(x =>
                {
                    if(x.SubmittedByRosterClerk)
                        data.Add(x);
                });

            await _dbContext
                .CShopChanges
                .AsNoTracking()
                .ForEachAsync(x =>
                {
                    if(x.SubmittedByRosterClerk)
                        data.Add(x);
                });

            await _dbContext
                .QualificationChanges
                .AsNoTracking()
                .ForEachAsync(x =>
                {
                    if(x.SubmittedByRosterClerk)
                        data.Add(x);
                });

            await _dbContext
                .SlotChanges
                .AsNoTracking()
                .ForEachAsync(x =>
                {
                    if(x.SubmittedByRosterClerk)
                        data.Add(x);
                });

            return data.ToList();
        }
    }
}
