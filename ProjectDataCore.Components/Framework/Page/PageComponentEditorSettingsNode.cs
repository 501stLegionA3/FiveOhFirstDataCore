using Microsoft.AspNetCore.Components;

namespace ProjectDataCore.Components.Framework.Page;
public class PageComponentEditorSettingsNode : ComponentBase, IDisposable
{
    [Parameter]
    public RenderFragment ChildContent { get; set; }
    [Parameter]
    public string Name { get; set; }

    [CascadingParameter(Name = "EditorSettingsNodeHook")]
    public Func<string, RenderFragment, bool, Task> PushNode { get; set; }

    public void Dispose()
    {
        if (PushNode is not null)
            PushNode.Invoke(Name, ChildContent, true);
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if (PushNode is not null)
            await PushNode.Invoke(Name, ChildContent, false);
    }
}
