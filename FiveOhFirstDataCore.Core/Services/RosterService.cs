using FiveOhFirstDataCore.Core.Account;
using FiveOhFirstDataCore.Core.Data.Roster;
using FiveOhFirstDataCore.Core.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Services
{
    public class RosterService : IRosterService
    {
        private readonly ApplicationDbContext _dbContext;

        public RosterService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
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
            var troopers = await _dbContext.Users.AsNoTracking().ToListAsync();

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
    }
}
