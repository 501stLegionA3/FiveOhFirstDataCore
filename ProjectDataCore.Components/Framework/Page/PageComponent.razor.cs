using Microsoft.JSInterop;

using ProjectDataCore.Data.Services.Alert;
using ProjectDataCore.Data.Services.Bus;
using ProjectDataCore.Data.Services.Bus.Scoped;
using ProjectDataCore.Data.Structures.Events.Parameters;
using ProjectDataCore.Data.Structures.Page.Components.Layout;

namespace ProjectDataCore.Components.Framework.Page;
public partial class PageComponent : IDisposable
{
#pragma warning disable CS8618 // Injections are never null.
    [Inject]
    public IJSRuntime JSRuntime { get; set; }
    [Inject]
    public IAlertService AlertService { get; set; }
    [Inject]
    public IScopedDataBus ScopedDataBus { get; set; }
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
    private string? LastKey { get; set; }
    [CascadingParameter]
    public Func<NodeTreeLoaderRefreshRequestedEventArgs, Task>? NodeTreeLoaderRefresh { get; set; }
    private bool DraggingSplitscreen { get; set; } = false;

    private bool IsConfiguring { get; set; } = false;

    private DotNetObjectReference<PageComponent>? DotNetRef { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if(firstRender)
        {
            await RegisterDraggableElementsAsync();
        }

        if (LastKey != EditingNode?.EditorKey)
        {
            LastKey = EditingNode?.EditorKey;

            await RegisterDroppableContainersAsync();
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        if (Publish is null)
            throw new ArgumentException("There must be at least a publish section to a Page Component.", nameof(PageComponent));
        if (string.IsNullOrWhiteSpace(Name))
            throw new ArgumentException("The name must have a non-blank value.", nameof(Name));
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
    public async Task AddSplitAsync(string startType, string destType)
    {
        if (EditingNode is not null)
        {
            try
            {
                switch (destType)
                {
                    case "left":
                        EditingNode.AddNode(false, true);
                        break;
                    case "top":
                        EditingNode.AddNode(true, false);
                        break;
                    case "bottom":
                        EditingNode.AddNode(true, true);
                        break;
                    case "right":
                        EditingNode.AddNode(false, false);
                        break;
                }
            }
            catch (Exception ex)
            {
                AlertService.CreateErrorAlert(ex.Message);
            }

            await ScopedDataBus.RequestLayoutNodeTreeRefreshAsync(this, new());

            await InvokeAsync(StateHasChanged);
        }
    }

    private async Task AddContextMenuSplitAsync(string destType)
    {
        await AddSplitAsync("", destType);

        if(EditingNode is not null)
            await ScopedDataBus.CloseMenuAsync(this, EditingNode.EditorKey);
    }

    private async Task RegisterDroppableContainersAsync()
    {
        if (EditingNode is not null)
        {
            await JSRuntime.InvokeVoidAsync("DropInterop.registerDropzone", $"{EditingNode.EditorKey}-add_split", GetDotNetReference(), nameof(AddSplitAsync));
        }
    }

    private async Task RegisterDraggableElementsAsync()
    {
        if (EditingNode is not null)
        {
            await JSRuntime.InvokeVoidAsync("DropInterop.init", EditingNode.EditorKey, GetDotNetReference(), true, true, nameof(DragChanged));
        }
    }

    [JSInvokable]
    public async Task DragChanged(bool started)
    {
        DraggingSplitscreen = started;
        await InvokeAsync(StateHasChanged);
    }

    private async Task UnregisterDraggableElementsAsync()
    {
        if (EditingNode is not null)
        {
            await JSRuntime.InvokeVoidAsync("DropInterop.destroyDroppable", EditingNode.EditorKey);
        }
    }

    private async Task UnregisterDroppableContainersAsync()
    {
        if (EditingNode is not null)
        {
            await JSRuntime.InvokeVoidAsync("DropInterop.destroyDropzone", $"{EditingNode.EditorKey}-add_split");
        }
    }

    public async void Dispose()
    {
        try
        {
            await UnregisterDraggableElementsAsync();
            await UnregisterDroppableContainersAsync();
        }
        catch 
        { 
            // Do nothing
        }
    }
}