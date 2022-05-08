using ProjectDataCore.Data.Structures.Events.Parameters;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Services.Bus.Scoped;
public partial class ScopedDataBus : IScopedDataBus
{
    public event IScopedDataBus.DisplayMenuEventHandler? DisplayMenu;
    public Task DisplayMenuAsync(object sender, DisplayMenuEventArgs args)
    {
        if(DisplayMenu is not null)
            _ = Task.Run(async () => await DisplayMenu.Invoke(sender, args));

        return Task.CompletedTask;
    }

    public event IScopedDataBus.CloseMenuEventHanlder? CloseMenu;
    public Task CloseMenuAsync(object sender)
    {
        if (CloseMenu is not null)
            _ = Task.Run(async () => await CloseMenu.Invoke(sender));

        return Task.CompletedTask;
    }
}
