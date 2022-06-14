using Microsoft.JSInterop;

using ProjectDataCore.Components.Framework.Page.Components;
using ProjectDataCore.Data.Services.Alert;
using ProjectDataCore.Data.Services.Bus;
using ProjectDataCore.Data.Services.Bus.Scoped;
using ProjectDataCore.Data.Services.History;
using ProjectDataCore.Data.Structures.Events.Parameters;
using ProjectDataCore.Data.Structures.History.PageEdit;
using ProjectDataCore.Data.Structures.Page.Components.Layout;

using System.Reflection;

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
    [CascadingParameter(Name = "PageEditComponent")]
    public PageEditComponent? EditComponent { get; set; }

    [Parameter]
    public RenderFragment? Editing { get; set; }
    [Parameter]
    public RenderFragment Publish { get; set; }
    [Parameter]
    public RenderFragment? Configure { get; set; }
    [Parameter]
    public string Name { get; set; }

    [Parameter]
    public LayoutNode? Node { get; set; }

    private string? LastKey { get; set; }
    [CascadingParameter]
    public Func<NodeTreeLoaderRefreshRequestedEventArgs, Task>? NodeTreeLoaderRefresh { get; set; }
    [CascadingParameter(Name = "DraggingComponent")]
    public bool DraggingComponent { get; set; } = false;
    
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


    private DotNetObjectReference<PageComponent>? DotNetRef { get; set; }

    #region Setup
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if(firstRender)
        {
            await RegisterDraggableElementsAsync();
        }

        if (LastKey != Node?.EditorKey)
        {
            LastKey = Node?.EditorKey;

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

        if(IsEditingScope && Node is not null)
        {
            if (Node.ParentNode is not null)
            {
                var parent = Node.ParentNode;

                if (Node.Order < parent.Nodes.Count - 1)
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

                if (Node.Order > 0)
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
        if (Node is not null)
        {
            ActionResult<LayoutNodeModifiedResult>? addRes = null;
            try
            {
                switch (dest)
                {
                    case "left":
                        addRes = Node.AddNode(false, true);
                        break;
                    case "top":
                        addRes = Node.AddNode(true, true);
                        break;
                    case "bottom":
                        addRes = Node.AddNode(true, false);
                        break;
                    case "right":
                        addRes = Node.AddNode(false, false);
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

        if(Node is not null)
            await ScopedDataBus.CloseMenuAsync(this, Node.EditorKey);
    }

    [JSInvokable]
    public async Task MergeNodeAsync(string type, string dest)
    {
        // Call the other node and tell it to delete itself.
        // This nodes deletes itself.
        if (Node is not null
            && Node.ParentNode is not null)
        {
            ActionResult<LayoutNodeModifiedResult>? addRes = null;
            try
            {
                switch (dest)
                {
                    case "left":
                    case "top":
                        var node = Node.ParentNode.Nodes[Node.Order - 1];
                        addRes = node.DeleteNode(false);
                        break;
                    case "bottom":
                    case "right":
                        node = Node.ParentNode.Nodes[Node.Order + 1];
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

        if (Node is not null)
            await ScopedDataBus.CloseMenuAsync(this, Node.EditorKey);
    }

    [JSInvokable]
    public async Task DeleteNodeAsync(string type, string dest)
    {
        // This nodes deletes itself.
        if (Node is not null)
        {
            ActionResult<LayoutNodeModifiedResult>? addRes = null;
            try
            {
                switch (dest)
                {
                    case "left":
                    case "top":
                        addRes = Node.DeleteNode(true);
                        break;
                    case "bottom":
                    case "right":
                        addRes = Node.DeleteNode(false);
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

        if (Node is not null)
            await ScopedDataBus.CloseMenuAsync(this, Node.EditorKey);
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
    private bool IsConfiguringNode { get; set; } = false;

    private async Task OnOpenNodeSettingsAsync()
    {
        if (EditComponent is not null
            && !IsConfiguringComponent)
        {
            IsConfiguringNode = true;
            await EditComponent.OpenNodeSettingsAsync();
            StateHasChanged();
        }
    }
    #endregion

    #region Component Management
    private bool IsConfiguringComponent { get; set; } = false;

    [JSInvokable]
    public Task AddComponentAsync(string itemIndex, string componentType)
    {
        if (Node is not null && EditComponent is not null)
        {
            var parts = itemIndex.Split(',');

            if (int.TryParse(parts.ElementAtOrDefault(0), out var index))
            {
                var attribute = EditComponent.EditorComponents.ElementAtOrDefault(index);

                if (attribute.Item1 is not null)
                {
                    var settingsRaw = Activator.CreateInstance(attribute.Item1.ComponentSettingsType);

                    if (settingsRaw is PageComponentSettingsBase settings)
                    {
                        settings.QualifiedTypeName = parts.ElementAtOrDefault(1) ?? "";
                        settings.DisplayName = attribute.Item1.Name;
                        Node.Component = settings;
                    }
                }
            }
        }

        return Task.CompletedTask;
    }

    private void OnDeleteComponent()
    {
        if (Node is not null)
            Node.Component = null;
    }

    private async Task OnOpenComponentSettingsAsync()
    {
        if (EditComponent is not null
            && !IsConfiguringNode)
        {
            IsConfiguringComponent = true;
            await EditComponent.OpenComponentSettingsAsync();
            EditComponent.OnConfigureMenuClosed += OnConfigureMenuClosed;
            StateHasChanged();
        }
    }
    #endregion

    #region Utility
    private async Task OnConfigureMenuClosed(PageEditComponent sender)
    {
        IsConfiguringNode = false;
        IsConfiguringComponent = false;
        sender.OnConfigureMenuClosed -= OnConfigureMenuClosed;
        await InvokeAsync(StateHasChanged);
    }
    #endregion

    #region Draggables
    private async Task RegisterDroppableContainersAsync()
    {
        if (Node is not null)
        {
            await JSRuntime.InvokeVoidAsync("DropInterop.registerDropzone", $"{Node.EditorKey}-add_split", GetDotNetReference(), nameof(AddSplitAsync));
            await JSRuntime.InvokeVoidAsync("DropInterop.registerDropzone", $"{Node.EditorKey}-merge", GetDotNetReference(), nameof(MergeNodeAsync));
            await JSRuntime.InvokeVoidAsync("DropInterop.registerDropzone", $"{Node.EditorKey}-delete", GetDotNetReference(), nameof(DeleteNodeAsync));
            await JSRuntime.InvokeVoidAsync("DropInterop.registerDropzone", $"{Node.EditorKey}-add-component", GetDotNetReference(), nameof(AddComponentAsync), Node.EditorKey);
        }
    }

    private async Task RegisterDraggableElementsAsync()
    {
        if (Node is not null)
        {
            await JSRuntime.InvokeVoidAsync("DropInterop.init", Node.EditorKey, GetDotNetReference(), true, true, nameof(DragChanged));
        }
    }

    private async Task UnregisterDraggableElementsAsync()
    {
        if (Node is not null)
        {
            await JSRuntime.InvokeVoidAsync("DropInterop.destroyDroppable", Node.EditorKey);
        }
    }

    private async Task UnregisterDroppableContainersAsync()
    {
        if (Node is not null)
        {
            await JSRuntime.InvokeVoidAsync("DropInterop.destroyDropzone", $"{Node.EditorKey}-add_split");
            await JSRuntime.InvokeVoidAsync("DropInterop.destroyDropzone", $"{Node.EditorKey}-merge");
            await JSRuntime.InvokeVoidAsync("DropInterop.destroyDropzone", $"{Node.EditorKey}-delete");
            await JSRuntime.InvokeVoidAsync("DropInterop.destroyDropzone", $"{Node.EditorKey}-add-component");
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