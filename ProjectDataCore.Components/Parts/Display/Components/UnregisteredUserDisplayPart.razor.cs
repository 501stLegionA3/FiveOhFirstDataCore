using ProjectDataCore.Data.Services.Roster;
using ProjectDataCore.Data.Services.User;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Components.Parts.Display.Components;

[DisplayComponent(Name = "Unregistered Users")]
public partial class UnregisteredUserDisplayPart : DisplayBase
{
#pragma warning disable CS8618 // Injections are not null.
    [Inject]
    public IAssignableDataService AssignableService { get; set; }
    [Inject]
    public IUserService UserService { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    private DataCoreUser MockUser { get; set; }
    private List<DataCoreUser> Users { get; set; } = new();
    private List<(string, string)> DisplayValues { get; set; } = new();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if(firstRender)
        {
            var res = await AssignableService.GetMockUserWithAssignablesAsync();

            if(res.GetResult(out var usr, out var err))
            {
                MockUser = usr;
            }
            else
            {
                // TODO handle errors
            }

            _ = Task.Run(InitAsync);

            StateHasChanged();
        }
    }

    private async Task InitAsync()
    {
        if (ComponentData is not null)
        {
            Users = await UserService.GetAllUnregisteredUsersAsync();

            foreach(var u in Users)
            {
                string val;
                if(ComponentData.StaticProperty)
                {
                    val = u.GetStaticProperty(ComponentData.PropertyToEdit, ComponentData.FormatString);
                }
                else
                {
                    val = u.GetAssignableProperty(ComponentData.PropertyToEdit, ComponentData.FormatString);
                }

                DisplayValues.Add((val, u.AccessCode ?? ""));

                await InvokeAsync(StateHasChanged);
            }
        }
    }

    protected Type[] AllowedStaticTypes { get; set; } = new Type[]
	{
		typeof(string)
	};
}
