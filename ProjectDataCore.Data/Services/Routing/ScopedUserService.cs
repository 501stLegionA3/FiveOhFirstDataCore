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

    public List<Guid> GetAllActiveScopes()
    {
        List<Guid> activeScopes = new();
        foreach (var s in ScopedUsers)
            activeScopes.Add(s.Key);

        return activeScopes;
    }

    public List<DataCoreUser>? GetScopedUsers(Guid host)
    {
        _ = ScopedUsers.TryGetValue(host, out var user);

        // We return null if it is not found, so no need for any if statements.
        return user;
    }

    public void InitScope(Guid key)
    {
        if (!ScopedUsers.TryGetValue(key, out _))
            ScopedUsers[key] = new();
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

    public void SetUserScope(Guid host, List<DataCoreUser> users)
    {
        ScopedUsers[host] = users;
    }

    public void UnloadSingleUserFromScope(Guid host, ref DataCoreUser user)
    {
        if(ScopedUsers.TryGetValue(host, out var set))
        {
            set.Remove(user);
        }
    }

    public void UnloadUserScope(Guid host)
    {
        _ = ScopedUsers.TryRemove(host, out _);
    }
}
