using FiveOhFirstDataCore.Core.Account;
using FiveOhFirstDataCore.Core.Database;
using FiveOhFirstDataCore.Core.Structures;
using FiveOhFirstDataCore.Core.Structures.Updates;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Math.EC.Rfc7748;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Services
{
    public class UpdateService : IUpdateService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<Trooper> _userManager;

        public UpdateService(ApplicationDbContext dbContext, UserManager<Trooper> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
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

        public async Task<IEnumerable<UpdateBase>> GetAllUpdatesAsync()
        {
            var one = await _dbContext
                .RankUpdates
                .Include(p => p.ChangedBy)
                .Include(p => p.ChangedFor)
                .AsSplitQuery()
                .ToListAsync<UpdateBase>();

            var two = await _dbContext
                .CShopUpdates
                .Include(p => p.ChangedBy)
                .Include(p => p.ChangedFor)
                .AsSplitQuery()
                .ToListAsync<UpdateBase>();

            var three = await _dbContext
                .QualificationUpdates
                .Include(p => p.Instructors)
                .Include(p => p.ChangedFor)
                .AsSplitQuery()
                .ToListAsync<UpdateBase>();

            var four = await _dbContext
                .SlotUpdates
                .Include(p => p.ApprovedBy)
                .Include(p => p.ChangedFor)
                .AsSplitQuery()
                .ToListAsync<UpdateBase>();

            var five = await _dbContext
                .ClaimUpdates
                .Include(p => p.ChangedBy)
                .Include(p => p.ChangedFor)
                .Include(p => p.Additions)
                .Include(p => p.Removals)
                .AsSplitQuery()
                .ToListAsync<UpdateBase>();

            one.AddRange(two);
            one.AddRange(three);
            one.AddRange(four);
            one.AddRange(five);
            var dataList = one.AsEnumerable();

            return dataList.OrderByDescending(x => x.ChangedOn).AsEnumerable();
        }

        public Task<ResultBase> RevertUpdateAsync(Trooper manager, UpdateBase update) 
            => update switch
        {
            ClaimUpdate u => RevertClaimUpdateAsync(manager, u),
            CShopUpdate u => RevertCShopUpdateAsync(manager, u),
            NickNameUpdate u => RevertNickNameUpdateAsync(manager, u),
            QualificationUpdate u => RevertQualificationUpdateAsync(manager, u),
            RankUpdate u => RevertRankUpdateAsync(manager, u),
            RecruitmentUpdate u => RevertRecruitmentUpdateAsync(manager, u),
            SlotUpdate u => RevertSlotUpdateAsync(manager, u),
            _ => Task.FromResult(new ResultBase(false, new() { "No Update of this type has an implemented reversion process." }))
        };

        private async Task<ResultBase> RevertClaimUpdateAsync(Trooper manager, ClaimUpdate update)
        {
            if (update.AutomaticChange)
                return new(false, new()
                {
                    "Unable to revert claim updates on automatic changes. " +
                    "If needed, remove these manually or revert the change that changed these claims."
                });

            List<string> errors = new();

            var user = update.ChangedFor;

            List<Claim> toRemove = new();
            foreach(var data in update.Additions)
            {
                toRemove.Add(new(data.Key, data.Value));
            }

            var identResult = await _userManager.RemoveClaimsAsync(user, toRemove);
            if (!identResult.Succeeded)
            {
                foreach (var err in identResult.Errors)
                    errors.Add($"[{err.Code}] {err.Description}");

                return new(false, errors);
            }

            List<Claim> toAdd = new();
            foreach(var data in update.Removals)
            {
                toAdd.Add(new(data.Key, data.Value));
            }

            identResult = await _userManager.AddClaimsAsync(user, toAdd);
            if (!identResult.Succeeded)
            {
                foreach (var err in identResult.Errors)
                    errors.Add($"[{err.Code}] {err.Description}");

                return new(false, errors);
            }

            user.ClaimUpdates.Add(new()
            {
                Additions = update.Removals,
                Removals = update.Additions,
                RevertChange = true,
                ChangedById = manager.Id,
                ChangedOn = DateTime.UtcNow,
            });

            identResult = await _userManager.UpdateAsync(user);
            if (!identResult.Succeeded)
            {
                foreach (var err in identResult.Errors)
                    errors.Add($"[{err.Code}] {err.Description}");

                return new(false, errors);
            }

            return new(true, null);
        }

        private async Task<ResultBase> RevertCShopUpdateAsync(Trooper manager, CShopUpdate update)
        {

            throw new NotImplementedException();
        }

        private async Task<ResultBase> RevertNickNameUpdateAsync(Trooper manager, NickNameUpdate update)
        {
            throw new NotImplementedException();
        }

        private async Task<ResultBase> RevertQualificationUpdateAsync(Trooper manager, QualificationUpdate update)
        {
            throw new NotImplementedException();
        }

        private async Task<ResultBase> RevertRankUpdateAsync(Trooper manager, RankUpdate update)
        {
            throw new NotImplementedException();
        }

        private async Task<ResultBase> RevertRecruitmentUpdateAsync(Trooper manager, RecruitmentUpdate update)
        {
            throw new NotImplementedException();
        }

        private async Task<ResultBase> RevertSlotUpdateAsync(Trooper manager, SlotUpdate update)
        {
            throw new NotImplementedException();
        }
    }
}
