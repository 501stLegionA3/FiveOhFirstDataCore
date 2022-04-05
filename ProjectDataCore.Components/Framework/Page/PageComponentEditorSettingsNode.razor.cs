using Microsoft.AspNetCore.Components;

namespace ProjectDataCore.Components.Framework.Page;
public partial class PageComponentEditorSettingsNode : ComponentBase
{
    [Parameter]
    public RenderFragment Configure { get; set; }
    [Parameter]
    public string Name { get; set; }

    [CascadingParameter(Name = "EditorSettingsNodeHook")]
    public Func<string, RenderFragment, Task> PushNode { get; set; }
    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        if (PushNode is not null)
            await PushNode.Invoke(Name, Configure);
    }
}
