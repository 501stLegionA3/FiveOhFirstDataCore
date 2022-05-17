using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Components.Parts.Layout.Form;

[LayoutComponent(Name = "Current User Form", Form = true)]
public partial class CurrentUserFormPart : FormLayoutBase
{
    public override Task OnFormInitAsync()
    {
        base.OnFormInitAsync();
        if (ActiveUser is not null)
        {
            SelectedUsers.Add(ActiveUser);
            SetUserScope(SelectedUsers);
        }

        return Task.CompletedTask;
    }

    public override async Task OnCancelAsync()
    {
        SelectedUsers.Clear();
        if (ActiveUser is not null)
        {
            SelectedUsers.Add(ActiveUser);
            SetUserScope(SelectedUsers);
        }

        if (CallRefreshRequest is not null)
            await CallRefreshRequest.Invoke();
    }
}
