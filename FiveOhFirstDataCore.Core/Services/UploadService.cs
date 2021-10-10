using FiveOhFirstDataCore.Data.Account;
using FiveOhFirstDataCore.Data.Structuresbase;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Data.Services
{
    public class UploadService : IUploadService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
        public UploadService(IDbContextFactory<ApplicationDbContext> dbContextFactory) => _dbContextFactory = dbContextFactory;

        public async Task UploadPFP(Trooper trooper, byte[] pfp)
        {
            await using var _dbContext = _dbContextFactory.CreateDbContext();
            var data = await _dbContext.FindAsync<Trooper>(trooper.Id);
            if (data is not null) data.PFP = pfp;
            await _dbContext.SaveChangesAsync();
        }
    }
}
