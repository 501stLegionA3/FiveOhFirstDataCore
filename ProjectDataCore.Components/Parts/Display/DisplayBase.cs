using ProjectDataCore.Data.Structures.Model.Page;

namespace ProjectDataCore.Components.Parts.Display;

public class DisplayBase : ParameterBase
{
    [Parameter]
    public DisplayComponentSettings? ComponentData { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        if (ComponentData is not null)
        {
            LoadScopedUser(ComponentData.UserScopeId);
            LoadParameterValue();

            UserScopeSelection = ComponentData.UserScopeId;
        }
    }

    protected override async Task RemoveCurrentDisplay()
    {
        if (ComponentData is not null)
        {
            var res = await PageEditService.DeleteDisplayComponentAsync(ComponentData.Key);

            if (CallRefreshRequest is not null)
                await CallRefreshRequest.Invoke();
        }
    }

    protected override void StartEdit()
	{
        if (ComponentData is not null)
        {
            EditModel = new(ComponentData);
            StateHasChanged();
        }
	}

    protected override void CancelEdit()
	{
        EditModel = null;
        StateHasChanged();
	}

    protected override async Task SaveEdit()
    {
        if(EditModel is not null)
		{
            var res = await PageEditService.UpdateDisplayComponentAsync(EditModel.Key, x =>
            {
                x.Label = Optional.FromValue(EditModel.Label);
                x.PropertyToEdit = EditModel.Property;
                x.StaticProperty = EditModel.StaticProperty;
                x.FormatString = Optional.FromValue(EditModel.FormatString);
                x.UserScope = Optional.FromValue(UserScopeSelection);
            });

            EditModel = null;

            if (CallRefreshRequest is not null)
                await CallRefreshRequest.Invoke();
		}
    }

    #region Parameter Scope
    protected override void LoadParameterValue()
    {
        if (ComponentData is not null)
        {
            if (ComponentData.StaticProperty)
            {
                LoadStaticProperty();
            }
            else
            {
                LoadDynamicProperty();
            }
        }
    }

    protected override void LoadStaticProperty()
    {
        if (ScopedUsers is not null && ComponentData is not null
            && ComponentData.PropertyToEdit is not null)
        {
            if (ScopedUsers.Count > 0)
            {
                DisplayValue = ScopedUsers[ScopeIndex]
                    .GetStaticProperty(ComponentData.PropertyToEdit, ComponentData.FormatString)
                    ?? "";
            }
        }
    }

    protected override void LoadDynamicProperty()
    {
        if (ScopedUsers is not null && ComponentData is not null
            && ComponentData.PropertyToEdit is not null)
        {
            if(ScopedUsers.Count > 0)
            {
                DisplayValue = ScopedUsers[ScopeIndex]
                    .GetAssignableProperty(ComponentData.PropertyToEdit, ComponentData.FormatString)
                    ?? "";
            }
        }
    }
    #endregion
}
