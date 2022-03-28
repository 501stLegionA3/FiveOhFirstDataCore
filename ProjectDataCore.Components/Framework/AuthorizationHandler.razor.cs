using Microsoft.AspNetCore.Components.Authorization;
using ProjectDataCore.Data.Services.Alert;
using ProjectDataCore.Data.Services.Policy;
using ProjectDataCore.Data.Services.User;
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
    [Inject]
    public IUserService UserService { get; set; }

    [Parameter]
    public RenderFragment Authorized { get; set; }

    [CascadingParameter]
    public Task<AuthenticationState> AuthStateTask { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    [Parameter]
    public RenderFragment? NotAuthorized { get; set; }
    [Parameter]
    public PageComponentSettingsBase? Settings { get; set; }

    [CascadingParameter(Name = "PageEdit")]
    public bool Editing { get; set; } = false;

    private bool IsAuthorized { get; set; } = false;

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        if (!Editing)
        {
            var userClaim = (await AuthStateTask).User;

            if (Settings is not null)
                IsAuthorized = await UserService.AuthorizeUserAsync(userClaim, Settings);
            else
                IsAuthorized = false;
        }
        else
        {
            IsAuthorized = true;
        }
    }
}
