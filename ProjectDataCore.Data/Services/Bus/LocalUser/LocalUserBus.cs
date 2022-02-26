using ProjectDataCore.Data.Services.User;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Services.Bus;
public partial class DataBus : IDataBus
{
    protected ConcurrentDictionary<ClaimsPrincipal, ILocalUserService> LocalUsers { get; init; } = new();
    protected ConcurrentDictionary<ILocalUserService, ClaimsPrincipal> LocalUsersInverse { get; init; } = new();

    public ILocalUserService? GetLoaclUserServiceFromClaimsPrincipal(ClaimsPrincipal principal)
    {
        _ = LocalUsers.TryGetValue(principal, out var user);

        return user;
    }

    public void RegisterLocalUserService(ILocalUserService localUser, ref ClaimsPrincipal principal)
    {
        LocalUsers.TryAdd(principal, localUser);
        LocalUsersInverse.TryAdd(localUser, principal);
    }

    public void UnregisterLocalUserService(ILocalUserService localUser)
    {
        if(LocalUsersInverse.TryRemove(localUser, out var principal))
            LocalUsers.TryRemove(principal, out _);
    }
}
