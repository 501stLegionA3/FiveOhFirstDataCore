using FiveOhFirstDataCore.Data.Account;
using FiveOhFirstDataCore.Data.Structures;
using FiveOhFirstDataCore.Data.Structures.Roster;
using FiveOhFirstDataCore.Data.Structuresbase;
using FiveOhFirstDataCore.Data.Extensions;
using FiveOhFirstDataCore.Data.Structures;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using System.Security.Claims;

namespace FiveOhFirstDataCore.Data.Services
{
    public partial class RosterService : IRosterService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
        private readonly UserManager<Trooper> _userManager;
        private readonly ILogger _logger;
        private readonly IWebsiteSettingsService _settings;

        public RosterService(IDbContextFactory<ApplicationDbContext> dbContextFactory, UserManager<Trooper> userManager,
            ILogger<RosterService> logger, IWebsiteSettingsService settings)
        {
            _dbContextFactory = dbContextFactory;
            _userManager = userManager;
            _logger = logger;
            _settings = settings;
        }

        public async Task<List<Trooper>> GetActiveReservesAsync()
        {
            using var _dbContext = _dbContextFactory.CreateDbContext();
            return await _dbContext.Users.AsNoTracking()
                .Where(x => x.Slot >= Slot.ZetaCompany && x.Slot < Slot.InactiveReserve)
                .ToListAsync();
        }

