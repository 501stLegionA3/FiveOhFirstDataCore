using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Components.Parts.Layout.Form;

public class FormLayoutBase : LayoutBase, IDisposable
{
    public List<DataCoreUser> SelectedUsers { get; set; } = new();

    private bool registeredScope = false;

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        InitScope();
    }

    public void InitScope()
    {
        if(ComponentData is not null)
        {
            registeredScope = true;
            ScopedUserService.InitScope(ComponentData.Key);
        }
    }

    protected void RegisterScope(ref DataCoreUser user)
    {
        if (ComponentData is not null)
        {
            registeredScope = true;
            ScopedUserService.LoadUserScope(ComponentData.Key, ref user);
        }

        _ = InvokeAsync(StateHasChanged);
    }

    protected void UnregisterScope(ref DataCoreUser user)
    {
        if(ComponentData is not null)
        {
            ScopedUserService.UnloadSingleUserFromScope(ComponentData.Key, ref user);
        }

        _ = InvokeAsync(StateHasChanged);
    }

    protected void SetUserScope(List<DataCoreUser> users)
    {
        if(ComponentData is not null)
        {
            registeredScope = true;
            ScopedUserService.SetUserScope(ComponentData.Key, users);
        }

        _ = InvokeAsync(StateHasChanged);
    }

    protected void OnFormValueChanged()
    {
        SetUserScope(SelectedUsers);
    }

    public void Dispose()
    {
        if (registeredScope && ComponentData is not null)
            ScopedUserService.UnloadUserScope(ComponentData.Key);
    }
}
