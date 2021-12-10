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
public interface IRoutingService
{
    /// <summary>
    /// Gets the type of a component by its name.
    /// </summary>
    /// <param name="qualifiedName">The qualified name of the component.</param>
    /// <param name="forceUpdate">If true, this will get the newest version of the type data, and updates the cache.</param>
    /// <returns>The <see cref="Type"/> for the provided <paramref name="qualifiedName"/></returns>
    public Type GetComponentType(string qualifiedName, bool forceUpdate = false);
}
