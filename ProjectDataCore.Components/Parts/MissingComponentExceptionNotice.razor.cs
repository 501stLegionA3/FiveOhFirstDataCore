namespace ProjectDataCore.Components.Parts;

public partial class MissingComponentExceptionNotice : CustomComponentBase
{
    [Parameter]
    public PageComponentSettingsBase? ComponentData { get; set; }

    private async Task RemoveCurrentComponentAsync()
    {
        if (ComponentData is not null)
        {
            switch(ComponentData)
            {
                case LayoutComponentSettings:
                    _ = await PageEditService.DeleteLayoutComponentAsync(ComponentData.Key);
                    break;
                case DisplayComponentSettings:
                    _ = await PageEditService.DeleteDisplayComponentAsync(ComponentData.Key);
                    break;
                case EditableComponentSettings:
                    _ = await PageEditService.DeleteEditableComponentAsync(ComponentData.Key);
                    break;
            }

            if (CallRefreshRequest is not null)
                await CallRefreshRequest.Invoke();
        }
    }
}