        public async Task<List<Trooper>> GetArchivedTroopersAsync()
        {
            using var _dbContext = _dbContextFactory.CreateDbContext();
            return await _dbContext.Users.AsNoTracking()
                .Where(x => x.Slot == Slot.Archived)
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

        public async Task<List<Trooper>> GetFullRosterAsync(bool includePromotions = false)
        {
            using var _dbContext = _dbContextFactory.CreateDbContext();
            var data = _dbContext.Users
                .Where(x => x.Slot < Slot.Archived);
            if (includePromotions)
                data = data.Include(p => p.PendingPromotions)
                .ThenInclude(p => p.RequestedBy)
                .AsSplitQuery();

            return await data.ToListAsync();
        }

        public async Task<List<Trooper>> GetInactiveReservesAsync()
        {
            using var _dbContext = _dbContextFactory.CreateDbContext();
            return await _dbContext.Users.AsNoTracking()
                .Where(x => x.Slot >= Slot.InactiveReserve && x.Slot < Slot.Archived)
                .ToListAsync();
        }

        public async Task<(HashSet<int>, HashSet<string>)> GetInUseUserDataAsync()
        {
            using var _dbContext = _dbContextFactory.CreateDbContext();
            HashSet<int> ids = new();
            HashSet<string> nicknames = new();
            await _dbContext.Users.AsNoTracking().ForEachAsync(x =>
            {
                ids.Add(x.BirthNumber);

                if (x.Slot < Slot.Archived)
                    nicknames.Add(x.NickName);
            });

            return (ids, nicknames);
        }

        public async Task<OrbatData> GetOrbatDataAsync()
        {
            using var _dbContext = _dbContextFactory.CreateDbContext();
            OrbatData data = new();
            await _dbContext.Users.AsNoTracking()
                .Where(x => x.Slot < Slot.ZetaCompany)
                .ForEachAsync(x => data.Assign(x));

            return data;
        }

        public async Task<List<Trooper>> GetPlacedRosterAsync()
        {
            using var _dbContext = _dbContextFactory.CreateDbContext();
            return await _dbContext.Users.AsNoTracking()
                .Where(x => x.Slot < Slot.ZetaCompany)
                .ToListAsync();
        }

        public async Task<Trooper?> GetTrooperFromIdAsync(int id, bool loadNotificaitonTrackers = false)
        {
            using var _dbContext = _dbContextFactory.CreateDbContext();
            var trooper = await _dbContext.FindAsync<Trooper>(id);
            if (loadNotificaitonTrackers)
            {
                await _dbContext.Entry(trooper).Collection(e => e.TrooperReportTrackers).LoadAsync();

            }

            return trooper;
        }

        public async Task<Trooper?> GetTrooperFromBirthNumberAsync(int birthNumber, bool loadNotificationTrackers = false)
        {
            using var _dbContext = _dbContextFactory.CreateDbContext();
            var trooperId = (await _dbContext.Users
                .Where(e => e.BirthNumber == birthNumber)
                .AsNoTracking()
                .FirstOrDefaultAsync())?.Id ?? 0;
            return await GetTrooperFromIdAsync(trooperId, loadNotificationTrackers);
        }

        public async Task<Trooper?> GetTrooperFromClaimsPrincipalAsync(ClaimsPrincipal claims, bool loadNotificationTrackers = false)
        {
            _ = int.TryParse(_userManager.GetUserId(claims), out int id);
            return await GetTrooperFromIdAsync(id, loadNotificationTrackers);
        }

        public async Task<List<Trooper>> GetUnregisteredTroopersAsync()
        {
            using var _dbContext = _dbContextFactory.CreateDbContext();
            return await _dbContext.Users.AsNoTracking()
                .Include(x => x.RecruitStatus)
                .Where(x => !string.IsNullOrEmpty(x.AccessCode)
                    && x.Slot < Slot.Archived)
                .ToListAsync();
        }

        public async Task<ZetaOrbatData> GetZetaOrbatDataAsync()
        {
            using var _dbContext = _dbContextFactory.CreateDbContext();
            ZetaOrbatData data = new();
            await _dbContext.Users.AsNoTracking()
                .Where(x => x.Slot >= Slot.ZetaCompany && x.Slot < Slot.InactiveReserve
                    || x.Slot == Slot.Hailstorm)
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
                    BirthNumber = trooperData.Id,
                    NickName = trooperData.NickName,
                    Rank = trooperData.StartingRank,
                    UserName = token,
                    StartOfService = time,
                    LastPromotion = time,
                    AccessCode = token,
                    Slot = Slot.InactiveReserve,
                    Role = Role.Trooper
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

            if (_dbContext.Entry(trooper).State == EntityState.Detached)
                _dbContext.Attach(trooper);

            await _dbContext.Entry(trooper)
                .Collection(e => e.DisciplinaryActions)
                .LoadAsync();
            await _dbContext.Entry(trooper)
                .Collection(e => e.Flags)
                .LoadAsync();
        }

        public async Task LoadDescriptionsAsync(Trooper trooper)
        {
            using var _dbContext = _dbContextFactory.CreateDbContext();
            await _dbContext.TrooperDescriptions
                .Where(e => e.DescriptionForId == trooper.Id)
                .LoadAsync();

            if (_dbContext.Entry(trooper).State == EntityState.Detached)
                _dbContext.Attach(trooper);

            await _dbContext.Entry(trooper)
                .Collection(e => e.Descriptions)
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
                    if (t.Role == Role.Commander
                        || t.Role == Role.NCOIC
                        || t.Role == Role.XO)
                    {
                        int slotDif = thisSlot - slot;
                        if (slotDif >= 0 && slotDif < 100)
                        {
                            if ((slotDif % 10) == 0)
                            {
                                // In the company staff
                                if (slotDif == 0
                                    || x.Role == Role.Commander
                                    || x.Role == Role.SergeantMajor)
                                {
                                    sub.Add(x);
                                    return;
                                }
                            }
                            else if (x.Role == Role.Lead
                                    && x.Team is null
                                    && x.Slot >= Slot.Mynock
                                    && x.Slot < Slot.Razor)
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
                    if (t.Role == Role.Commander
                        || t.Role == Role.SergeantMajor
                        || t.Role == Role.MasterWarden
                        || t.Role == Role.ChiefWarden)
                    {
                        int slotDif = thisSlot - slot;
                        if (slotDif >= 0 && slotDif < 10)
                        {
                            //// In the platoon staff
                            //if (slotDif == 0) 
                            //    sub.Add(x);
                            //// This is a squad leader
                            //else if (x.Role == Role.Lead && x.Team is null) 
                            //    sub.Add(x);

                            //else if (x.Role == Role.Pilot || x.Role == Role.Warden)
                            //    sub.Add(x);

                            sub.Add(x);
                        }
                    }
                }
                // This is a squad
                else
                {
                    if (t.Role == Role.Lead)
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

        public async Task<Trooper?> GetDirectSuperior(Trooper t)
        {
            using var _dbContext = _dbContextFactory.CreateDbContext();
            Trooper? superior = null;
            List<Role> InfRoles = new() { Role.Commander, Role.XO, Role.NCOIC, Role.SergeantMajor };
            List<Role> RazorRoles = new() { Role.Commander, Role.SubCommander, Role.SCLO };
            List<Role> WardenRoles = new() { Role.MasterWarden, Role.ChiefWarden };
            List<Role> MynockRoles = new() { Role.Commander, Role.NCOIC };

            #region Squad
            if (t.Slot.IsSquad() && t.Role != Role.Lead)
            {
                superior = await _dbContext.Users
                    .Where(e => e.Role == Role.Lead)
                    .Where(e => e.Team == t.Team)
                    .Where(e => e.Slot == t.Slot)
                    .Where(e => e.Id != t.Id)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();
                if (superior is null)
                {
                    superior = await _dbContext.Users
                        .Where(e => e.Role == Role.Lead)
                        .Where(e => e.Team == null)
                        .Where(e => e.Slot == t.Slot)
                        .AsNoTracking()
                        .FirstOrDefaultAsync();
                }
            }
            else if (t.Slot.IsSquad() && t.Team is not null)
            {
                superior = await _dbContext.Users
                    .Where(e => e.Role == Role.Lead)
                    .Where(e => e.Slot == t.Slot)
                    .Where(e => e.Team == null)
                    .Where(e => e.Id != t.Id)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();
            }
            else if (t.Slot.IsSquad() && t.Team is null)
            {
                foreach(var role in InfRoles)
                {
                    superior = await _dbContext.Users
                        .Where(e => e.Role == role)
                        .Where(e => e.Slot == t.Slot.GetPlatoon())
                        .AsNoTracking()
                        .FirstOrDefaultAsync();
                    if (superior is null) continue;
                    else break;
                }
            }
            #endregion

            #region Platoon
            if (t.Slot.IsPlatoon() && t.Role != Role.Commander)
            {
                foreach (var role in InfRoles)
                {
                    superior = await _dbContext.Users
                        .Where(e => e.Role == role)
                        .Where(e => e.Slot == t.Slot)
                        .Where(e => e.Id != t.Id)
                        .AsNoTracking()
                        .FirstOrDefaultAsync();
                    if (superior is null) continue;
                    else break;
                }
            }
            else if (t.Slot.IsPlatoon() && t.Role == Role.Commander)
            {
                foreach (var role in InfRoles)
                {
                    superior = await _dbContext.Users
                        .Where(e => e.Role == role)
                        .Where(e => e.Slot == t.Slot.GetCompany())
                        .AsNoTracking()
                        .FirstOrDefaultAsync();
                    if (superior is null) continue;
                    else break;
                }

            }
            #endregion

            #region Company
            // Company Staff
            if (t.Slot.IsCompany() && t.Role != Role.Commander)
            {
                foreach(var role in InfRoles)
                {
                    superior = await _dbContext.Users
                        .Where(e => e.Role == role)
                        .Where(e => e.Slot == t.Slot)
                        .Where(e => e.Id != t.Id)
                        .AsNoTracking()
                        .FirstOrDefaultAsync();
                    if (superior is null) continue;
                    else break;
                }
            }
            //Company Commander
            else if (t.Slot.IsCompany())
            {
                foreach (var role in InfRoles)
                {
                    superior = await _dbContext.Users
                        .Where(e => e.Role == role)
                        .Where(e => e.Slot == Slot.Hailstorm)
                        .Where(e => e.Id != t.Id)
                        .AsNoTracking()
                        .FirstOrDefaultAsync();
                    if (superior is null) continue;
                    else break;
                }
            }
            #endregion

            #region Battalion
            if (t.Slot.IsBattalion() && t.Role != Role.Commander)
            {
                superior = await _dbContext.Users
                    .Where(e => e.Slot == Slot.Hailstorm)
                    .Where(e => e.Role == Role.Commander)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();
            }
            #endregion

            #region Razor

            if (t.Slot.IsRazorSection())
            {
                foreach (var role in RazorRoles)
                {
                    superior = await _dbContext.Users
                        .Where(e => e.Slot == t.Slot.GetFlight())
                        .Where(e => e.Role == role)
                        .Where(e => e.Id != t.Id)
                        .AsNoTracking()
                        .FirstOrDefaultAsync();
                    if (superior is null) continue;
                    else break;
                }
            }

            if (t.Slot.IsFlight())
            {
                foreach (var role in RazorRoles)
                {
                    superior = await _dbContext.Users
                        .Where(e => e.Slot == Slot.Razor)
                        .Where(e => e.Role == role)
                        .Where(e => e.Id != t.Id)
                        .AsNoTracking()
                        .FirstOrDefaultAsync();
                    if (superior is null) continue;
                    else break;
                }
            }

            if (t.Slot.IsSquadron() && t.Role != Role.Commander)
            {
                foreach (var role in RazorRoles)
                {
                    superior = await _dbContext.Users
                        .Where(e => e.Slot == t.Slot)
                        .Where(e => e.Role == role)
                        .Where(e => e.Id != t.Id)
                        .AsNoTracking()
                        .FirstOrDefaultAsync();
                    if (superior is null) continue;
                    else break;
                }
            }
            else if (t.Slot.IsSquadron())
            {
                foreach (var role in InfRoles)
                {
                    superior = await _dbContext.Users
                        .Where(e => e.Role == role)
                        .Where(e => e.Slot == Slot.Hailstorm)
                        .Where(e => e.Id != t.Id)
                        .AsNoTracking()
                        .FirstOrDefaultAsync();
                    if (superior is null) continue;
                    else break;
                }
            }
            #endregion

            #region Warden
            if (t.Slot.IsWardenTeam() && t.Team is not null && t.Flight is not Flight.Alpha)
            {
                superior = await _dbContext.Users
                    .Where(e => e.Role == Role.Warden)
                    .Where(e => e.Slot == t.Slot)
                    .Where(e => e.Flight == Flight.Alpha)
                    .Where(e => e.Team == t.Team)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();
                
                if (superior is null)
                {
                    superior = await _dbContext.Users
                        .Where(e => e.Role == Role.Warden)
                        .Where(e => e.Slot == t.Slot)
                        .Where(e => e.Team == null)
                        .Where(e => e.Flight == null)
                        .AsNoTracking()
                        .FirstOrDefaultAsync();

                    if (superior is null)
                    {
                        foreach(var role in WardenRoles)
                        {
                            superior = await _dbContext.Users
                                .Where(e => e.Role == role)
                                .Where(e => e.Slot == Slot.Warden)
                                .AsNoTracking()
                                .FirstOrDefaultAsync();
                            if (superior is null) continue;
                            else break;
                        }
                    }
                }
                
            }
            else if (t.Slot.IsWardenTeam() && t.Team is not null && t.Flight is Flight.Alpha)
            {
                superior = await _dbContext.Users
                    .Where(e => e.Role == Role.Warden)
                    .Where(e => e.Flight == null)
                    .Where(e => e.Slot == t.Slot)
                    .Where(e => e.Team == null)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();
                
                if (superior is null)
                {
                    foreach(var role in WardenRoles)
                    {
                        superior = await _dbContext.Users
                            .Where(e => e.Role == role)
                            .Where(e => e.Slot == Slot.Warden)
                            .AsNoTracking()
                            .FirstOrDefaultAsync();
                        if (superior is null) continue;
                        else break;
                    }
                }
            }
            else if (t.Slot.IsWardenTeam() && t.Team is null && t.Flight is null)
            {
                foreach (var role in WardenRoles)
                {
                    superior = await _dbContext.Users
                        .Where(e => e.Role == role)
                        .Where(e => e.Slot == Slot.Warden)
                        .AsNoTracking()
                        .FirstOrDefaultAsync();
                    if (superior is null) continue;
                    else break;
                }
            }

            if (t.Slot == Slot.Warden && t.Role != Role.MasterWarden)
            {
                superior = await _dbContext.Users
                    .Where(e => e.Role == Role.MasterWarden)
                    .Where(e => e.Slot == t.Slot)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();
                if (superior is null)
                {
                    foreach (var role in RazorRoles)
                    {
                        superior = await _dbContext.Users
                            .Where(e => e.Slot == Slot.Razor)
                            .Where(e => e.Role == role)
                            .Where(e => e.Id != t.Id)
                            .AsNoTracking()
                            .FirstOrDefaultAsync();
                        if (superior is null) continue;
                        else break;
                    }
                }
            }
            else if (t.Slot == Slot.Warden)
            {
                foreach (var role in RazorRoles)
                {
                    superior = await _dbContext.Users
                        .Where(e => e.Slot == Slot.Razor)
                        .Where(e => e.Role == role)
                        .Where(e => e.Id != t.Id)
                        .AsNoTracking()
                        .FirstOrDefaultAsync();
                    if (superior is null) continue;
                    else break;
                }
            }
            #endregion

            #region Mynock
            if (t.Slot.IsMynockSection() && t.Role is not Role.Lead)
            {
                superior = await _dbContext.Users
                    .Where(e => e.Id != t.Id)
                    .Where(e => e.Team == t.Team)
                    .Where(e => e.Role == Role.Lead)
                    .Where(e => e.Slot == t.Slot)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();
                if (superior is null)
                {
                    superior = await _dbContext.Users
                        .Where(e => e.Team == null)
                        .Where(e => e.Role == Role.Lead)
                        .Where(e => e.Slot == t.Slot)
                        .AsNoTracking()
                        .FirstOrDefaultAsync();
                }
            }
            else if (t.Slot.IsMynockSection() && t.Team is not null)
            {
                superior = await _dbContext.Users
                    .Where(e => e.Slot == t.Slot)
                    .Where(e => e.Role == Role.Lead)
                    .Where(e => e.Team == null)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();
            }
            else if (t.Slot.IsMynockSection())
            {
                foreach (var role in MynockRoles)
                {
                    superior = await _dbContext.Users
                        .Where(e => e.Slot == Slot.Mynock)
                        .Where(e => e.Role == role)
                        .AsNoTracking()
                        .FirstOrDefaultAsync();
                    if (superior is null) continue;
                    else break;
                }
            }

            if(t.Slot == Slot.Mynock && t.Role is not Role.Commander)
            {
                foreach (var role in MynockRoles)
                {
                    superior = await _dbContext.Users
                        .Where(e => e.Role == role)
                        .Where(e => e.Slot == t.Slot)
                        .Where(e => e.Id != t.Id)
                        .AsNoTracking()
                        .FirstOrDefaultAsync();
                    if (superior is null) continue;
                    else break;
                }
            }
            else if (t.Slot == Slot.Mynock)
            {
                foreach (var role in InfRoles)
                {
                    superior = await _dbContext.Users
                        .Where(e => e.Role == role)
                        .Where(e => e.Slot == Slot.Hailstorm)
                        .Where(e => e.Id != t.Id)
                        .AsNoTracking()
                        .FirstOrDefaultAsync();
                    if (superior is null) continue;
                    else break;
                }
            }
            #endregion

            return superior;
        }

        public async Task<IAssignable<Trooper>?> GetSquadDataAsync(Slot slot, bool manager)
        {
            if (!manager && !slot.IsSquad())
                return null;

            IAssignable<Trooper> data;

            if (slot >= Slot.ZetaCompany && slot < Slot.ZetaTwo)
                data = new ZetaSquadData();
            else if (slot >= Slot.ZetaTwo && slot < Slot.InactiveReserve)
                data = new ZetaUTCSquadData();
            else
                data = new SquadData();

            using var _dbContext = _dbContextFactory.CreateDbContext();

            await _dbContext.Users.AsNoTracking()
                .AsSplitQuery()
                .Include(p => p.PendingPromotions)
                .ThenInclude(p => p.RequestedBy)
                .Where(x => x.Slot == slot)
                .ForEachAsync(x => data.Assign(x));

            return data;
        }

        public async Task<IAssignable<Trooper>?> GetSquadDataAsync(ClaimsPrincipal claims)
        {
            var user = await _userManager.GetUserAsync(claims);
            if (user is null) return null;
            bool manager = await _userManager.IsInRoleAsync(user, "Admin")
                || await _userManager.IsInRoleAsync(user, "Manager");
            return await GetSquadDataAsync(user.Slot, manager);
        }

        public async Task<IAssignable<Trooper>?> GetPlatoonDataAsync(Slot slot, bool manager)
        {
            if (!manager && !slot.IsPlatoon() && !slot.IsSquad())
                return null;

            IAssignable<Trooper> data;

            if (slot >= Slot.ZetaCompany && slot < Slot.ZetaTwo)
                data = new ZetaSectionData(4);
            else if (slot >= Slot.ZetaTwo && slot < Slot.InactiveReserve)
                data = new ZetaUTCSectionData(4);
            else
                data = new PlatoonData(3);

            using var _dbContext = _dbContextFactory.CreateDbContext();

            var s = (int)slot / 10;
            await _dbContext.Users
                .AsSplitQuery()
                .Include(p => p.PendingPromotions)
                .ThenInclude(p => p.RequestedBy)
                .Where(x => s == ((int)x.Slot / 10))
                .ForEachAsync(x => data.Assign(x));

            return data;
        }

        public async Task<IAssignable<Trooper>?> GetPlatoonDataAsync(ClaimsPrincipal claims)
        {
            var user = await _userManager.GetUserAsync(claims);
            if (user is null) return null;
            bool manager = await _userManager.IsInRoleAsync(user, "Admin")
                || await _userManager.IsInRoleAsync(user, "Manager");
            return await GetPlatoonDataAsync(user.Slot, manager);
        }

        public async Task<IAssignable<Trooper>?> GetCompanyDataAsync(Slot slot, bool manager)
        {
            if (!manager && !slot.IsCompany() && !slot.IsPlatoon() && !slot.IsSquad())
                return null;

            IAssignable<Trooper> data;

            if (slot >= Slot.ZetaCompany && slot < Slot.InactiveReserve)
                data = new ZetaCompanyData(1, 4, 4);
            else
                data = new CompanyData(3, 3);

            using var _dbContext = _dbContextFactory.CreateDbContext();

            var s = (int)slot / 100;
            await _dbContext.Users
                .AsSplitQuery()
                .Include(p => p.PendingPromotions)
                .ThenInclude(p => p.RequestedBy)
                .Where(x => s == ((int)x.Slot / 100))
                .ForEachAsync(x => data.Assign(x));

            return data;
        }

        public async Task<IAssignable<Trooper>?> GetCompanyDataAsync(ClaimsPrincipal claims)
        {
            var user = await _userManager.GetUserAsync(claims);
            if (user is null) return null;
            bool manager = await _userManager.IsInRoleAsync(user, "Admin")
                || await _userManager.IsInRoleAsync(user, "Manager");
            return await GetCompanyDataAsync(user.Slot, manager);
        }

        public async Task<HailstormData> GetHailstormDataAsync()
        {
            var data = new HailstormData();

            using var _dbContext = _dbContextFactory.CreateDbContext();

            await _dbContext.Users.AsSplitQuery()
                .Include(p => p.PendingPromotions)
                .ThenInclude(p => p.RequestedBy)
                .Where(x => x.Slot == Slot.Hailstorm)
                .ForEachAsync(x => data.Assign(x));

            return data;
        }

        public async Task<List<Trooper>> GetTroopersWithPendingPromotionsAsync()
        {
            using var _dbContext = _dbContextFactory.CreateDbContext();

            var pending = await _dbContext.Users
                .Include(p => p.PendingPromotions)
                .ThenInclude(p => p.RequestedBy)
                .AsSplitQuery()
                .Where(p => p.PendingPromotions.Count > 0)
                .ToListAsync();

            return pending;
        }

        public async Task<RazorSquadronData> GetRazorDataAsync()
        {
            using var _dbContext = _dbContextFactory.CreateDbContext();

            var razor = new RazorSquadronData();

            await _dbContext.Users
                .Include(p => p.PendingPromotions)
                .ThenInclude(p => p.RequestedBy)
                .AsSplitQuery()
                .Where(p => p.Slot >= Slot.Razor && p.Slot < Slot.Warden)
                .ForEachAsync(x => razor.Assign(x));

            return razor;
        }

        public async Task<WardenData> GetWardenDataAsync()
        {
            using var _dbContext = _dbContextFactory.CreateDbContext();

            var warden = new WardenData();

            await _dbContext.Users
                .Include(p => p.PendingPromotions)
                .ThenInclude(p => p.RequestedBy)
                .AsSplitQuery()
                .Where(p => p.Slot >= Slot.Warden && p.Slot < Slot.ZetaCompany)
                .ForEachAsync(x => warden.Assign(x));

            return warden;
        }

        public async Task<MynockDetachmentData> GetMynockDataAsync()
        {
            using var _dbContext = _dbContextFactory.CreateDbContext();

            var mynock = new MynockDetachmentData();

            await _dbContext.Users
                .Include(p => p.PendingPromotions)
                .ThenInclude(p => p.RequestedBy)
                .AsSplitQuery()
                .Where(p => p.Slot >= Slot.Mynock && p.Slot < Slot.Razor)
                .ForEachAsync(x => mynock.Assign(x));

            return mynock;
        }

        public async Task<MynockSectionData?> GetMynockSectionDataAsync(Slot slot, bool manager)
        {
            if (manager || (slot >= Slot.Mynock && slot <= Slot.MynockOneThree))
            {
                var data = new MynockSectionData();

                using var _dbContext = _dbContextFactory.CreateDbContext();

                await _dbContext.Users.AsNoTracking()
                    .AsSplitQuery()
                    .Include(p => p.PendingPromotions)
                    .ThenInclude(p => p.RequestedBy)
                    .Where(x => x.Slot == slot && x.Slot >= Slot.Mynock && x.Slot < Slot.Razor)
                    .ForEachAsync(x => data.Assign(x));
                return data;
            }

            return null;
        }

        public async Task<MynockSectionData?> GetMynockSectionDataAsync(ClaimsPrincipal claims)
        {
            var user = await _userManager.GetUserAsync(claims);
            if (user is null) return null;
            bool manager = await _userManager.IsInRoleAsync(user, "Admin")
                || await _userManager.IsInRoleAsync(user, "Manager");
            return await GetMynockSectionDataAsync(user.Slot, manager);
        }

        public async Task<ZetaUTCSectionData> GetZetaUTCSectionDataAsync()
        {
            var data = new ZetaUTCSectionData(4);

            using var _dbContext = _dbContextFactory.CreateDbContext();

            await _dbContext.Users.AsNoTracking()
                .AsSplitQuery()
                .Where(x => x.Slot >= Slot.ZetaTwo && x.Slot < Slot.InactiveReserve)
                .ForEachAsync(x => data.Assign(x));

            return data;
        }

        public async Task<ZetaUTCSquadData> GetZetaUTCSquadFromSlotAsync(Slot slot)
        {
            var data = new ZetaUTCSquadData();

            using var _dbContext = _dbContextFactory.CreateDbContext();

            await _dbContext.Users.AsNoTracking()
                .AsSplitQuery()
                .Where(x => x.Slot == slot && x.Slot >= Slot.ZetaCompany && x.Slot < Slot.InactiveReserve)
                .ForEachAsync(x => data.Assign(x));

            return data;
        }

        public async Task<List<Trooper>> GetNewRecruitsAsync()
        {
            using var _dbContext = _dbContextFactory.CreateDbContext();
            return await _dbContext.Users
                .Where(x => x.Rank == TrooperRank.Recruit)
                .Where(x => x.Slot != Slot.Archived)
                .Include(x => x.RecruitStatus)
                .AsSplitQuery()
                .ToListAsync();
        }

        public async Task<List<Trooper>> GetCurrentUTCCadets()
        {
            using var _dbContext = _dbContextFactory.CreateDbContext();
            return await _dbContext.Users
                .Where(x => x.Rank == TrooperRank.Cadet)
                .Where(x => x.Slot >= Slot.ZetaTwo
                    && x.Slot <= Slot.ZetaTwoFour)
                .Include(x => x.RecruitStatus)
                .AsSplitQuery()
                .ToListAsync();
        }

        public async Task<Trooper?> GetAcklayAdjutantAsync()
        {
            using var _dbContext = _dbContextFactory.CreateDbContext();
            return await _dbContext.Users
                .Where(x => x.Slot == Slot.AcklayOne)
                .Where(x => x.Role == Role.Adjutant)
                .FirstOrDefaultAsync();
        }
    }
}