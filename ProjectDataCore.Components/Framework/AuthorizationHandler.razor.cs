using ProjectDataCore.Data.Services.Alert;
using ProjectDataCore.Data.Services.Policy;
using ProjectDataCore.Data.Structures.Policy;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Components.Framework;
public partial class AuthorizationHandler
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [Inject]
    public IPolicyService PolicyService { get; set; }
    [Inject]
    public IAlertService AlertService { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    [Parameter]
    public RenderFragment Authorized { get; set; }
    [Parameter]
    public RenderFragment? NotAuthorized { get; set; }
    [Parameter]
    public PageComponentSettingsBase? Settings { get; set; }
    [Parameter]
    public bool ForceAdmin { get; set; } = false;

    [CascadingParameter(Name = "PageEdit")]
    public bool Editing { get; set; } = false;

    private string AuthPolicy { get; set; } = "";
    private DynamicAuthorizationPolicy? AdminPagePolicy { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        if (!Editing)
        {
            if (ForceAdmin)
            {
                if (AdminPagePolicy is null)
                {
                    var res = await PolicyService.GetAdminPagePolicy();
                    if (res.GetResult(out var dynamPolicy, out var err))
                    {
                        AdminPagePolicy = dynamPolicy;
                        AuthPolicy = AdminPagePolicy.Key.ToString();
                    }
                    else
                    {
                        AlertService.CreateErrorAlert(err);
                    }
                }
                else
                {
                    AuthPolicy = AdminPagePolicy.Key.ToString();
                }
            }
            else if (Settings?.AuthorizationPolicyKey is not null)
            {
                AuthPolicy = Settings.AuthorizationPolicyKey.Value.ToString();
            }
        }
    }
}
