using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Components.Framework.Component;
public partial class LayoutComponentSettingsEditForm
{
#pragma warning disable CS8618 // Inject is never null.
    [Inject]
    public IPageEditService PageEditService { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [Parameter]
    public LayoutComponentSettings? ComponentData { get; set; }
    [Parameter]
    public Func<Task> OnSettingsClose { get; set; }

    public string ComponentName { get; set; } = "";

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        ComponentName = ComponentData?.DisplayName ?? string.Empty;
    }

    protected async Task SaveChangesAsync()
    {
        if (ComponentData is not null)
        {
            var res = await PageEditService.UpdateDisplayNameAsync(ComponentData.Key, ComponentName);
            
            if(!res.GetResult(out var err))
            {
                // TODO handle errors.
            }
        }

        if(OnSettingsClose is not null)
            await OnSettingsClose.Invoke();
    }

    protected async Task AbortChangesAsync()
    {
        if (OnSettingsClose is not null)
            await OnSettingsClose.Invoke();
    }
}
