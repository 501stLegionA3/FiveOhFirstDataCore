using FiveOhFirstDataCore.Core.Account;
using FiveOhFirstDataCore.Core.Data.Roster;
using FiveOhFirstDataCore.Core.Database;
using FiveOhFirstDataCore.Core.Structures;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Services
{
    public partial class RosterService : IRosterService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<Trooper> _userManager;
        private readonly IDiscordService _discord;

        public RosterService(ApplicationDbContext dbContext, UserManager<Trooper> userManager,
            IDiscordService discord)
        {
            this._dbContext = dbContext;
            this._userManager = userManager;
            this._discord = discord;
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

        public async Task<List<Trooper>> GetAllTroopersAsync()
        {
            return await _dbContext.Users.AsNoTracking().ToListAsync();
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

        public async Task<Trooper?> GetTrooperFromIdAsync(int id)
        {
            var trooper = await _dbContext.FindAsync<Trooper>(id);
            return trooper;
        }

        public async Task<Trooper?> GetTrooperFromClaimsPrincipalAsync(ClaimsPrincipal claims)
        {
            _ = int.TryParse(_userManager.GetUserId(claims), out int id);
            return await GetTrooperFromIdAsync(id);
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
                if (x.Slot >= Data.Slot.ZetaCompany && x.Slot < Data.Slot.InactiveReserve
                    || x.Slot == Data.Slot.Hailstorm)
                    data.Assign(x);
            });

            return data;
        }

        public async Task<RegisterTrooperResult> RegisterTrooper(NewTrooperData trooperData, ClaimsPrincipal user)
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

                var recruiter = await GetTrooperFromClaimsPrincipalAsync(user);
                trooper.RecruitedByData = new()
                {
                    RecruitedById = recruiter?.Id ?? 0,
                    ChangedOn = DateTime.UtcNow,
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

                identRes = await _userManager.AddClaimAsync(trooper, new("Display", $"{trooper.Id} {trooper.NickName}"));

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

        public async Task LoadPublicProfileDataAsync(Trooper trooper)
        {
            await _dbContext.DisciplinaryActions.Include(e => e.FiledBy)
                .Where(e => e.FiledAgainstId == trooper.Id)
                .LoadAsync();

            await _dbContext.TrooperFlags.Include(e => e.Author)
                .Where(e => e.FlagForId == trooper.Id)
                .LoadAsync();

            if(_dbContext.Entry(trooper).State == EntityState.Detached)
                _dbContext.Attach(trooper);

            await _dbContext.Entry(trooper)
                .Collection(e => e.DisciplinaryActions)
                .LoadAsync();
            await _dbContext.Entry(trooper)
                .Collection(e => e.Flags)
                .LoadAsync();
        }

        public async Task<List<Trooper>> GetDirectSubordinates(Trooper t)
        {
            List<Trooper> sub = new();

            await _dbContext.Users
                .AsNoTracking()
                .ForEachAsync(x =>
            {
                if (x.Id == t.Id) return;
                if (x.AccessCode is null) return;

                int slot = (int)t.Slot;
                int thisSlot = (int)x.Slot;

                if (slot == 0)
                {
                    if ((slot / 10 % 10) == 0)
                    {
                        sub.Add(x);
                        return;
                    }
                }
                // This is a company
                else if ((slot / 10 % 10) == 0)
                {
                    if (t.Role == Data.Role.Commander)
                    {
                        int slotDif = thisSlot - slot;
                        if (slotDif >= 0 && slotDif < 100)
                        {
                            if ((slotDif % 10) == 0)
                            {
                                // In the company staff
                                if (slotDif == 0
                                    || x.Role == Data.Role.Commander)
                                {
                                    sub.Add(x);
                                    return;
                                }
                            }
                            else if (x.Role == Data.Role.Lead
                                    && x.Team is null
                                    && x.Slot >= Data.Slot.Mynock
                                    && x.Slot < Data.Slot.Razor)
                            {
                                sub.Add(x);
                                return;
                            }
                        }
                    }
                }

                // This is a Plt.
                if ((slot % 10) == 0)
                {
                    if (t.Role == Data.Role.Commander 
                        || t.Role == Data.Role.MasterWarden 
                        || t.Role == Data.Role.CheifWarden)
                    {
                        int slotDif = thisSlot - slot;
                        if (slotDif >= 0 && slotDif < 10)
                        {
                            // In the platoon staff
                            if (slotDif == 0) 
                                sub.Add(x);
                            // This is a squad leader
                            else if (x.Role == Data.Role.Lead && x.Team is null) 
                                sub.Add(x);

                            else if (x.Role == Data.Role.Pilot || x.Role == Data.Role.Warden)
                                sub.Add(x);
                        }
                    }
                }
                // This is a squad
                else
                {
                    if(t.Role == Data.Role.Lead)
                    {
                        if (thisSlot == slot)
                        {
                            if (t.Team is null)
                                sub.Add(x);
                            else if (t.Team == x.Team)
                                sub.Add(x);
                        }
                    }
                }
            });

            return sub;
        }
    }
}
