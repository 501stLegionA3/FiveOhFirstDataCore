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

        public Task<IEnumerable<RecruitmentUpdate>> GetRecruitmentChangesAsync()
        {
            return Task.FromResult(_dbContext
                .RecruitmentUpdates
                .Include(p => p.ChangedFor)
                .Include(p => p.RecruitedBy)
                .AsSplitQuery()
                .OrderByDescending(x => x.ChangedOn)
                .AsEnumerable());
        }

        public Task<IEnumerable<SlotUpdate>> GetReturningMemberChangesAsync()
        {
            return Task.FromResult(_dbContext
                .SlotUpdates
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
                .RankUpdates
                .Where(x => x.SubmittedByRosterClerk)
                .Include(p => p.ChangedBy)
                .Include(p => p.ChangedFor)
                .AsSplitQuery()
                .ToListAsync<UpdateBase>();

            var two = await _dbContext
                .CShopUpdates
                .Where(x => x.SubmittedByRosterClerk)
                .Include(p => p.ChangedBy)
                .Include(p => p.ChangedFor)
                .AsSplitQuery()
                .ToListAsync<UpdateBase>();

            var three = await _dbContext
                .QualificationUpdates
                .Where(x => x.SubmittedByRosterClerk)
                .Include(p => p.Instructors)
                .Include(p => p.ChangedFor)
                .AsSplitQuery()
                .ToListAsync<UpdateBase>();

            var four = await _dbContext
                .SlotUpdates
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

        public Task<bool> RevertUpdateAsync(UpdateBase update) 
            => update switch
        {
            ClaimUpdate u => RevertClaimUpdateAsync(u),
            CShopUpdate u => RevertCShopUpdateAsync(u),
            NickNameUpdate u => RevertNickNameUpdateAsync(u),
            QualificationUpdate u => RevertQualificationUpdateAsync(u),
            RankUpdate u => RevertRankUpdateAsync(u),
            RecruitmentUpdate u => RevertRecruitmentUpdateAsync(u),
            SlotUpdate u => RevertSlotUpdateAsync(u),
            _ => Task.FromResult(false),
        };

        private async Task<bool> RevertClaimUpdateAsync(ClaimUpdate update)
        {
            throw new NotImplementedException();
        }

        private async Task<bool> RevertCShopUpdateAsync(CShopUpdate update)
        {
            throw new NotImplementedException();
        }

        private async Task<bool> RevertNickNameUpdateAsync(NickNameUpdate update)
        {
            throw new NotImplementedException();
        }

        private async Task<bool> RevertQualificationUpdateAsync(QualificationUpdate update)
        {
            throw new NotImplementedException();
        }

        private async Task<bool> RevertRankUpdateAsync(RankUpdate update)
        {
            throw new NotImplementedException();
        }

        private async Task<bool> RevertRecruitmentUpdateAsync(RecruitmentUpdate update)
        {
            throw new NotImplementedException();
        }

        private async Task<bool> RevertSlotUpdateAsync(SlotUpdate update)
        {
            throw new NotImplementedException();
        }
    }
}
