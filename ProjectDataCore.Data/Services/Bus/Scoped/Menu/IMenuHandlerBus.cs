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

    public delegate Task CloseMenuEventHanlder(object sender);
    public event CloseMenuEventHanlder CloseMenu;
    public Task CloseMenuAsync(object sender);
}
