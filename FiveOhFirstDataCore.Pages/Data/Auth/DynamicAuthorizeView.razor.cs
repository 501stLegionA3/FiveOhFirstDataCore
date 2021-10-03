using FiveOhFirstDataCore.Data.Account;
using FiveOhFirstDataCore.Data.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Components.Data.Auth;
public partial class DynamicAuthorizeView
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [Inject]
    public IAuthorizationService AuthorizationService {  get; set; }
    [Inject]
    public IWebsiteSettingsService WebsiteSettings { get; set; }
    [Inject]
    public NavigationManager Navigation { get; set; }
    [Inject]
    ILogger<DynamicAuthorizeView> Logger { get; set; }
    [CascadingParameter]
    private Task<AuthenticationState> AuthStateTask { get; set; }
    [CascadingParameter]
    private Trooper? CurrentUser { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    [Parameter]
    public string? SectionName { get; set; }
    [Parameter]
    public bool IsPage { get; set; } = true;

    private string QualifiedName { get; set; } = "default";

    private AuthenticationState? AuthState { get; set; }

    private bool IsAuthorizing { get; set; } = true;
    private bool IsAuthorized { get; set; } = false;
    private bool FirstRun { get; set; } = true;

    protected override async Task OnInitializedAsync()
    {
        if(FirstRun)
            await base.OnInitializedAsync();

        if (AuthStateTask is not null)
        {
            AuthState = await AuthStateTask;
            var user = AuthState.User;

            QualifiedName = SectionName ?? "default";

            try
            {
                var authRes = await AuthorizationService.AuthorizeAsync(user, QualifiedName);

                IsAuthorized = authRes.Succeeded;
            }
            catch (Exception ex)
            {
                await WebsiteSettings.GetOrCreatePolicySectionAsync(QualifiedName);
                Logger.LogWarning(ex, $"No policy found. Generated authorization policy section for {QualifiedName}");
                IsAuthorized = false;
                if (FirstRun)
                {
                    FirstRun = false;
                    await OnInitializedAsync();
                }
            }
        }
        else
        {
            IsAuthorized = false;
        }

        IsAuthorizing = false;
    }
}
