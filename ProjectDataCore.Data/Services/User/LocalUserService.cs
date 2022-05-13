using ProjectDataCore.Data.Account;
using ProjectDataCore.Data.Services.Bus;
using ProjectDataCore.Data.Services.Bus.Global;
using ProjectDataCore.Data.Structures.Events.Parameters;
using ProjectDataCore.Data.Structures.Keybindings;
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
    private readonly IGlobalDataBus _dataBus;
    
    private bool disposedValue;
    private bool initalizedValue;
    protected DataCoreUser? LocalUser { get; set; }

    public LocalUserService(IDbContextFactory<ApplicationDbContext> dbContextFactory, IGlobalDataBus dataBus)
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
            .Include(x => x.KeyBindings)
            .FirstOrDefaultAsync();

        LocalUser = user;

        if(LocalUser is not null)
        {
            _dataBus.RegisterLocalUserService(this, LocalUser.Id);
        }
        else
        {
            initalizedValue = false;
        }
    }

    public void Deinitalize()
    {
        _dataBus.UnregisterLocalUserService(this);
        LocalUser = null;
    }

    public async Task<bool> InitalizeIfDeinitalizedAsync(Guid userId)
    {

        if (!initalizedValue)
        {
            initalizedValue = true;
            await InitalizeAsync(userId);
            return true;
        }

        return false;
    }

    public void DeinitalizeIfInitalized()
    {
        if (initalizedValue)
        {
            initalizedValue = false;
            Deinitalize();
        }
    }
    
    public bool ValidateWithPolicy(DynamicAuthorizationPolicy policy)
    {
        if (LocalUser is null)
            return false;

        return policy.Validate(LocalUser);
    }

    public Dictionary<OnPressEventArgs, Keybinding> GetCustomKeybindings()
    {
        if (LocalUser is null)
            return new();

        Dictionary<OnPressEventArgs, Keybinding> data = new();
        foreach (var binding in LocalUser.KeyBindings)
        {
            data.Add(binding.GetMinimalKeyboardEventArgs(), binding.Keybinding);
        }

        return data;
    }

    public async Task ReloadKeybindingsAsync()
    {
        if (LocalUser is null)
            return;

        var _dbContext = await _dbContextFactory.CreateDbContextAsync();
        var entity = _dbContext.Attach(LocalUser);
        await entity.Collection(e => e.KeyBindings).LoadAsync();
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
