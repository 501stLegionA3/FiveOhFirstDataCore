using ProjectDataCore.Data.Structures.Events.Parameters;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Services.Bus.Scoped;
public partial class ScopedDataBus : IScopedDataBus
{
    public event IScopedDataBus.NodeTreeLoaderRefreshRequestedEventHandler? NodeTreeLoaderRefreshRequested;

    public Task RequestLayoutNodeTreeRefreshAsync(object sender, NodeTreeLoaderRefreshRequestedEventArgs args)
    {
        if (NodeTreeLoaderRefreshRequested is not null)
            _ = Task.Run(async () => await NodeTreeLoaderRefreshRequested.Invoke(sender, args));

        return Task.CompletedTask;
    }
}
