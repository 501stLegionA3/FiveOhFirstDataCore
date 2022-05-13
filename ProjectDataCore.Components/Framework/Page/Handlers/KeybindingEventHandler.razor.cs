using Microsoft.AspNetCore.Components.Web;

using ProjectDataCore.Data.Services.Bus.Scoped;
using ProjectDataCore.Data.Services.Keybindings;
using ProjectDataCore.Data.Structures.Events.Parameters;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Components.Framework.Page.Handlers;
public partial class KeybindingEventHandler : IDisposable
{
    private bool disposedValue;
#pragma warning disable CS8618 // Injections are never null.
    [Inject]
    public IScopedDataBus ScopedDataBus { get; set; }
    [Inject]
    public IKeybindingService KeybindingService { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    private string LastKeybinding { get; set; }
    private bool ClosePopup { get; set; } = false;

    private Timer? ClosePopupTimer { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        ScopedDataBus.KeyPressed += ScopedDataBus_KeyPressed;
    }

    private Task ScopedDataBus_KeyPressed(object sender, OnPressEventArgs args)
    {
        _ = Task.Run(async () =>
        {
            var res = await KeybindingService.ExecuteKeybindingAsync(args);

            if(res is not null)
            {
                LastKeybinding = res.Value.AsFull();
                ClosePopup = false;

                if (ClosePopupTimer is null)
                    ClosePopupTimer = new(async (x) =>
                    {
                        ClosePopup = true;
                        await InvokeAsync(StateHasChanged);
                    }, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);

                ClosePopupTimer.Change(TimeSpan.FromSeconds(3), Timeout.InfiniteTimeSpan);
            }
        });

        return Task.CompletedTask;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                ScopedDataBus.KeyPressed -= ScopedDataBus_KeyPressed;
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
