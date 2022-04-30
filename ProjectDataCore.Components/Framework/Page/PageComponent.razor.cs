using Microsoft.JSInterop;

using ProjectDataCore.Data.Services.Alert;
using ProjectDataCore.Data.Structures.Page.Components.Layout;

namespace ProjectDataCore.Components.Framework.Page;
public partial class PageComponent : IDisposable
{
#pragma warning disable CS8618 // Injections are never null.
    [Inject]
    public IJSRuntime JSRuntime { get; set; }
    [Inject]
    public IAlertService AlertService { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    [CascadingParameter(Name = "PageEdit")]
    public bool IsEditingScope { get; set; }

    [Parameter]
    public RenderFragment? Editing { get; set; }
    [Parameter]
    public RenderFragment Publish { get; set; }
    [Parameter]
    public RenderFragment? Configure { get; set; }
    [Parameter]
    public string Name { get; set; }

    [Parameter]
    public LayoutNode? EditingNode { get; set; }
    [Parameter]
    public Func<Task>? NodeTreeLoaderRefresh { get; set; }
    private string NodeKeyRoot { get; set; }
    private bool DraggingSplitscreen { get; set; } = false;

    private bool IsConfiguring { get; set; } = false;

    private DotNetObjectReference<PageComponent>? DotNetRef { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if(firstRender)
        {
            await RegisterDroppableContainersAsync();
            await RegisterDraggableElementsAsync();
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        if (Publish is null)
            throw new ArgumentException("There must be at least a publish section to a Page Component.", nameof(PageComponent));
        if (string.IsNullOrWhiteSpace(Name))
            throw new ArgumentException("The name must have a non-blank value.", nameof(Name));

        NodeKeyRoot = EditingNode?.Key.ToString() ?? "";
    }

    protected DotNetObjectReference<PageComponent> GetDotNetReference()
    {
        if (DotNetRef is null)
        {
            DotNetRef = DotNetObjectReference.Create(this);
        }

        return DotNetRef;
    }

    [JSInvokable]
    public async void AddSplit(string startType, string destType)
    {
        if (EditingNode is not null)
        {
            try
            {
                switch (destType)
                {
                    case "top":
                        EditingNode.AddNode(false, true);
                        break;
                    case "right":
                        EditingNode.AddNode(true, false);
                        break;
                    case "bottom":
                        EditingNode.AddNode(true, true);
                        break;
                    case "left":
                        EditingNode.AddNode(false, false);
                        break;
                }
            }
            catch (Exception ex)
            {
                AlertService.CreateErrorAlert(ex.Message);
            }

            if (NodeTreeLoaderRefresh is not null)
                await NodeTreeLoaderRefresh.Invoke();
        }
    }

    private async Task RegisterDroppableContainersAsync()
    {
        if (EditingNode is not null)
        {
            await JSRuntime.InvokeVoidAsync("DropInterop.registerDropzone", $"{NodeKeyRoot}-add_split", GetDotNetReference(), nameof(AddSplit));
        }
    }

    private async Task RegisterDraggableElementsAsync()
    {
        if (EditingNode is not null)
        {
            await JSRuntime.InvokeVoidAsync("DropInterop.init", NodeKeyRoot, GetDotNetReference(), true, nameof(DragChanged));
        }
    }

    [JSInvokable]
    public void DragChanged(bool started)
    {
        DraggingSplitscreen = started;
        StateHasChanged();
    }

    private async Task UnregisterDraggableElementsAsync()
    {
        if (EditingNode is not null)
        {
            await JSRuntime.InvokeVoidAsync("DropInterop.destroyDroppable", NodeKeyRoot);
        }
    }

    private async Task UnregisterDroppableContainersAsync()
    {
        if (EditingNode is not null)
        {
            await JSRuntime.InvokeVoidAsync("DropInterop.destroyDropzone", $"{NodeKeyRoot}-add_split");
        }
    }

    public async void Dispose()
    {
        await UnregisterDraggableElementsAsync();
        await UnregisterDroppableContainersAsync();
    }
}