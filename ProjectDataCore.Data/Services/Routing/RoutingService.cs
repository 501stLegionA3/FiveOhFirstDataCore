using ProjectDataCore.Data.Structures.Page;
using ProjectDataCore.Data.Structures.Page.Components;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
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

    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

    private ConcurrentDictionary<string, Type> ComponentCache { get; init; }
    private Assembly ComponentAssembly { get; init; }

    public RoutingService(RoutingServiceSettings settings, IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;

        ComponentCache = new ConcurrentDictionary<string, Type>();
        ComponentAssembly = settings.Assembly;
    }

    public Type GetComponentType(string qualifiedName, bool forceUpdate = false)
    {
        // If there is an update requested of the item is not in the cache...
        if (forceUpdate || !ComponentCache.TryGetValue(qualifiedName, out Type? type))
        {
            // ... then try to get the type from the components assembly...
            type = ComponentAssembly.GetType(qualifiedName);
            // ... if it does not exist, throw an error ...
            if (type is null)
                throw new MissingComponentException("The provided qualified name was not found in the components assembly.");
            // ... otherwise add it to the cache ...
            ComponentCache[qualifiedName] = type;
        }
        // ... then return the type.
        return type;
    }

    public async Task<CustomPageSettings?> GetPageSettingsFromRouteAsync(string route)
    {
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();
        // Find the page settings for the provided route.
        var pageSettings = await _dbContext.CustomPageSettings
            .Where(x => x.Route == route)
            .FirstOrDefaultAsync();

        return pageSettings;
    }

    public async IAsyncEnumerable<bool> LoadPageSettingsAsync(CustomPageSettings settings)
    {
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();
        // attach the settings object...
        var obj = _dbContext.Attach(settings);
        // ... then load the base layout ...
        await obj.Reference(e => e.Layout).LoadAsync();
        // ... if the layout is not null ...
        if (settings.Layout is not null)
        {
            // ... yield the first value so the display can be refreshed ...
            yield return true;

            // ... then the base layout to the level queue ...
            Queue<LayoutComponentSettings> level = new();
            level.Enqueue(settings.Layout);

            // ... and while we have level data ...
            while (level.TryDequeue(out var levelItem))
            {
                // ... attach the level item ...
                var layoutObj = _dbContext.Attach(levelItem);
                // ... and load its direct children ...
                var children = layoutObj.Collection(x => x.ChildComponents).Query()
                    .Include(x => x.ParentLayout)
                    .AsAsyncEnumerable();

                // ... then for each child ...
                await foreach (var child in children)
                {
                    // ... notify that it has been loaded ...
                    yield return true;

                    // ... and if the child is a layout component, enqueue it.
                    if (child is LayoutComponentSettings childLayout)
                        level.Enqueue(childLayout);
                }
            }
        }

        // return one last time for loading purposes.
        yield return true;
    }
}

/// <summary>
/// Exception for when a component type can not be loaded.
/// </summary>
public class MissingComponentException : Exception
{
    public MissingComponentException() { }
    public MissingComponentException(string? message) : base(message) { }
    public MissingComponentException(string? message, Exception? innerException) : base(message, innerException) { }
    protected MissingComponentException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}