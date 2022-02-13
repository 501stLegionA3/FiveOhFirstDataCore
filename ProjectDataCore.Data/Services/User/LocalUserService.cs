using ProjectDataCore.Data.Account;
using ProjectDataCore.Data.Services.Bus;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Services.User;
public class LocalUserService : ILocalUserService
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
    private readonly IDataBus _dataBus;
    
    private bool disposedValue;
    protected DataCoreUser? LocalUser { get; set; }

    public LocalUserService(IDbContextFactory<ApplicationDbContext> dbContextFactory, IDataBus dataBus)
    {
        _dbContextFactory = dbContextFactory;
        _dataBus = dataBus;
    }

    public async Task InitalizeAsync(Guid userId, ClaimsPrincipal principal)
    {
        // Reset the data.
        DeInitalize();

        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();
        var user = await _dbContext.Users
            .Where(x => x.Id == userId)
            .FirstOrDefaultAsync();

        LocalUser = user;

        // Do any other setup and event registration here.
        _dataBus.RegisterLocalUserService(this, principal);
    }

    public void DeInitalize()
    {
        _dataBus.UnregisterLocalUserService(this);
        LocalUser = null;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // dispose managed state (managed objects)
            }

            // free unmanaged resources (unmanaged objects) and override finalizer
            // set large fields to null
            disposedValue = true;
            LocalUser = null;

            _dataBus.UnregisterLocalUserService(this);
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
