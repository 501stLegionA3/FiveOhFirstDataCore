using ProjectDataCore.Data.Account;
using ProjectDataCore.Data.Services.Bus;
using ProjectDataCore.Data.Structures.Policy;

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
    private bool initalizeValue;
    protected DataCoreUser? LocalUser { get; set; }

    public LocalUserService(IDbContextFactory<ApplicationDbContext> dbContextFactory, IDataBus dataBus)
    {
        _dbContextFactory = dbContextFactory;
        _dataBus = dataBus;
    }

    public async Task InitalizeAsync(Guid userId)
    {
        // Reset the data.
        Deinitalize();

        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();
        var user = await _dbContext.Users
            .Where(x => x.Id == userId)
            .FirstOrDefaultAsync();

        LocalUser = user;
    }

    public void Deinitalize()
    {
        _dataBus.UnregisterLocalUserService(this);
        LocalUser = null;
    }

    public async Task<bool> InitalizeIfDeinitalizedAsync(Guid userId)
    {

        if (!initalizeValue)
        {
            initalizeValue = true;
            await InitalizeAsync(userId);
            return true;
        }

        return false;
    }

    public void DeinitalizeIfInitalized()
    {
        if (initalizeValue)
        {
            initalizeValue = false;
            Deinitalize();
        }
    }
    
    public bool ValidateWithPolicy(DynamicAuthorizationPolicy policy)
    {
        if (LocalUser is null)
            return false;

        return policy.Validate(LocalUser);
    }

    public void RegisterClaimsPrincipal(ref ClaimsPrincipal principal)
    {
        if (LocalUser is not null)
        {
            // Do any other setup and event registration here.
            _dataBus.RegisterLocalUserService(this, ref principal);
        }
        else
        {
            initalizeValue = false;
        }
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

            Deinitalize();
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
