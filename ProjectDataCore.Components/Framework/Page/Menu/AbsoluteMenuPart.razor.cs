using ProjectDataCore.Data.Services.Bus.Scoped;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Components.Page.Menu;
public partial class AbsoluteMenuPart : ComponentBase, IDisposable
{
    private bool disposedValue;
#pragma warning disable CS8618 // Injections are never null.
    [Inject]
    public IScopedDataBus ScopedDataBus { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

#pragma warning disable CS8618 // Editor required is never null.
    [Parameter, EditorRequired]
    public RenderFragment Display { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [Parameter]
    public bool OpenMenu { get; set; } = false;
    private bool Opened { get; set; } = false;

    [CascadingParameter(Name = "MenuId")]
    public string Id { get; set; } = "";

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        if(OpenMenu && !Opened)
        {
            Opened = true;
            await ScopedDataBus.DisplayMenuAsync(this, new(Display, Id));
        }
        else if (!OpenMenu && Opened)
        {
            Opened = false;
            await ScopedDataBus.CloseMenuAsync(this, Id);
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                _ = Task.Run(async () => await ScopedDataBus.CloseMenuAsync(this, Id));
            }

            disposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~AbsoluteMenuPart()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
