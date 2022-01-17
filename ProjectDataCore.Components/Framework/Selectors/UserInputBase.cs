using ProjectDataCore.Data.Services.User;
using ProjectDataCore.Data.Structures.Selector.User;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Components.Framework.Selectors;

public class UserInputBase : ComponentBase
{

#pragma warning disable CS8618 // Injections are never null.
    [Inject]
    public IUserService UserService { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    [Parameter]
    public List<DataCoreUser> SelectedUsers { get; set; } = new();
    public List<DataCoreUser> SuggestedUsers { get; set; } = new();
    public List<DataCoreUser> AllUsers { get; set; } = new();

    public string SearchRaw { get; set; } = "";

    [Parameter]
    public UserSelectComponentSettings? Settings { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            AllUsers = await UserService.GetAllUsersAsync();
        }
    }

    protected void OnSearchUpdated(string? value)
    {
        SearchRaw = value ?? "";

        _ = Task.Run(async () =>
        {
            var set = AllUsers.Where(x => GetClosenessMatch(SearchRaw, x));
            SuggestedUsers.Clear();

            foreach (var i in set)
            {
                SuggestedUsers.Add(i);
                await InvokeAsync(StateHasChanged);
            }
        });
    }

    protected void AddSelectedUser(DataCoreUser user)
    {
        SelectedUsers.Add(user);
        SearchRaw = "";

        StateHasChanged();
    }

    protected bool GetClosenessMatch(string search, DataCoreUser user)
    {
        if (Settings is not null && Settings.Properties.Count > 0)
        {
            for (int i = 0; i < Settings.Properties.Count; i++)
            {
                string val;
                if (Settings.IsStaticList[i])
                    val = user.GetStaticProperty(Settings.Properties[i], Settings.Formats[i]);
                else
                    val = user.GetAssignableProperty(Settings.Properties[i], Settings.Formats[i]);

                if (val.StartsWith(search, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

        return user.UserName.StartsWith(search, StringComparison.OrdinalIgnoreCase);
    }

    protected void OnRemoveSelection(DataCoreUser user)
    {
        SelectedUsers.Remove(user);

        StateHasChanged();
    }

    protected string GetDisplayValue(DataCoreUser user)
    {
        if (Settings is not null && Settings.Properties.Count > 0)
        {
            List<string> values = new();
            for (int i = 0; i < Settings.Properties.Count; i++)
            {
                string val;
                if (Settings.IsStaticList[i])
                    val = user.GetStaticProperty(Settings.Properties[i], Settings.Formats[i]);
                else
                    val = user.GetAssignableProperty(Settings.Properties[i], Settings.Formats[i]);

                values.Add(val);
            }

            return string.Join(" ", values);
        }

        return user.UserName;
    }
}
