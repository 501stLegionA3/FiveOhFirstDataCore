using ProjectDataCore.Data.Structures.Model.User;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Components.Parts.Edit.Components;

[EditableComponent(Name = "Basic Single Value")]
public partial class BasicEditPart : EditBase, IDisposable
{
	protected Type[] AllowedStaticTypes { get; set; } = new Type[]
	{
		typeof(string)
	};

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        if(ParentForm is not null)
        {
            ParentForm.AddSubmitListener(ScopeIndex, OnSubmitAsync);
        }
    }

    public Task OnSubmitAsync(DataCoreUserEditModel model)
    {
        if (ComponentData is not null)
        {
            if (ComponentData.StaticProperty)
            {
                model.StaticValues[ComponentData.PropertyToEdit] = StaticValue;
            }
            else
            {
                model.AssignableValues[ComponentData.PropertyToEdit] = SelectedValues.ToList(x => x.Item2);
            }
        }

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        if (ParentForm is not null)
        {
            ParentForm.RemoveSubmitListener(ScopeIndex, OnSubmitAsync);
        }
    }
}
