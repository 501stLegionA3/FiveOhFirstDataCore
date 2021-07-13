using FiveOhFirstDataCore.Core.Account;
using FiveOhFirstDataCore.Core.Data;
using FiveOhFirstDataCore.Core.Database;
using FiveOhFirstDataCore.Core.Extensions;
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
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
        private readonly UserManager<Trooper> _userManager;

        public UpdateService(IDbContextFactory<ApplicationDbContext> dbContextFactory, UserManager<Trooper> userManager)
        {
            _dbContextFactory = dbContextFactory;
            _userManager = userManager;
        }

        public async Task<List<RecruitmentUpdate>> GetRecruitmentChangesAsync()
        {
            using var _dbContext = _dbContextFactory.CreateDbContext();
            return await _dbContext
                .RecruitmentUpdates
                .Include(p => p.ChangedFor)
                .Include(p => p.RecruitedBy)
                .AsSplitQuery()
                .OrderByDescending(x => x.ChangedOn)
                .ToListAsync();
        }

        public async Task<List<SlotUpdate>> GetReturningMemberChangesAsync()
        {
            using var _dbContext = _dbContextFactory.CreateDbContext();
            return await _dbContext
                .SlotUpdates
                .Where(x => x.OldSlot == Data.Slot.Archived)
                .Include(p => p.ChangedFor)
                .Include(p => p.ApprovedBy)
                .AsSplitQuery()
                .OrderByDescending(x => x.ChangedOn)
                .ToListAsync();
        }

        public async Task<List<UpdateBase>> GetRosterUpdatesAsync()
        {
            using var _dbContext = _dbContextFactory.CreateDbContext();
            var one = await _dbContext
                .RankUpdates
                .Where(x => x.SubmittedByRosterClerk)
                .Include(p => p.RequestedBy)
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

            var five = await _dbContext
                .TimeUpdates
                .Where(x => x.SubmittedByRosterClerk)
                .Include(p => p.ChangedBy)
                .Include(p => p.ChangedFor)
                .AsSplitQuery()
                .ToListAsync<UpdateBase>();

            one.AddRange(two);
            one.AddRange(three);
            one.AddRange(four);
            one.AddRange(five);
            var dataList = one.AsEnumerable();

            return dataList.OrderByDescending(x => x.ChangedOn).ToList();
        }

        public async Task<List<UpdateBase>> GetAllUpdatesAsync()
        {
            using var _dbContext = _dbContextFactory.CreateDbContext();
            var one = await _dbContext
                .RankUpdates
                .Include(p => p.RequestedBy)
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

            var six = await _dbContext
                .TimeUpdates
                .Include(p => p.ChangedBy)
                .Include(p => p.ChangedFor)
                .AsSplitQuery()
                .ToListAsync<UpdateBase>();

            one.AddRange(two);
            one.AddRange(three);
            one.AddRange(four);
            one.AddRange(five);
            one.AddRange(six);
            var dataList = one.AsEnumerable();

            return dataList.OrderByDescending(x => x.ChangedOn).ToList();
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
            TimeUpdate u => RevertTimeUpdateAsync(manager, u),
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
                ChangedOn = DateTime.UtcNow.ToEst(),
            });

            identResult = await _userManager.UpdateAsync(user);
            if (!identResult.Succeeded)
            {
                foreach (var err in identResult.Errors)
                    errors.Add($"[{err.Code}] {err.Description}");

                return new(false, errors);
            }

            return new(true);
        }

        private async Task<ResultBase> RevertCShopUpdateAsync(Trooper manager, CShopUpdate update)
        {
            return new(false, new() { "C-Shop changes are unabled to be reverted at this time." });
        }

        private async Task<ResultBase> RevertNickNameUpdateAsync(Trooper manager, NickNameUpdate update)
        {
            using var _dbContext = _dbContextFactory.CreateDbContext();
            update.ChangedFor.NickName = update.OldNickname;
            update.ChangedFor.NickNameUpdates.Add(new()
            {
                OldNickname = update.NewNickname,
                NewNickname = update.OldNickname,
                ApprovedById = manager.Id,
                ChangedOn = DateTime.UtcNow.ToEst(),
                RevertChange = true
            });

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return new(false, new() { ex.Message });
            }

            return new(true);
        }

        private async Task<ResultBase> RevertQualificationUpdateAsync(Trooper manager, QualificationUpdate update)
        {
            using var _dbContext = _dbContextFactory.CreateDbContext();
            var oldQuals = update.ChangedFor.Qualifications;
            update.ChangedFor.Qualifications = update.OldQualifications;
            update.ChangedFor.QualificationUpdates.Add(new()
            {
                OldQualifications = oldQuals,
                Added = update.Removed,
                Removed = update.Added,
                Instructors = new() { manager },
                ChangedOn = DateTime.UtcNow.ToEst(),
                RevertChange = true,
            });

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return new(false, new() { ex.Message });
            }

            return new(true);
        }

        private async Task<ResultBase> RevertRankUpdateAsync(Trooper manager, RankUpdate update)
        {
            var rank = update.ChangedFrom.GetRank();
            switch(rank)
            {
                case TrooperRank r:
                    update.ChangedFor.Rank = r;
                    break;
                case RTORank r:
                    update.ChangedFor.RTORank = r;
                    break;
                case MedicRank r:
                    update.ChangedFor.MedicRank = r;
                    break;
                case WardenRank r:
                    update.ChangedFor.WardenRank = r;
                    break;
                case WarrantRank r:
                    update.ChangedFor.WarrantRank = r;
                    break;
                case PilotRank r:
                    update.ChangedFor.PilotRank = r;
                    break;
            }

            update.ChangedFor.RankChanges.Add(new()
            {
                ChangedFrom = update.ChangedTo,
                ChangedTo = update.ChangedFrom,
                RequestedById = manager.Id,
                ChangedOn = DateTime.UtcNow.ToEst(),
                RevertChange = true
            });

            try
            {
                using var _dbContext = _dbContextFactory.CreateDbContext();
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return new(false, new() { ex.Message });
            }

            return new(true);
        }

        private async Task<ResultBase> RevertRecruitmentUpdateAsync(Trooper manager, RecruitmentUpdate update)
        {
            return new(false, new() { "Recruitment data is unable to be changed. To remove a recruit, archive or delete the account." });
        }

        private async Task<ResultBase> RevertSlotUpdateAsync(Trooper manager, SlotUpdate update)
        {
            update.ChangedFor.Slot = update.OldSlot;
            update.ChangedFor.Team = update.OldTeam;
            update.ChangedFor.Role = update.OldRole ?? update.ChangedFor.Role;
            update.ChangedFor.Flight = update.OldFlight;
            update.ChangedFor.SlotUpdates.Add(new()
            {
                NewSlot = update.OldSlot,
                NewRole = update.OldRole,
                NewFlight = update.OldFlight,
                NewTeam = update.NewTeam,

                OldSlot = update.NewSlot,
                OldRole = update.NewRole,
                OldFlight = update.NewFlight,
                OldTeam = update.NewTeam,

                ApprovedBy = new() { manager },
                ChangedOn = DateTime.UtcNow.ToEst(),
                RevertChange = true
            });

            try
            {
                using var _dbContext = _dbContextFactory.CreateDbContext();
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return new(false, new() { ex.Message });
            }

            return new(true);
        }

        private async Task<ResultBase> RevertTimeUpdateAsync(Trooper manager, TimeUpdate update)
        {
            if(update.OldGraduatedBCT is not null)
            {
                update.ChangedFor.GraduatedBCTOn = update.OldGraduatedBCT.Value;
            }

            if(update.OldGraduatedUTC is not null)
            {
                update.ChangedFor.GraduatedUTCOn = update.OldGraduatedUTC.Value;
            }

            if (update.OldBilletChange is not null)
            {
                update.ChangedFor.LastBilletChange = update.OldBilletChange.Value;
            }

            if (update.OldPromotion is not null)
            {
                update.ChangedFor.LastPromotion = update.OldPromotion.Value;
            }

            if(update.OldStartOfService is not null)
            {
                update.ChangedFor.StartOfService = update.OldStartOfService.Value;
            }

            update.ChangedFor.TimeUpdates.Add(new()
            {
                NewGraduatedBCT = update.OldGraduatedBCT,
                OldGraduatedBCT = update.NewGraduatedBCT,

                NewGraduatedUTC = update.OldGraduatedUTC,
                OldGraduatedUTC = update.NewGraduatedUTC,

                NewBilletChange = update.OldBilletChange,
                OldBilletChange = update.NewBilletChange,

                NewPromotion = update.OldPromotion,
                OldPromotion = update.NewPromotion,

                NewStartOfService = update.OldStartOfService,
                OldStartOfService = update.NewStartOfService,

                ChangedOn = DateTime.UtcNow.ToEst(),
                ChangedById = manager.Id,
                SubmittedByRosterClerk = update.SubmittedByRosterClerk,
                RevertChange = true
            });

            try
            {
                using var _dbContext = _dbContextFactory.CreateDbContext();
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return new(false, new() { ex.Message });
            }

            return new(true);
        }
    }
}
