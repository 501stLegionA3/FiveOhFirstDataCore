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
    public Task CloseMenuAsync(object sender, string id)
    {
        if (CloseMenu is not null)
            _ = Task.Run(async () => await CloseMenu.Invoke(sender, id));

        return Task.CompletedTask;
    }

    public event IScopedDataBus.MenuClosedEventArgs MenuClosed;
    public Task MenuClosedAsync(object sender, DisplayMenuEventArgs args)
    {
        if (MenuClosed is not null)
            _ = Task.Run(async () => await MenuClosed.Invoke(sender, args));

        return Task.CompletedTask;
    }

    public event IScopedDataBus.ReloadMenuEventHandler? ReloadMenu;
    public Task RequestMenuReload(object sender)
    {
        if (ReloadMenu is not null)
            _ = Task.Run(async () => await ReloadMenu.Invoke(sender));

        return Task.CompletedTask;
    }
}
