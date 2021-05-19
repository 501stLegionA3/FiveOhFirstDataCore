using FiveOhFirstDataCore.Core.Database;
using FiveOhFirstDataCore.Core.Structures.Updates;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Math.EC.Rfc7748;
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

        public Task<IEnumerable<RecruitmentChange>> GetRecruitmentChangesAsync()
        {
            return Task.FromResult(_dbContext
                .RecruitmentChanges
                .Include(p => p.ChangedFor)
                .Include(p => p.RecruitedBy)
                .AsSplitQuery()
                .OrderByDescending(x => x.ChangedOn)
                .AsEnumerable());
        }

        public Task<IEnumerable<SlotChange>> GetReturningMemberChangesAsync()
        {
            return Task.FromResult(_dbContext
                .SlotChanges
                .Where(x => x.OldSlot == Data.Slot.Archived)
                .Include(p => p.ChangedFor)
                .Include(p => p.ApprovedBy)
                .AsSplitQuery()
                .OrderByDescending(x => x.ChangedOn)
                .AsEnumerable());
        }

        public async Task<IEnumerable<UpdateBase>> GetRosterUpdatesAsync()
        {
            ConcurrentBag<UpdateBase> data = new();

            await _dbContext
                .RankChanges
                .Include(p => p.ChangedBy)
                .Include(p => p.ChangedFor)
                .AsSplitQuery()
                .ForEachAsync(x =>
                {
                    if(x.SubmittedByRosterClerk)
                        data.Add(x);
                });

            await _dbContext
                .CShopChanges
                .Include(p => p.ChangedBy)
                .Include(p => p.ChangedFor)
                .AsSplitQuery()
                .ForEachAsync(x =>
                {
                    if(x.SubmittedByRosterClerk)
                        data.Add(x);
                });

            await _dbContext
                .QualificationChanges
                .Include(p => p.Instructors)
                .Include(p => p.ChangedFor)
                .AsSplitQuery()
                .ForEachAsync(x =>
                {
                    if(x.SubmittedByRosterClerk)
                        data.Add(x);
                });

            await _dbContext
                .SlotChanges
                .Include(p => p.ApprovedBy)
                .Include(p => p.ChangedFor)
                .AsSplitQuery()
                .ForEachAsync(x =>
                {
                    if(x.SubmittedByRosterClerk)
                        data.Add(x);
                });

            var dataList = data.AsEnumerable();

            return dataList.OrderByDescending(x => x.ChangedOn).AsEnumerable();
        }
    }
}
