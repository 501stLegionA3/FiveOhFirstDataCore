using ProjectDataCore.Data.Structures.Events.Parameters;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Services.Bus.Scoped;
public partial interface IScopedDataBus
{
    public delegate Task NodeTreeLoaderRefreshRequestedEventHandler(object sender, NodeTreeLoaderRefreshRequestedEventArgs args);
    public event NodeTreeLoaderRefreshRequestedEventHandler NodeTreeLoaderRefreshRequested;
    public Task RequestLayoutNodeTreeRefreshAsync(object sender, NodeTreeLoaderRefreshRequestedEventArgs args);


}
