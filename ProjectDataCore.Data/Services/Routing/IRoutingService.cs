using ProjectDataCore.Data.Structures.Page;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Services.Routing;

/// <summary>
/// Handles custom routing and delivering route information
/// to the page generator.
/// </summary>
/// <remarks>
/// Due to the nature of loading larger page values, this service
/// should be run as a singleton so the cache systems will operate
/// properly accross all users.
/// </remarks>
public interface IRoutingService
{
    /// <summary>
    /// Gets the type of a component by its name.
    /// </summary>
    /// <param name="qualifiedName">The qualified name of the component.</param>
    /// <param name="forceUpdate">If true, this will get the newest version of the type data, and updates the cache.</param>
    /// <returns>The <see cref="Type"/> for the provided <paramref name="qualifiedName"/></returns>
    public Type GetComponentType(string qualifiedName, bool forceUpdate = false);

    /// <summary>
    /// Gets the top level page settings object from a provided route.
    /// </summary>
    /// <remarks>
    /// To reduce load times, this method will only return the <see cref="CustomPageSettings"/> and
    /// none of the connected entities. To load the rest of the settings object pass the returned 
    /// object into the <see cref="LoadPageSettingsAsync(CustomPageSettings)"/> method.
    /// </remarks>
    /// <param name="route">The route to get a settings object for.</param>
    /// <returns>A <see cref="CustomPageSettings"/> object if the route is valid, otherwise null.</returns>
    public Task<CustomPageSettings?> GetPageSettingsFromRouteAsync(string route);

    /// <summary>
    /// Loads the full settings for a page.
    /// </summary>
    /// <param name="settings">The page settings object to load.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}"/> where the boolean value is true.</returns>
    public IAsyncEnumerable<bool> LoadPageSettingsAsync(CustomPageSettings settings);
}
