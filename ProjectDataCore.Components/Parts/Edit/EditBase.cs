using ProjectDataCore.Data.Structures.Assignable.Configuration;
using ProjectDataCore.Data.Structures.Assignable.Value;
using ProjectDataCore.Data.Structures.Model.Assignable;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Components.Parts.Edit;

public class EditBase : ParameterBase
{
    [Parameter]
    public EditableComponentSettings? ComponentData { get; set; }
    public BaseAssignableConfiguration? AssignableConfiguration { get; set; }
    public BaseAssignableValue? AssignableValue { get; set; }
    public string StaticValue { get; set; } = "";

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

    protected override void StartEdit()
    {
        if (ComponentData is not null)
        {
            EditModel = new(ComponentData);
            StateHasChanged();
        }
    }

    protected override async Task SaveEdit()
    {
        if (EditModel is not null)
        {
            var res = await PageEditService.UpdateEditableComponentAsync(EditModel.Key, x =>
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

    protected override void CancelEdit()
    {
        EditModel = null;
        StateHasChanged();
    }


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

    protected override void LoadDynamicProperty()
    {
        if (ScopedUsers is not null && ComponentData is not null
            && ComponentData.PropertyToEdit is not null)
        {
            if(ScopedUsers.Count > 0)
            {
                AssignableValue = ScopedUsers[ScopeIndex]
                    .GetAssignablePropertyContainer(ComponentData.PropertyToEdit);

                if (AssignableValue is not null)
                {
                    AssignableConfiguration = AssignableValue.AssignableConfiguration;
                }
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
                StaticValue = ScopedUsers[ScopeIndex]
                    .GetStaticProperty(ComponentData.PropertyToEdit, ComponentData.FormatString)
                    ?? "";
            }
        }
    }


    protected override async Task RemoveCurrentDisplay()
    {
        if (ComponentData is not null)
        {
            var res = await PageEditService.DeleteEditableComponentAsync(ComponentData.Key);

            if (CallRefreshRequest is not null)
                await CallRefreshRequest.Invoke();
        }
    }
}
