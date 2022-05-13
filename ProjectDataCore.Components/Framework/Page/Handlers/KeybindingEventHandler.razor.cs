using ProjectDataCore.Data.Services.Bus.Scoped;

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
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        ScopedDataBus.KeyPressed += ScopedDataBus_KeyPressed;
    }

    private Task ScopedDataBus_KeyPressed(object sender, Microsoft.AspNetCore.Components.Web.KeyboardEventArgs args)
    {
        throw new NotImplementedException();
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
