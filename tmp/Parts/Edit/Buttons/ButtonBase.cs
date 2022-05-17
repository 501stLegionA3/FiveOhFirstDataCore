using ProjectDataCore.Components.Parts.Layout.Form;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Components.Parts.Edit.Buttons;
public class ButtonBase : CustomComponentBase
{
    [Parameter]
    public ButtonComponentSettings? ComponentData { get; set; }

    [CascadingParameter(Name = "ParentForm")]
    public FormLayoutBase? ParentForm { get; set; }

    public bool EditingSettings { get; set; } = false;
    public string DisplayNameEdit { get; set; } = "";
    public ButtonComponentSettings.ButtonStyle ButtonStyle { get; set; }
    public bool CallFormSubmit { get; set; } = false;
    public bool CallFormCancel { get; set; } = false;

    public async Task OnClickAsync()
    {
        if(ParentForm is not null 
            && ComponentData is not null)
        {
            if (ComponentData.InvokeSave)
                await ParentForm.OnSubmitAsync();

            if (ComponentData.ResetForm)
                await ParentForm.OnCancelAsync();
        }
    }

    protected void StartEdit()
    {
        if (ComponentData is not null)
        {
            EditingSettings = true;
            DisplayNameEdit = ComponentData.DisplayName ?? "";
            ButtonStyle = ComponentData.Style;
            CallFormSubmit = ComponentData.InvokeSave;
            CallFormCancel = ComponentData.ResetForm;
        }
    }

    protected async Task SaveEdit()
    {
        if(ComponentData is not null)
        {
            var res = await PageEditService.UpdateButtonComponentAsync(ComponentData.Key, (x) =>
            {
                x.DisplayName = DisplayNameEdit;
                x.Style = ButtonStyle;
                x.InvokeSave = CallFormSubmit;
                x.ResetForm = CallFormCancel;
            });

            if(!res.GetResult(out var err))
            {
                // TODO handle errors
            }

            CancelEdit();

            if (CallRefreshRequest is not null)
                await CallRefreshRequest.Invoke();
        }
    }

    protected void CancelEdit()
    {
        if(ComponentData is not null)
        {
            EditingSettings = false;
            DisplayNameEdit = "";
            ButtonStyle = default;
        }
    }


    protected async Task RemoveCurrentDisplay()
    {
        if (ComponentData is not null)
        {
            var res = await PageEditService.DeleteButtonComponentAsync(ComponentData.Key);

            if (CallRefreshRequest is not null)
                await CallRefreshRequest.Invoke();
        }
    }
}
