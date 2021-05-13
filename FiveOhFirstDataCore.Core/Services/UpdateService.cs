using FiveOhFirstDataCore.Core.Database;
using FiveOhFirstDataCore.Core.Structures.Updates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Services
{
    public class UpdateService : IUpdateService
    {
        private readonly ApplicationDbContext _dbContext;

        public UpdateService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<List<UpdateBase>> GetRosterUpdatesAsync()
        {
            throw new NotImplementedException();
        }
    }
}
