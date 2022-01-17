using ProjectDataCore.Data.Account;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Services.Routing;

/// <summary>
/// A service for loading, unloading, and retreiving custom
/// user scopes for use with the <see cref="IRoutingService"/>.
/// </summary>
public interface IScopedUserService
{
    public void LoadUserScope(Guid host, ref DataCoreUser user);
    public void UnloadUserScope(Guid host);
    public void InitScope(Guid key);
    public List<DataCoreUser>? GetScopedUsers(Guid host);
    public void UnloadSingleUserFromScope(Guid host, ref DataCoreUser user);
    public void SetUserScope(Guid host, List<DataCoreUser> users);
    public List<Guid> GetAllActiveScopes();
}
