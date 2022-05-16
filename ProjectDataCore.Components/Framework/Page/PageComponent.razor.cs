using Microsoft.JSInterop;

using ProjectDataCore.Data.Services.Alert;
using ProjectDataCore.Data.Services.Bus;
using ProjectDataCore.Data.Services.Bus.Scoped;
using ProjectDataCore.Data.Services.History;
using ProjectDataCore.Data.Structures.Events.Parameters;
using ProjectDataCore.Data.Structures.History.PageEdit;
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
    [Inject]
    public IEditHistoryService EditHistoryService { get; set; }
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

    /// <summary>
    /// Indcates if the node can merge into another node. left, top, bottom, right.
    /// </summary>
    private bool[] CanMerge { get; } = new bool[] { false, false, false, false };
    private bool DraggingMerge { get; set; } = false;

    /// <summary>
    /// Indicates if the node can delete and be filled by another node. left, top, bottom, right.
    /// </summary>
    private bool[] CanDelete { get; } = new bool[] { false, false, false, false };
    private bool DraggingDelete { get; set; } = false;

    private bool IsConfiguring { get; set; } = false;

    private DotNetObjectReference<PageComponent>? DotNetRef { get; set; }
    
    #region Setup
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

        if(IsEditingScope && EditingNode is not null)
        {
            if (EditingNode.ParentNode is not null)
            {
                var parent = EditingNode.ParentNode;

                if (EditingNode.Order < parent.Nodes.Count - 1)
                {
                    if (parent.Rows)
                    {
                        CanMerge[2] = true;
                        CanDelete[2] = true;
                    }
                    else
                    {
                        CanMerge[3] = true;
                        CanDelete[3] = true;
                    }
                }

                if (EditingNode.Order > 0)
                {
                    if (parent.Rows)
                    {
                        CanMerge[1] = true;
                        CanDelete[1] = true;
                    }
                    else
                    {
                        CanMerge[0] = true;
                        CanDelete[0] = true;
                    }
                }
            }
        }
    }

    protected DotNetObjectReference<PageComponent> GetDotNetReference()
    {
        if (DotNetRef is null)
        {
            DotNetRef = DotNetObjectReference.Create(this);
        }

        return DotNetRef;
    }
    #endregion

    #region Layout Node Editing

    [JSInvokable]
    public async Task AddSplitAsync(string type, string dest)
    {
        if (EditingNode is not null)
        {
            ActionResult<LayoutNodeModifiedResult>? addRes = null;
            try
            {
                switch (dest)
                {
                    case "left":
                        addRes = EditingNode.AddNode(false, true);
                        break;
                    case "top":
                        addRes = EditingNode.AddNode(true, true);
                        break;
                    case "bottom":
                        addRes = EditingNode.AddNode(true, false);
                        break;
                    case "right":
                        addRes = EditingNode.AddNode(false, false);
                        break;
                }
            }
            catch (Exception ex)
            {
                AlertService.CreateErrorAlert(ex.Message);
            }

            if (addRes is not null)
            {
                if (addRes.GetResult(out var res, out var err))
                {
                    EditHistoryService.Push(
                        new LayoutNodeSplitEditHistory($"Split {dest}", res));
                }
                else
				{
                    AlertService.CreateErrorAlert(err);
				}
            }

            await ScopedDataBus.RequestLayoutNodeTreeRefreshAsync(this, new());

            await InvokeAsync(StateHasChanged);
        }
    }

    private async Task AddContextMenuSplitAsync(string dest)
    {
        await AddSplitAsync("", dest);

        if(EditingNode is not null)
            await ScopedDataBus.CloseMenuAsync(this, EditingNode.EditorKey);
    }

    [JSInvokable]
    public async Task MergeNodeAsync(string type, string dest)
    {
        // Call the other node and tell it to delete itself.
        // This nodes deletes itself.
        if (EditingNode is not null
            && EditingNode.ParentNode is not null)
        {
            ActionResult<LayoutNodeModifiedResult>? addRes = null;
            try
            {
                switch (dest)
                {
                    case "left":
                    case "top":
                        var node = EditingNode.ParentNode.Nodes[EditingNode.Order - 1];
                        addRes = node.DeleteNode(false);
                        break;
                    case "bottom":
                    case "right":
                        node = EditingNode.ParentNode.Nodes[EditingNode.Order + 1];
                        addRes = node.DeleteNode(true);
                        break;
                }
            }
            catch (Exception ex)
            {
                AlertService.CreateErrorAlert(ex.Message);
            }

            if (addRes is not null)
            {
                if (addRes.GetResult(out var res, out var err))
                {
                    EditHistoryService.Push(
                        new LayoutNodeMergedEditHistory($"Merged node {dest}", res));
                }
                else
                {
                    AlertService.CreateErrorAlert(err);
                }
            }

            await ScopedDataBus.RequestLayoutNodeTreeRefreshAsync(this, new());

            await InvokeAsync(StateHasChanged);
        }
    }

    private async Task ContextMenuMergeNodeAsync(string dest)
    {
        await MergeNodeAsync("", dest);

        if (EditingNode is not null)
            await ScopedDataBus.CloseMenuAsync(this, EditingNode.EditorKey);
    }

    [JSInvokable]
    public async Task DeleteNodeAsync(string type, string dest)
    {
        // This nodes deletes itself.
        if (EditingNode is not null)
        {
            ActionResult<LayoutNodeModifiedResult>? addRes = null;
            try
            {
                switch (dest)
                {
                    case "left":
                    case "top":
                        addRes = EditingNode.DeleteNode(true);
                        break;
                    case "bottom":
                    case "right":
                        addRes = EditingNode.DeleteNode(false);
                        break;
                }
            }
            catch (Exception ex)
            {
                AlertService.CreateErrorAlert(ex.Message);
            }

            if (addRes is not null)
            {
                if (addRes.GetResult(out var res, out var err))
                {
                    EditHistoryService.Push(
                        new LayoutNodeMergedEditHistory($"Deleted and filled from the {dest}", res));
                }
                else
                {
                    AlertService.CreateErrorAlert(err);
                }
            }

            await ScopedDataBus.RequestLayoutNodeTreeRefreshAsync(this, new());

            await InvokeAsync(StateHasChanged);
        }
    }

    private async Task ContextMenuDeleteNodeAsync(string dest)
    {
        await DeleteNodeAsync("", dest);

        if (EditingNode is not null)
            await ScopedDataBus.CloseMenuAsync(this, EditingNode.EditorKey);
    }

    [JSInvokable]
    public async Task DragChanged(bool started, string type)
    {
        switch(type)
        {
            case "split":
                DraggingSplitscreen = started;
                break;
            case "merge":
                DraggingMerge = started;
                break;
            case "delete":
                DraggingDelete = started;
                break;
        }

        await InvokeAsync(StateHasChanged);
    }
    #endregion

    #region Node Settings
    private async Task OnOpenNodeSettingsAsync()
    {

    }
    #endregion

    #region Component Management
    [JSInvokable]
    public async Task AddComponentAsync(string componentType)
    {

    }

    private void OnDeleteComponent()
    {

    }

    private async Task OnOpenComponentSettingsAsync()
    {

    }
    #endregion

    #region Draggables
    private async Task RegisterDroppableContainersAsync()
    {
        if (EditingNode is not null)
        {
            await JSRuntime.InvokeVoidAsync("DropInterop.registerDropzone", $"{EditingNode.EditorKey}-add_split", GetDotNetReference(), nameof(AddSplitAsync));
            await JSRuntime.InvokeVoidAsync("DropInterop.registerDropzone", $"{EditingNode.EditorKey}-merge", GetDotNetReference(), nameof(MergeNodeAsync));
            await JSRuntime.InvokeVoidAsync("DropInterop.registerDropzone", $"{EditingNode.EditorKey}-delete", GetDotNetReference(), nameof(DeleteNodeAsync));
        }
    }

    private async Task RegisterDraggableElementsAsync()
    {
        if (EditingNode is not null)
        {
            await JSRuntime.InvokeVoidAsync("DropInterop.init", EditingNode.EditorKey, GetDotNetReference(), true, true, nameof(DragChanged));
        }
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
            await JSRuntime.InvokeVoidAsync("DropInterop.destroyDropzone", $"{EditingNode.EditorKey}-merge");
            await JSRuntime.InvokeVoidAsync("DropInterop.destroyDropzone", $"{EditingNode.EditorKey}-delete");
        }
    }
    #endregion
    
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