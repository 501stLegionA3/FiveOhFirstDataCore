using FiveOhFirstDataCore.Data.Services;

using Microsoft.AspNetCore.Components;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Components.Data.Auth;
public partial class DynamicAuthorizeViewTools
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [Inject]
    public IAlertService AlertService { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [Parameter]
    public string? SectionName { get; set; }
    [Parameter]
    public RenderFragment? EditPolicySection { get; set; }
    private string BoundingCssClass { get; set; } = "";
    private string EditPolicy { get; set; } = "Require Manager";

    private void CoatArea()
    {
        BoundingCssClass = "coat";
    }

    private void UncoatArea()
    {
        BoundingCssClass = "fade";
    }

    private void DisplayModal()
    {
        if (EditPolicySection is not null)
            AlertService.PostModal(this, EditPolicySection);
    }
}
