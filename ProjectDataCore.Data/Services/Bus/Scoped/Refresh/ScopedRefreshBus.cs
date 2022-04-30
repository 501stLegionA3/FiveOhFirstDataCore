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

    public void RequestLayoutNodeTreeRefresh(object sender, NodeTreeLoaderRefreshRequestedEventArgs args)
    {
        if (NodeTreeLoaderRefreshRequested is not null)
            NodeTreeLoaderRefreshRequested.Invoke(sender, args);
    }
}
