using Microsoft.AspNetCore.Components.Web;

using ProjectDataCore.Data.Services.Bus.Scoped;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Components.Framework.Page.Handlers;
public partial class PageClickHandler : ComponentBase
{
#pragma warning disable CS8618 // Injections are never null.
    [Inject]
    public IScopedDataBus ScopedDataBus { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    [Parameter]
    public RenderFragment Content { get; set; }

    public async Task OnClickAsync(MouseEventArgs e)
    {
        await ScopedDataBus.SendPageClickEventAsync(this, new(e));
    }
}
