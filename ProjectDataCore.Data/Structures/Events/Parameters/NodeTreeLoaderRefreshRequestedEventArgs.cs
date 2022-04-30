using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Events.Parameters;
public class NodeTreeLoaderRefreshRequestedEventArgs : EventArgs
{
    public bool ReloadDraggables { get; init; }

    public NodeTreeLoaderRefreshRequestedEventArgs(bool reloadDraggables = true)
    {
        ReloadDraggables = reloadDraggables;
    }
}
