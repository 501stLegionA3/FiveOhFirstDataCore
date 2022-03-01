using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Services.InternalAuth;
public class InternalAuthorizationService : IInternalAuthorizationService
{
    private ConcurrentDictionary<string, InternalAuthorizationType> Tokens { get; set; } = new();

    public bool CheckToken(string token, InternalAuthorizationType scope)
    {
        if (Tokens.TryGetValue(token, out var registeredScope))
            return scope == registeredScope;

        return false;
    }

    public void RegisterToken(string token, InternalAuthorizationType scope)
    {
        _ = Tokens.TryAdd(token, scope);
    }

    public void UnregisterToken(string token)
    {
        _ = Tokens.TryRemove(token, out _);
    }
}
