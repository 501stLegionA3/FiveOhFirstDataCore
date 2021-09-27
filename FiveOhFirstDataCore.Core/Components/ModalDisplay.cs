using FiveOhFirstDataCore.Data.Services;

using Microsoft.AspNetCore.Components;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Data.Components;

public class ModalDisplay : ComponentBase
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [Inject]
    public IAlertService AlertService { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    [Parameter]
    public bool Display { get; set; } = false;
    [Parameter]
    public RenderFragment? ToDisplay { get; set; }


    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        if(Display && ToDisplay is not null)
        {
            AlertService.PostModal(this, ToDisplay);
        }
    }
}
