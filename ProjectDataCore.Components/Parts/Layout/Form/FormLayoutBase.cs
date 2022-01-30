using ProjectDataCore.Data.Services.User;
using ProjectDataCore.Data.Structures.Model.User;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Components.Parts.Layout.Form;

public abstract class FormLayoutBase : LayoutBase, IDisposable
{
#pragma warning disable CS8618 // Inject is never null.
    [Inject]
    public IUserService UserService { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public List<DataCoreUser> SelectedUsers { get; set; } = new();

    protected ConcurrentDictionary<int, HashSet<Func<DataCoreUserEditModel, Task>>> OnSubmitListeners { get; private set; } = new();

    private bool registeredScope = false;

    public Func<Task> RefreshUserListAsync { get; set; }

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

    public void AddSubmitListener(int index, Func<DataCoreUserEditModel, Task> action)
    {
        if (!OnSubmitListeners.TryGetValue(index, out var actions))
        {
            actions = new();
            OnSubmitListeners[index] = actions;
        }

        actions.Add(action);
    }

    public void RemoveSubmitListener(int index, Func<DataCoreUserEditModel, Task> action)
    {
        if (OnSubmitListeners.TryGetValue(index, out var actions))
        {
            actions.Remove(action);
        }
    }

    public virtual async Task OnSubmitAsync()
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
            var res = await UserService.UpdateUserAsync(SelectedUsers[i].Id, x =>
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

    public virtual async Task OnCancelAsync()
    {
        SelectedUsers.Clear();
        SetUserScope(SelectedUsers);

        if (RefreshUserListAsync is not null)
            await RefreshUserListAsync.Invoke();
    }

    public void Dispose()
    {
        if (registeredScope && ComponentData is not null)
            ScopedUserService.UnloadUserScope(ComponentData.Key);
    }
}
