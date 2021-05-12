using FiveOhFirstDataCore.Core.Account;
using FiveOhFirstDataCore.Core.Data.Roster;
using FiveOhFirstDataCore.Core.Database;
using FiveOhFirstDataCore.Core.Structures;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Services
{
    public class RosterService : IRosterService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<Trooper> _userManager;

        public RosterService(ApplicationDbContext dbContext, UserManager<Trooper> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public async Task<List<Trooper>> GetActiveReservesAsync()
        {
            List<Trooper> troopers = new();
            await _dbContext.Users.AsNoTracking().ForEachAsync(x =>
            {
                if (x.Slot >= Data.Slot.ZetaCompany && x.Slot < Data.Slot.InactiveReserve)
                    troopers.Add(x);
            });

            return troopers;
        }

        public async Task<List<Trooper>> GetArchivedTroopersAsync()
        {
            List<Trooper> troopers = new();
            await _dbContext.Users.AsNoTracking().ForEachAsync(x =>
            {
                if (x.Slot == Data.Slot.Archived)
                    troopers.Add(x);
            });

            return troopers;
        }

        public async Task<List<Trooper>> GetFullRosterAsync()
        {
            List<Trooper> troopers = new();
            await _dbContext.Users.AsNoTracking().ForEachAsync(x =>
            {
                if (x.Slot < Data.Slot.Archived)
                    troopers.Add(x);
            });

            return troopers;
        }

        public async Task<List<Trooper>> GetInactiveReservesAsync()
        {
            List<Trooper> troopers = new();
            await _dbContext.Users.AsNoTracking().ForEachAsync(x =>
            {
                if (x.Slot == Data.Slot.InactiveReserve)
                    troopers.Add(x);
            });

            return troopers;
        }

        public async Task<(HashSet<int>, HashSet<string>)> GetInUseUserDataAsync()
        {
            HashSet<int> ids = new();
            HashSet<string> nicknames = new();
            await _dbContext.Users.AsNoTracking().ForEachAsync(x =>
            {
                ids.Add(x.Id);

                if (x.Slot < Data.Slot.InactiveReserve)
                    nicknames.Add(x.NickName);
            });

            return (ids, nicknames);
        }

        public async Task<OrbatData> GetOrbatDataAsync()
        {
            OrbatData data = new();
            await _dbContext.Users.AsNoTracking().ForEachAsync(x =>
            {
                if(x.Slot < Data.Slot.ZetaCompany)
                    data.Assign(x);
            });

            return data;
        }

        public async Task<List<Trooper>> GetPlacedRosterAsync()
        {
            List<Trooper> troopers = new();
            await _dbContext.Users.AsNoTracking().ForEachAsync(x =>
            {
                if (x.Slot < Data.Slot.ZetaCompany)
                    troopers.Add(x);
            });

            return troopers;
        }

        public async Task<List<Trooper>> GetUnregisteredTroopersAsync()
        {
            List<Trooper> troopers = new();
            await _dbContext.Users.AsNoTracking()
                .Include(x => x.RecruitStatus)
                .ForEachAsync(x =>
            {
                if (!string.IsNullOrEmpty(x.AccessCode))
                    troopers.Add(x);
            });

            return troopers;
        }

        public async Task<ZetaOrbatData> GetZetaOrbatDataAsync()
        {
            ZetaOrbatData data = new();
            await _dbContext.Users.AsNoTracking().ForEachAsync(x =>
            {
                if (x.Slot >= Data.Slot.ZetaCompany && x.Slot < Data.Slot.InactiveReserve)
                    data.Assign(x);
            });

            return data;
        }

        public async Task<RegisterTrooperResult> RegisterTrooper(NewTrooperData trooperData)
        {
            List<string> errors = new();
            try
            {
                var uniqueSets = await GetInUseUserDataAsync();
                if (uniqueSets.Item1.Contains(trooperData.Id))
                {
                    errors.Add("This ID is already in use.");
                }

                if (uniqueSets.Item2.Contains(trooperData.NickName))
                {
                    errors.Add("This Nickname is already in use.");
                }

                if (errors.Count > 0)
                    return new(false, null, errors);

                var token = Guid.NewGuid().ToString();

                var trooper = new Trooper()
                {
                    Id = trooperData.Id,
                    NickName = trooperData.NickName,
                    Rank = trooperData.StartingRank,
                    UserName = token,
                    StartOfService = DateTime.Now,
                    LastPromotion = DateTime.Now,
                    AccessCode = token
                };

                var identRes = await _userManager.CreateAsync(trooper, token);

                if (!identRes.Succeeded)
                {
                    foreach (var error in identRes.Errors)
                        errors.Add($"[{error.Code}] {error.Description}");

                    return new(false, null, errors);
                }

                identRes = await _userManager.AddToRoleAsync(trooper, "Trooper");

                if (!identRes.Succeeded)
                {
                    foreach (var error in identRes.Errors)
                        errors.Add($"[{error.Code}] {error.Description}");

                    return new(false, null, errors);
                }

                identRes = await _userManager.AddClaimAsync(trooper, new("Training", "BCT"));

                if (!identRes.Succeeded)
                {
                    foreach (var error in identRes.Errors)
                        errors.Add($"[{error.Code}] {error.Description}");

                    return new(false, null, errors);
                }

                return new(true, token, null);
            }
            catch (Exception ex)
            {
                errors.Add(ex.Message);
                return new(false, null, errors);
            }
        }
    }
}
