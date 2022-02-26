using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.JSInterop;
using ProjectDataCore;
using ProjectDataCore.Shared;
using ProjectDataCore.Components.Roster;
using ProjectDataCore.Components.Nav;
using ProjectDataCore.Components.Parts;
using ProjectDataCore.Components.Util;
using ProjectDataCore.Components.Framework.Component;
using ProjectDataCore.Data.Structures.Assignable.Configuration;
using ProjectDataCore.Data.Structures;
using ProjectDataCore.Data.Structures.Nav;
using ProjectDataCore.Data.Services.User;

namespace ProjectDataCore.Shared;
public partial class MainLayout
{
#pragma warning disable CS8618 // Injections are never null.

    [Inject]
    public IUserService UserService { get; set; }

    [Inject]
    public IAssignableDataService AssignableDataService { get; set; }

    [Inject]
    public ILocalUserService LocalUserService { get; set; }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    [CascadingParameter]
    public Task<AuthenticationState>? AuthStateTask { get; set; }

    /// <summary>
    /// The currently signed in user.
    /// </summary>
    public DataCoreUser? ActiveUser { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        if (AuthStateTask is not null)
        {
            var user = (await AuthStateTask).User;
            var newUser = await UserService.GetUserFromClaimsPrinciaplAsync(user);
            var changed = ActiveUser?.Id != newUser?.Id;

            ActiveUser = newUser;

            if (ActiveUser is not null && changed)
            {

                _ = Task.Run(async () =>
                {
                    var res = await AssignableDataService.EnsureAssignableValuesAsync(ActiveUser);
                    if (!res.GetResult(out var err))
                    {
                        // TODO handle errors.
                    }
                });
            }

            if(ActiveUser is not null)
            {
                if (await LocalUserService.InitalizeIfDeinitalizedAsync(ActiveUser.Id))
                    LocalUserService.RegisterClaimsPrincipal(ref user);
            }
            else
            {
                LocalUserService.DeinitalizeIfInitalized();
            }
        }
    }
}
