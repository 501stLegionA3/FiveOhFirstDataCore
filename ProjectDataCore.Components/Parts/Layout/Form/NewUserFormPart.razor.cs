using ProjectDataCore.Data.Services.Roster;
using ProjectDataCore.Data.Structures.Model.User;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Components.Parts.Layout.Form;

[LayoutComponent(Name = "New Account Form", Form = true)]
public partial class NewUserFormPart : FormLayoutBase
{
#pragma warning disable CS8618 // Assignables are never null.
    [Inject]
    public IAssignableDataService AssignableDataService { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public override async Task OnFormInitAsync()
    {
        var res = await AssignableDataService.GetMockUserWithAssignablesAsync();
        if (res.GetResult(out var user, out var err))
        {
            SelectedUsers.Add(user); 
            SetUserScope(SelectedUsers);
        }

        await base.OnFormInitAsync();
    }

    public override async Task OnSubmitAsync()
    {
        var model = new DataCoreUserEditModel();
        if (OnSubmitListeners.TryGetValue(0, out var listener))
        {
            List<Task> runners = new();
            foreach (var l in listener)
                runners.Add(l.Invoke(model));

            await Task.WhenAll(runners);
        }

        for (int i = 0; i < SelectedUsers.Count; i++)
        {
            var res = await UserService.CreateUserAsync(SelectedUsers[i], x =>
            {
                x.StaticValues = model.StaticValues;
                x.AssignableValues = model.AssignableValues;
                x.Slots = model.Slots;
            });

            if (!res.GetResult(out var err))
            {
                // TODO display errors.
                return;
            }
        }
    }

    public override async Task OnCancelAsync()
    {
        if (CallRefreshRequest is not null)
        {
            await CallRefreshRequest.Invoke();
        }
        else
        {
            SelectedUsers.Clear();
            var res = await AssignableDataService.GetMockUserWithAssignablesAsync();
            if (res.GetResult(out var user, out var err))
            {
                SelectedUsers.Add(user);
                SetUserScope(SelectedUsers);
            }
        }
    }
}
