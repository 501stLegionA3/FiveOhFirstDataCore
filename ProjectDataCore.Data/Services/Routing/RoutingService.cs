using ProjectDataCore.Data.Structures.Page;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Services.Routing;

public class RoutingService : IRoutingService
{
    public class RoutingServiceSettings
    {
        public Assembly Assembly { get; init; }

        public RoutingServiceSettings(Assembly asm)
            => Assembly = asm;
    }

    private ConcurrentDictionary<string, Type> ComponentCache { get; init; }
    private Assembly ComponentAssembly { get; init; }

    public RoutingService(RoutingServiceSettings settings)
    {
        ComponentCache = new ConcurrentDictionary<string, Type>();
        ComponentAssembly = settings.Assembly;
    }

    public Type GetComponentType(string qualifiedName, bool forceUpdate = false)
    {
        if (forceUpdate || !ComponentCache.TryGetValue(qualifiedName, out Type? type))
        {
            type = ComponentAssembly.GetType(qualifiedName);

            if (type is null)
                throw new ArgumentException("The provided qualified name was not found in the components assembly.", 
                    nameof(qualifiedName));

            ComponentCache[qualifiedName] = type;
        }

        return type;
    }

    public Task<CustomPageSettings> GetPageSettingsFromRouteAsync(string route)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<bool> LoadPageSettingsAsync(CustomPageSettings settings)
    {
        throw new NotImplementedException();
    }
}
