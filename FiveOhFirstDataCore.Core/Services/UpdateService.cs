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
            var one = await _dbContext
                .RankChanges
                .Where(x => x.SubmittedByRosterClerk)
                .Include(p => p.ChangedBy)
                .Include(p => p.ChangedFor)
                .AsSplitQuery()
                .ToListAsync<UpdateBase>();

            var two = await _dbContext
                .CShopChanges
                .Where(x => x.SubmittedByRosterClerk)
                .Include(p => p.ChangedBy)
                .Include(p => p.ChangedFor)
                .AsSplitQuery()
                .ToListAsync<UpdateBase>();

            var three = await _dbContext
                .QualificationChanges
                .Where(x => x.SubmittedByRosterClerk)
                .Include(p => p.Instructors)
                .Include(p => p.ChangedFor)
                .AsSplitQuery()
                .ToListAsync<UpdateBase>();

            var four = await _dbContext
                .SlotChanges
                .Where(x => x.SubmittedByRosterClerk)
                .Include(p => p.ApprovedBy)
                .Include(p => p.ChangedFor)
                .AsSplitQuery()
                .ToListAsync<UpdateBase>();

            one.AddRange(two);
            one.AddRange(three);
            one.AddRange(four);
            var dataList = one.AsEnumerable();

            return dataList.OrderByDescending(x => x.ChangedOn).AsEnumerable();
        }
    }
}
