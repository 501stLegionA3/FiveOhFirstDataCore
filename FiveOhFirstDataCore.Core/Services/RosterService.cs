using FiveOhFirstDataCore.Core.Account;
using FiveOhFirstDataCore.Core.Data.Roster;
using FiveOhFirstDataCore.Core.Database;
using FiveOhFirstDataCore.Core.Structures;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Services
{
    public partial class RosterService : IRosterService
    {
        private readonly IServiceProvider _services;

        public RosterService(IServiceProvider services)
        {
            _services = services;
        }

        public async Task<List<Trooper>> GetActiveReservesAsync()
        {
            using var scope = _services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            List<Trooper> troopers = new();
            await dbContext.Users.AsNoTracking().ForEachAsync(x =>
            {
                if (x.Slot >= Data.Slot.ZetaCompany && x.Slot < Data.Slot.InactiveReserve)
                    troopers.Add(x);
            });

            return troopers;
        }

        public async Task<List<Trooper>> GetArchivedTroopersAsync()
        {
            using var scope = _services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            List<Trooper> troopers = new();
            await dbContext.Users.AsNoTracking().ForEachAsync(x =>
            {
                if (x.Slot == Data.Slot.Archived)
                    troopers.Add(x);
            });

            return troopers;
        }

        public async Task<List<Trooper>> GetFullRosterAsync()
        {
            using var scope = _services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            List<Trooper> troopers = new();
            await dbContext.Users.AsNoTracking().ForEachAsync(x =>
            {
                if (x.Slot < Data.Slot.Archived)
                    troopers.Add(x);
            });

            return troopers;
        }

        public async Task<List<Trooper>> GetInactiveReservesAsync()
        {
            using var scope = _services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            List<Trooper> troopers = new();
            await dbContext.Users.AsNoTracking().ForEachAsync(x =>
            {
                if (x.Slot == Data.Slot.InactiveReserve)
                    troopers.Add(x);
            });

            return troopers;
        }

        public async Task<(HashSet<int>, HashSet<string>)> GetInUseUserDataAsync()
        {
            using var scope = _services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            HashSet<int> ids = new();
            HashSet<string> nicknames = new();
            await dbContext.Users.AsNoTracking().ForEachAsync(x =>
            {
                ids.Add(x.Id);

                if (x.Slot < Data.Slot.InactiveReserve)
                    nicknames.Add(x.NickName);
            });

            return (ids, nicknames);
        }

        public async Task<OrbatData> GetOrbatDataAsync()
        {
            using var scope = _services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            OrbatData data = new();
            await dbContext.Users.AsNoTracking().ForEachAsync(x =>
            {
                if(x.Slot < Data.Slot.ZetaCompany)
                    data.Assign(x);
            });

            return data;
        }

        public async Task<List<Trooper>> GetPlacedRosterAsync()
        {
            using var scope = _services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            List<Trooper> troopers = new();
            await dbContext.Users.AsNoTracking().ForEachAsync(x =>
            {
                if (x.Slot < Data.Slot.ZetaCompany)
                    troopers.Add(x);
            });

            return troopers;
        }

        public async Task<Trooper?> GetTrooperFromIdAsync(int id)
        {
            using var scope = _services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var trooper = await dbContext.FindAsync<Trooper>(id);
            return trooper;
        }

        public async Task<Trooper?> GetTrooperFromClaimsPrincipalAsync(ClaimsPrincipal claims)
        {
            using var scope = _services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Trooper>>();
            var trooper = await userManager.GetUserAsync(claims);
            return trooper;
        }

        public async Task<List<Trooper>> GetUnregisteredTroopersAsync()
        {
            using var scope = _services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            List<Trooper> troopers = new();
            await dbContext.Users.AsNoTracking()
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
            using var scope = _services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            ZetaOrbatData data = new();
            await dbContext.Users.AsNoTracking().ForEachAsync(x =>
            {
                if (x.Slot >= Data.Slot.ZetaCompany && x.Slot < Data.Slot.InactiveReserve)
                    data.Assign(x);
            });

            return data;
        }

        public async Task<RegisterTrooperResult> RegisterTrooper(NewTrooperData trooperData)
        {
            using var scope = _services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Trooper>>();
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

                var identRes = await userManager.CreateAsync(trooper, token);

                if (!identRes.Succeeded)
                {
                    foreach (var error in identRes.Errors)
                        errors.Add($"[{error.Code}] {error.Description}");

                    return new(false, null, errors);
                }

                identRes = await userManager.AddToRoleAsync(trooper, "Trooper");

                if (!identRes.Succeeded)
                {
                    foreach (var error in identRes.Errors)
                        errors.Add($"[{error.Code}] {error.Description}");

                    return new(false, null, errors);
                }

                identRes = await userManager.AddClaimAsync(trooper, new("Training", "BCT"));

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
