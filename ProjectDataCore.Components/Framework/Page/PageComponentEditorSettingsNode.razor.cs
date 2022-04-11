using Microsoft.AspNetCore.Components;

namespace ProjectDataCore.Components.Framework.Page;
public partial class PageComponentEditorSettingsNode : ComponentBase, IDisposable
{
    [Parameter]
    public RenderFragment Configure { get; set; }
    [Parameter]
    public string Name { get; set; }

    [CascadingParameter(Name = "EditorSettingsNodeHook")]
    public Func<string, RenderFragment, bool, Task> PushNode { get; set; }

    public void Dispose()
    {
        if (PushNode is not null)
            PushNode.Invoke(Name, Configure, true);
    }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        if (PushNode is not null)
            await PushNode.Invoke(Name, Configure, false);
    }
}
