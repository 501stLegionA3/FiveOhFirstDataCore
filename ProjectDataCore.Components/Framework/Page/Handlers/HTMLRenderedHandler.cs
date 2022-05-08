using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Components.Framework.Page.Handlers;
public class HTMLRenderedHandler : ComponentBase
{
    [Parameter]
    public Func<Task>? OnRender { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender && OnRender is not null)
        {
            await OnRender.Invoke();
        }
    }
}
