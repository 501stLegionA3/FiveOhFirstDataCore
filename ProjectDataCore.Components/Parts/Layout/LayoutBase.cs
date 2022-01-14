namespace ProjectDataCore.Components.Parts.Layout;

public class LayoutBase : CustomComponentBase
{
    [Parameter]
    public LayoutComponentSettings? ComponentData { get; set; }

    protected async Task RemoveCurrentLayoutAsync(LayoutComponentSettings settings)
    {
        var res = await PageEditService.DeleteLayoutComponentAsync(settings.Key);

        if (CallRefreshRequest is not null)
            await CallRefreshRequest.Invoke();
    }
}
