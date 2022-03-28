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
    protected ConcurrentDictionary<string, ILocalUserService> LocalUsers { get; init; } = new();
    protected ConcurrentDictionary<ILocalUserService, string> LocalUsersInverse { get; init; } = new();

    public ILocalUserService? GetLoaclUserServiceFromClaimsPrincipal(ClaimsPrincipal principal)
    {
        var princName = principal.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(princName))
            return null;

        _ = LocalUsers.TryGetValue(princName, out var user);

        return user;
    }

    public void RegisterLocalUserService(ILocalUserService localUser, Guid userId)
    {
        var name = userId.ToString();

        LocalUsers.TryAdd(name, localUser);
        LocalUsersInverse.TryAdd(localUser, name);
    }

    public void UnregisterLocalUserService(ILocalUserService localUser)
    {
        if(LocalUsersInverse.TryRemove(localUser, out var principal))
            LocalUsers.TryRemove(principal, out _);
    }
}
