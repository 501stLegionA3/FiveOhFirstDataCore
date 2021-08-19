using FiveOhFirstDataCore.Core.Account;
using FiveOhFirstDataCore.Core.Database;
using FiveOhFirstDataCore.Core.Structures;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Services
{
    public class EparService : IEparService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

        public EparService(IDbContextFactory<ApplicationDbContext> dbContextFactory)
            => (_dbContextFactory) = (dbContextFactory);

        public async Task<ResultBase> CreateChangeRequest(TrooperChangeRequestDataBase requestBase, int submitterId)
        {
            TrooperChangeRequestData? request = requestBase as TrooperChangeRequestData;

            if (request is null) return new(false, new List<string> { "The request object was null." });

            await using var _dbContext = _dbContextFactory.CreateDbContext();

            var trooper = await _dbContext.FindAsync<Trooper>(submitterId);

            // We can't add anything to a null item.
            if (trooper is null) return new(false, new List<string> { "There was no trooper found for the inputed ID." });

            trooper.TrooperChangeRequests.Add(request);

            await _dbContext.SaveChangesAsync();

            return new(true, null);
        }

        public Task<IReadOnlyList<TrooperChangeRequestData>> GetActiveChangeRequests()
        {
            throw new NotImplementedException();
        }
    }
}
