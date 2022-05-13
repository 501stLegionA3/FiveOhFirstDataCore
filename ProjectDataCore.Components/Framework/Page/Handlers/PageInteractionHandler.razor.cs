using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

using ProjectDataCore.Data.Services.Bus.Scoped;
using ProjectDataCore.Data.Structures.Events.Parameters;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Components.Framework.Page.Handlers;
public partial class PageInteractionHandler : ComponentBase, IDisposable
{
    private bool disposedValue;
#pragma warning disable CS8618 // Injections are never null.
    [Inject]
    public IScopedDataBus ScopedDataBus { get; set; }
    [Inject]
    public IJSRuntime JSRuntime { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

#pragma warning disable CS8618 // Editor required is never null.
    [Parameter, EditorRequired]
    public RenderFragment Content { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    private DotNetObjectReference<PageInteractionHandler>? DotNetRef { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            await JSRuntime.InvokeVoidAsync("Util.registerKeyDownHandler", GetDotNetReference(), nameof(OnKeyDownAsync));
        }
    }

    protected DotNetObjectReference<PageInteractionHandler> GetDotNetReference()
    {
        if (DotNetRef is null)
        {
            DotNetRef = DotNetObjectReference.Create(this);
        }

        return DotNetRef;
    }

    public async Task OnClickAsync(MouseEventArgs e)
        => await ScopedDataBus.SendPageClickEventAsync(this, new(e));

    [JSInvokable]
    public async Task OnKeyDownAsync(OnPressEventArgs e)
        => await ScopedDataBus.SendKeyPressEventAsync(this, e);

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await JSRuntime.InvokeVoidAsync("Util.removeKeyDownHandler", GetDotNetReference());
                    }
                    catch { 
                        // If this errors the event handler was removed anyways.
                    }
                });
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
