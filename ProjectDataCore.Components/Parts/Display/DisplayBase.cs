namespace ProjectDataCore.Components.Parts.Display;

public class DisplayBase : CustomComponentBase
{
    [Parameter]
    public DisplayComponentSettings? ComponentData { get; set; }
    public string DisplayValue { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        if(ComponentData is not null)
            LoadScopedUser(ComponentData.Key);
    }

    protected async Task RemoveCurrentDisplay()
    {
        if (ComponentData is not null)
        {
            var res = await PageEditService.DeleteDisplayComponentAsync(ComponentData.Key);

            if (CallRefreshRequest is not null)
                await CallRefreshRequest.Invoke();
        }
    }
}
