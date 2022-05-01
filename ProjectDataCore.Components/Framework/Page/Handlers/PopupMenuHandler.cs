using Microsoft.JSInterop;

using ProjectDataCore.Data.Services.Bus.Scoped;
using ProjectDataCore.Data.Structures.Events.Parameters;
using ProjectDataCore.Data.Structures.Page.ContextMenu;

using System.Collections.Concurrent;

namespace ProjectDataCore.Components.Framework.Page.Handlers;
public partial class PopupMenuHandler : ComponentBase, IDisposable
{
#pragma warning disable CS8618 // Injections are never null.
    [Inject]
    public IScopedDataBus ScopedDataBus { get; set; }
    [Inject]
    public IJSRuntime JSRuntime { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    private Type? OpenedMenu { get; set; } = null;
    private ConcurrentDictionary<string, object?> MenuParams { get; set; } = new();

    protected async override Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        
        ScopedDataBus.PopupMenuOpenRequested += OpenContextMenu;
    }

    private void OpenContextMenu(object sender, PopupMenuOpenRequestEventArgs args)
    {
        _ = Task.Run(async () => await HandleContextMenu(args));
    }

    private async Task HandleContextMenu(PopupMenuOpenRequestEventArgs args)
    {
        // Make sure this is a proper subclass.
        if (!args.MenuType.IsSubclassOf(typeof(PopupMenuBase)))
            return;
        
        // Display our custom context menus.
        OpenedMenu = args.MenuType;

        MenuParams.Clear();

        MenuParams[nameof(PopupMenuBase.Content)] = args.Fragment;
        MenuParams[nameof(PopupMenuBase.EventArgs)] = args.EventArgs;

        await InvokeAsync(StateHasChanged);
    }

    #region Dispose
#nullable disable
    private bool disposedValue;
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)

                ScopedDataBus.PopupMenuOpenRequested -= OpenContextMenu;
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
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
    #endregion
}
