using ProjectDataCore.Data.Services.Bus.Scoped;
using ProjectDataCore.Data.Structures.Events.Parameters;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Components.Framework.Page.Handlers;
public partial class AbsoluteMenuPopupHandler : ComponentBase, IDisposable
{
    private bool disposedValue;
#pragma warning disable CS8618 // Injections are never null.
    [Inject]
    public IScopedDataBus ScopedDataBus { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    private ConcurrentDictionary<object, RenderFragment> Menus { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        ScopedDataBus.DisplayMenu += ScopedDataBus_DisplayMenu;
        ScopedDataBus.CloseMenu += ScopedDataBus_CloseMenu;
    }

    private Task ScopedDataBus_DisplayMenu(object sender, DisplayMenuEventArgs args)
    {
        _ = Task.Run(async () =>
        {
            Menus[sender] = args.Menu;

            await InvokeAsync(StateHasChanged);
        });

        return Task.CompletedTask;
    }

    private Task ScopedDataBus_CloseMenu(object sender)
    {
        _ = Task.Run(async () =>
        {
            _ = Menus.TryRemove(sender, out _);

            await InvokeAsync(StateHasChanged);
        });

        return Task.CompletedTask;
    }

#nullable disable
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                ScopedDataBus.DisplayMenu -= ScopedDataBus_DisplayMenu;
                ScopedDataBus.CloseMenu -= ScopedDataBus_CloseMenu;
            }

            ScopedDataBus = null;
            Menus = null;
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
#nullable enable
}
