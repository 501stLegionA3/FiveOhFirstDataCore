using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Components.Framework;
public partial class AuthorizationHandler
{
    [Parameter]
    public RenderFragment Authorized { get; set; }
    [Parameter]
    public RenderFragment? NotAuthorized { get; set; }
    [Parameter]
    public PageComponentSettingsBase? Settings { get; set; }
    [Parameter]
    public bool ForceAdmin { get; set; } = false;

    private string AuthPolicy { get; set; } = "";

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        if(ForceAdmin)
        {
            AuthPolicy = "internal-admin-policy";
        }
        else if (Settings?.AuthorizationPolicyKey is not null)
        {
            AuthPolicy = Settings.AuthorizationPolicyKey.Value.ToString();
        }
    }
}
