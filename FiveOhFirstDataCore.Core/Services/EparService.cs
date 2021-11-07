using FiveOhFirstDataCore.Data.Account;
using FiveOhFirstDataCore.Data.Structures;
using FiveOhFirstDataCore.Data.Structuresbase;
using FiveOhFirstDataCore.Data.Extensions;
using FiveOhFirstDataCore.Data.Structures;

using Microsoft.EntityFrameworkCore;

using System.Security.Claims;

namespace FiveOhFirstDataCore.Data.Services
{
    public class EparService : IEparService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
        private readonly IRosterService _roster;

        public EparService(IDbContextFactory<ApplicationDbContext> dbContextFactory, IRosterService roster)
            => (_dbContextFactory, _roster) = (dbContextFactory, roster);

        public async Task<ResultBase> CreateChangeRequest(TrooperChangeRequestData request, int submitterId)
        {
            if (request is null) return new(false, new List<string> { "The request object was null." });

            request.ChangedOn = DateTime.UtcNow.ToEst();

            await using var _dbContext = _dbContextFactory.CreateDbContext();

            var trooper = await _dbContext.FindAsync<Trooper>(submitterId);

            // We can't add anything to a null item.
            if (trooper is null) return new(false, new List<string> { "There was no trooper found for the inputed ID." });

            if (request.Qualifications != trooper.Qualifications)
                request.QualsChanged = true;

            trooper.TrooperChangeRequests.Add(request);

            await _dbContext.SaveChangesAsync();

            return new(true, null);
        }

        public async Task<IReadOnlyList<TrooperChangeRequestData>> GetActiveChangeRequests()
        {
            await using var _dbContext = _dbContextFactory.CreateDbContext();

            return await _dbContext.ChangeRequests
                .Where(x => !x.Finalized)
                .Include(x => x.ChangedFor)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<TrooperChangeRequestData>> GetActiveChangeRequests(int start, int end, object[] args)
        {
            await using var _dbContext = _dbContextFactory.CreateDbContext();

            return _dbContext.ChangeRequests
                .Where(x => !x.Finalized)
                .OrderBy(x => x.ChangedOn)
                .Include(x => x.ChangedFor)
                .AsEnumerable()
                .Take(new Range(start, end))
                .ToList();
        }

        public async Task<int> GetActiveChangeRequestCount(object[] args)
        {
            await using var _dbContext = _dbContextFactory.CreateDbContext();

            return await _dbContext.ChangeRequests
                .Where(x => !x.Finalized)
                .CountAsync();
        }

        public async Task<TrooperChangeRequestData?> GetChangeRequestAsync(Guid id)
        {
            await using var _dbContext = _dbContextFactory.CreateDbContext();

            var item = await _dbContext.FindAsync<TrooperChangeRequestData>(id);

            if (item is null) return null;

            await _dbContext.Entry(item).Reference(e => e.ChangedFor).LoadAsync();

            return item;
        }

        public async Task<ResultBase> FinalizeChangeRequest(Guid requestId, bool approve, int finalizer, ClaimsPrincipal finalizerClaim)
        {
            await using var _dbContext = _dbContextFactory.CreateDbContext();

            var item = await _dbContext.FindAsync<TrooperChangeRequestData>(requestId);

            if (item is null) return new(false, new List<string>() { "No change request for the provided ID was found" });

            item.Approved = approve;
            item.Finalized = true;
            item.FinalizedById = finalizer;

            await _dbContext.SaveChangesAsync();

            if (approve)
            {
                if (item.HasChange(false))
                    return await ApplyChangeRequest(item.ChangedForId, item, finalizerClaim);
                else return new(true, null);
            }
            else return new(true, null);
        }

        private async Task<ResultBase> ApplyChangeRequest(int toEdit, TrooperChangeRequestData data, ClaimsPrincipal finalizer)
        {
            await using var _dbContext = _dbContextFactory.CreateDbContext();

            var user = await _dbContext.FindAsync<Trooper>(toEdit);

            if (user is null) return new(false, new List<string>() { "No user for the specified ID was found." });

            if (data.Rank is not null)
                user.Rank = data.Rank;
            if (data.RTORank is not null)
                user.RTORank = data.RTORank;
            if (data.MedicRank is not null)
                user.MedicRank = data.MedicRank;
            if (data.WarrantRank is not null)
                user.WarrantRank = data.WarrantRank;
            if (data.WardenRank is not null)
                user.WardenRank = data.WardenRank;
            if (data.PilotRank is not null)
                user.PilotRank = data.PilotRank;

            if (data.Role is not null)
                user.Role = data.Role.Value;
            if (data.Team is not null)
                user.Team = data.Team;
            if (data.Flight is not null)
                user.Flight = data.Flight;
            if (data.Slot is not null)
                user.Slot = data.Slot.Value;

            if (data.Qualifications != Qualification.None)
                user.Qualifications = data.Qualifications;

            if (data.LastPromotion is not null)
                user.LastPromotion = data.LastPromotion.Value;
            if (data.StartOfService is not null)
                user.StartOfService = data.StartOfService.Value;
            if (data.LastBilletChange is not null)
                user.LastBilletChange = data.LastBilletChange.Value;
            if (data.GraduatedBCTOn is not null)
                user.GraduatedBCTOn = data.GraduatedBCTOn.Value;
            if (data.GraduatedUTCOn is not null)
                user.GraduatedUTCOn = data.GraduatedUTCOn.Value;

            user.LastUpdate = DateTime.UtcNow;

            return await _roster.UpdateAsync(user, new(), new(), finalizer);
        }
    }
}
