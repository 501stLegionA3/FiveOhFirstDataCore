using ProjectDataCore.Components.Parts.Edit;
using ProjectDataCore.Data.Structures.Assignable.Configuration;
using ProjectDataCore.Data.Structures.Model.Assignable;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Components.Framework.Component;
public partial class AssignableValueEditor
{
    [Parameter]
    public EditBase EditModel { get; set; }

    public AssignableConfigurationValueEditModel MultipleValueInput { get; set; } = new();
    public AssignableConfigurationValueEditModel SingleValueInput { get; set; }
    public int StaticSelectorIndex { get; set; } = 0;
    public bool BothOnStatic { get; set; } = true;

    [Parameter]
    public List<(string, dynamic)> SelectedValues { get; set; } = new();

	protected override async Task OnParametersSetAsync()
	{
		await base.OnParametersSetAsync();

        if (EditModel.AssignableValue is not null)
        {
            SingleValueInput = new(EditModel.AssignableValue);
        }
	}

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if(firstRender)
        {
            if (EditModel.AssignableValue is not null 
                && EditModel.AssignableConfiguration is IAssignableConfiguration config)
            {
                for (int i = 0; i < EditModel.AssignableValue.GetValues().Count; i++)
                {
                    var val = config.GetSingleValuePair(new AssignableConfigurationValueEditModel(EditModel.AssignableValue, i));
                    SelectedValues.Add(val.Value);
                }
            }

            StateHasChanged();
        }
    }

    protected void AddValue()
	{
        if (EditModel.AssignableConfiguration is IAssignableConfiguration config)
        {
            (string, dynamic)? val;
            if (EditModel.AssignableConfiguration.AllowedInput == BaseAssignableConfiguration.InputType.FreehandOnly
                || (EditModel.AssignableConfiguration.AllowedInput == BaseAssignableConfiguration.InputType.Both 
                    && !BothOnStatic))
            {
                if (EditModel.AssignableConfiguration.AllowMultiple)
                {
                    val = config.GetSingleValuePair(MultipleValueInput);
                }
                else
                {
                    val = config.GetSingleValuePair(SingleValueInput);
                }
            }
            else
			{
                val = config.GetSingleValuePair(StaticSelectorIndex);
			}

            if(val is not null)
            {
                if (EditModel.AssignableConfiguration.AllowMultiple)
                {
                    SelectedValues.Add(val.Value);
                    MultipleValueInput = new();
                }
                else
                {
                    if (SelectedValues.Count <= 0)
                        SelectedValues.Add(val.Value);
                    else
                        SelectedValues[0] = val.Value;
                }
			}
        }
	}

    protected void RemoveValue((string, dynamic) toRemove)
	{
        SelectedValues.Remove(toRemove);
	}
}
