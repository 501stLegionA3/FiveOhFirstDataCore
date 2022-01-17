using ProjectDataCore.Data.Account;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Services.Routing;

public class ScopedUserService : IScopedUserService
{
    private ConcurrentDictionary<Guid, List<DataCoreUser>> ScopedUsers { get; init; } = new();

    public List<DataCoreUser>? GetScopedUser(Guid host)
    {
        _ = ScopedUsers.TryGetValue(host, out var user);

        // We return null if it is not found, so no need for any if statements.
        return user;
    }

    public void LoadUserScope(Guid host, ref DataCoreUser user)
    {
        if(ScopedUsers.TryGetValue(host, out var set))
        {
            set.Add(user);
        }
        else
        {
            ScopedUsers[host] = new() { user };
        }
    }

    public void UnloadUserScope(Guid host)
    {
        _ = ScopedUsers.TryRemove(host, out _);
    }
}
