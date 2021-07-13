using FiveOhFirstDataCore.Core.Account;
using FiveOhFirstDataCore.Core.Data;
using FiveOhFirstDataCore.Core.Data.Promotions;
using FiveOhFirstDataCore.Core.Data.Roster;
using FiveOhFirstDataCore.Core.Database;
using FiveOhFirstDataCore.Core.Extensions;
using FiveOhFirstDataCore.Core.Structures;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
        private readonly UserManager<Trooper> _userManager;
        private readonly IDiscordService _discord;
        private readonly ILogger _logger;

        public RosterService(IDbContextFactory<ApplicationDbContext> dbContextFactory, UserManager<Trooper> userManager,
            IDiscordService discord, ILogger<RosterService> logger, IServiceProvider provider)
        {
            this._dbContextFactory = dbContextFactory;
            this._userManager = userManager;
            this._discord = discord;
            this._logger = logger;
        }

        public async Task<List<Trooper>> GetActiveReservesAsync()
        {
            using var _dbContext = _dbContextFactory.CreateDbContext();
            return await _dbContext.Users.AsNoTracking()
                .Where(x => x.Slot >= Data.Slot.ZetaCompany && x.Slot < Data.Slot.InactiveReserve)
                .ToListAsync();
        }

        public async Task<List<Trooper>> GetArchivedTroopersAsync()
        {
            using var _dbContext = _dbContextFactory.CreateDbContext();
            return await _dbContext.Users.AsNoTracking()
                .Where(x => x.Slot == Data.Slot.Archived)
                .ToListAsync();
        }

        public async Task<List<Trooper>> GetAllTroopersAsync(bool includeAdmin = false)
        {
            using var _dbContext = _dbContextFactory.CreateDbContext();
            if (includeAdmin)
                return await _dbContext.Users.AsNoTracking().ToListAsync();
            else
                return await _dbContext.Users.AsNoTracking()
                .Where(x => x.Id != -1).ToListAsync();
        }

        public async Task<List<Trooper>> GetFullRosterAsync()
        {
            using var _dbContext = _dbContextFactory.CreateDbContext();
            return await _dbContext.Users.AsNoTracking()
                .Where(x => x.Slot < Data.Slot.Archived).ToListAsync();
        }

        public async Task<List<Trooper>> GetInactiveReservesAsync()
        {
            using var _dbContext = _dbContextFactory.CreateDbContext();
            return await _dbContext.Users.AsNoTracking()
                .Where(x => x.Slot >= Data.Slot.InactiveReserve && x.Slot < Data.Slot.Archived)
                .ToListAsync();
        }

        public async Task<(HashSet<int>, HashSet<string>)> GetInUseUserDataAsync()
        {
            using var _dbContext = _dbContextFactory.CreateDbContext();
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
            using var _dbContext = _dbContextFactory.CreateDbContext();
            OrbatData data = new();
            await _dbContext.Users.AsNoTracking()
                .Where(x => x.Slot < Data.Slot.ZetaCompany)
                .ForEachAsync(x => data.Assign(x));

            return data;
        }

        public async Task<List<Trooper>> GetPlacedRosterAsync()
        {
            using var _dbContext = _dbContextFactory.CreateDbContext();
            return await _dbContext.Users.AsNoTracking()
                .Where(x => x.Slot < Data.Slot.ZetaCompany)
                .ToListAsync();
        }

        public async Task<Trooper?> GetTrooperFromIdAsync(int id)
        {
            using var _dbContext = _dbContextFactory.CreateDbContext();
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
            using var _dbContext = _dbContextFactory.CreateDbContext();
            return await _dbContext.Users.AsNoTracking()
                .Include(x => x.RecruitStatus)
                .Where(x => !string.IsNullOrEmpty(x.AccessCode))
                .ToListAsync();
        }

        public async Task<ZetaOrbatData> GetZetaOrbatDataAsync()
        {
            using var _dbContext = _dbContextFactory.CreateDbContext();
            ZetaOrbatData data = new();
            await _dbContext.Users.AsNoTracking()
                .Where(x => x.Slot >= Data.Slot.ZetaCompany && x.Slot < Data.Slot.InactiveReserve
                    || x.Slot == Data.Slot.Hailstorm)
                .ForEachAsync(x => data.Assign(x));

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

                var time = DateTime.UtcNow.ToEst();

                var trooper = new Trooper()
                {
                    Id = trooperData.Id,
                    NickName = trooperData.NickName,
                    Rank = trooperData.StartingRank,
                    UserName = token,
                    StartOfService = time,
                    LastPromotion = time,
                    AccessCode = token,
                    Slot = Data.Slot.InactiveReserve,
                };

                var recruiter = await GetTrooperFromClaimsPrincipalAsync(user);
                trooper.RecruitedByData = new()
                {
                    RecruitedById = recruiter?.Id ?? 0,
                    ChangedOn = time,
                };

                trooper.RecruitStatus = new()
                {
                    Age = trooperData.Age,
                    ModsInstalled = trooperData.ModsDownloaded,
                    OverSixteen = trooperData.Sixteen,
                    PossibleTroll = trooperData.PossibleTroll,
                    PreferredRole = trooperData.PreferredRole
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

        public async Task LoadPublicProfileDataAsync(Trooper trooper)
        {
            using var _dbContext = _dbContextFactory.CreateDbContext();
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
            using var _dbContext = _dbContextFactory.CreateDbContext();
            List<Trooper> sub = new();

            if (t is null) return sub;

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
                    if (t.Role == Data.Role.Commander
                        || t.Role == Data.Role.NCOIC
                        || t.Role == Data.Role.XO)
                    {
                        int slotDif = thisSlot - slot;
                        if (slotDif >= 0 && slotDif < 100)
                        {
                            if ((slotDif % 10) == 0)
                            {
                                // In the company staff
                                if (slotDif == 0
                                    || x.Role == Data.Role.Commander
                                    || x.Role == Data.Role.SergeantMajor)
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
                        || t.Role == Data.Role.SergeantMajor
                        || t.Role == Data.Role.MasterWarden 
                        || t.Role == Data.Role.ChiefWarden)
                    {
                        int slotDif = thisSlot - slot;
                        if (slotDif >= 0 && slotDif < 10)
                        {
                            //// In the platoon staff
                            //if (slotDif == 0) 
                            //    sub.Add(x);
                            //// This is a squad leader
                            //else if (x.Role == Data.Role.Lead && x.Team is null) 
                            //    sub.Add(x);

                            //else if (x.Role == Data.Role.Pilot || x.Role == Data.Role.Warden)
                            //    sub.Add(x);

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

        public async Task<SquadData?> GetSquadDataFromSlotAsync(Slot slot, bool manager)
        {
            if (!manager && !slot.IsSquad())
                return null;
            
            var data = new SquadData();

            await _dbContext.Users.AsNoTracking()
                .AsSplitQuery()
                .Include(p => p.PendingPromotions)
                .ThenInclude(p => p.RequestedBy)
                .Where(x => x.Slot == slot)
                .ForEachAsync(x => data.Assign(x));

            return data;
        }

        public async Task<SquadData?> GetSquadDataFromClaimPrincipalAsync(ClaimsPrincipal claims)
        {
            var user = await _userManager.GetUserAsync(claims);
            bool manager = await _userManager.IsInRoleAsync(user, "Admin") 
                || await _userManager.IsInRoleAsync(user, "Manager");
            return await GetSquadDataFromSlotAsync(user.Slot, manager);
        }

        public async Task<PlatoonData?> GetPlatoonDataFromSlotAsync(Slot slot, bool manager)
        {
            if (!manager && !slot.IsPlatoon())
                return null;

            var data = new PlatoonData(3);

            using var _dbContext = _dbContextFactory.CreateDbContext();

            var s = (int)slot / 10;
            await _dbContext.Users.AsNoTracking()
                .AsSplitQuery()
                .Include(p => p.PendingPromotions)
                .ThenInclude(p => p.RequestedBy)
                .Where(x => s == ((int)x.Slot / 10))
                .ForEachAsync(x => data.Assign(x));

            return data;
        }

        public async Task<PlatoonData?> GetPlatoonDataFromClaimPrincipalAsync(ClaimsPrincipal claims)
        {
            var user = await _userManager.GetUserAsync(claims);
            bool manager = await _userManager.IsInRoleAsync(user, "Admin")
                || await _userManager.IsInRoleAsync(user, "Manager");
            return await GetPlatoonDataFromSlotAsync(user.Slot, manager);
        }

        public async Task<CompanyData?> GetCompanyDataFromSlotAsync(Slot slot, bool manager)
        {
            if (!manager && !slot.IsCompany())
                return null;

            var data = new CompanyData(3, 3);

            using var _dbContext = _dbContextFactory.CreateDbContext();

            var s = (int)slot / 100;
            await _dbContext.Users.AsNoTracking()
                .AsSplitQuery()
                .Include(p => p.PendingPromotions)
                .ThenInclude(p => p.RequestedBy)
                .Where(x => s == ((int)x.Slot / 100))
                .ForEachAsync(x => data.Assign(x));

            return data;
        }

        public async Task<CompanyData?> GetCompanyDataFromClaimPrincipalAsync(ClaimsPrincipal claims)
        {
            var user = await _userManager.GetUserAsync(claims);
            bool manager = await _userManager.IsInRoleAsync(user, "Admin")
                || await _userManager.IsInRoleAsync(user, "Manager");
            return await GetCompanyDataFromSlotAsync(user.Slot, manager);
        }
    }
}
