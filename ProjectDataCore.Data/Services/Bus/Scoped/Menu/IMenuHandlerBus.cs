using ProjectDataCore.Data.Structures.Events.Parameters;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Services.Bus.Scoped;
public partial interface IScopedDataBus
{
    public delegate Task DisplayMenuEventHandler(object sender, DisplayMenuEventArgs args);
    public event DisplayMenuEventHandler DisplayMenu;
    public Task DisplayMenuAsync(object sender, DisplayMenuEventArgs args);

    public delegate Task CloseMenuEventHanlder(object sender, string id);
    public event CloseMenuEventHanlder CloseMenu;
    public Task CloseMenuAsync(object sender, string id);

    public delegate Task MenuClosedEventArgs(object sender, DisplayMenuEventArgs args);
    public event MenuClosedEventArgs MenuClosed;
    public Task MenuClosedAsync(object sender, DisplayMenuEventArgs args);

    public delegate Task ReloadMenuEventHandler(object sender);
    public event ReloadMenuEventHandler ReloadMenu;
    public Task RequestMenuReload(object sender);
}
