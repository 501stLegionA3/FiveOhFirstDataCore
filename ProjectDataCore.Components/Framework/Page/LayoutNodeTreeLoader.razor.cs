using Microsoft.JSInterop;

using ProjectDataCore.Data.Services.Bus;
using ProjectDataCore.Data.Services.Bus.Scoped;
using ProjectDataCore.Data.Services.History;
using ProjectDataCore.Data.Services.Routing;
using ProjectDataCore.Data.Structures.Events.Parameters;
using ProjectDataCore.Data.Structures.History.PageEdit.Layout;
using ProjectDataCore.Data.Structures.Page.Components.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Components.Framework.Page;
public partial class LayoutNodeTreeLoader : IDisposable
{
#pragma warning disable CS8618 // Injections are never null.
    [Inject]
    public IRoutingService RoutingService { get; set; }
    [Inject]
    public IJSRuntime JSRuntime { get; set; }
    [Inject]
    public IScopedDataBus ScopedDataBus { get; set; }
    [Inject]
    public IEditHistoryService EditHistoryService { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    [CascadingParameter(Name = "PageEdit")]
    public bool IsEditingScope { get; set; }

    [Parameter]
    public PageEditComponent? EditComponent { get; set; }

    [Parameter]
    public LayoutNode? ParentNode { get; set; }

    private bool DraggablesNeedReloading { get; set; } = false;

    /// <summary>
    /// The type of base layout component to display.
    /// </summary>
    private Type? ComponentType { get; set; }
    /// <summary>
    /// The parameters for the component.
    /// </summary>
    private Dictionary<string, object> ComponentParams { get; } = new()
    {
        { "ComponentData", null }
    };

    private IJSObjectReference? InteropHandle { get; set; }
    private DotNetObjectReference<LayoutNodeTreeLoader>? DotNetRef { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if(firstRender)
        {
            await ReloadDragabbles(this);
        }

        if(DraggablesNeedReloading)
        {
            DraggablesNeedReloading = false;
            await ReloadDragabbles(this);
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        if (EditComponent is not null)
            EditComponent.OnDragRefreshRequested += ReloadDragabbles;

        if (ParentNode is not null)
        {
            if (ParentNode.Component is not null)
            {
                // ... set the component params ...
                ComponentParams["ComponentData"] = ParentNode.Component;
                ComponentParams["EditingNode"] = ParentNode;
                // ... and the component type ...
                ComponentType = RoutingService.GetComponentType(ParentNode.Component.QualifiedTypeName);
            }
        }

        ScopedDataBus.NodeTreeLoaderRefreshRequested += RefreshRequested;
    }

    protected async Task ReloadDragabbles(object sender)
    {
        if (ParentNode is null
            || !IsEditingScope
            || ParentNode.Nodes.Count <= 0)
            return;

        var dotRef = GetDotNetReference();

        if (sender != this)
        {
            await DisposeDragabbles();
        }

        var rows = new List<string>();
        var cols = new List<string>();

        for (int i = 0; i < ParentNode.Nodes.Count - 1; i++)
        {
            if (ParentNode.Rows)
            {
                rows.Add("");
                rows.Add($"#gutter-node-{i}");
            }
            else
            {
                cols.Add("");
                cols.Add($"#gutter-node-{i}");
            }
        }

        // This is the first render, don't worry about disposing of any scopes.
        await JSRuntime.InvokeVoidAsync("SplitInterop.createSplit", ParentNode.EditorKey, dotRef,
            nameof(UpdateSizes), nameof(PushSizeUpdate), rows.ToArray(), cols.ToArray());
    }

    protected async Task DisposeDragabbles()
    {
        if (ParentNode is null)
            return;

        await JSRuntime.InvokeVoidAsync("SplitInterop.destroy", ParentNode.EditorKey);
    }

    protected DotNetObjectReference<LayoutNodeTreeLoader> GetDotNetReference()
    {
        if(DotNetRef is null)
        {
            DotNetRef = DotNetObjectReference.Create(this);
        }

        return DotNetRef;
    }

    #region Size Handler
    private string? oldSize;

    [JSInvokable]
    public void UpdateSizes(string size)
    {
        if (ParentNode is not null)
        {
            if (oldSize is null)
            {
                oldSize = ParentNode.RawNodeWidths;
            }

            ParentNode.SetNodeWidths(size);
        }
    }

    [JSInvokable]
    public void PushSizeUpdate()
    {
        if (oldSize is not null
            && ParentNode is not null)
        {
            EditHistoryService.Push(new LayoutNodeSizesChangedEditHistory("Layout Size Dragged", ParentNode, oldSize));
        }

        oldSize = null;
    }
    #endregion

    public Task RefreshRequested(object sender, NodeTreeLoaderRefreshRequestedEventArgs args)
    {
        _ = Task.Run(async () =>
        {
            DraggablesNeedReloading = args.ReloadDraggables;
            await InvokeAsync(StateHasChanged);
        });

        return Task.CompletedTask;
    }

    public async void Dispose()
    {
        try
        {
            await DisposeDragabbles();
        }
        catch
        {
            // The JS Interop may not exist here,
            // and if that is the case we dont need to
            // dispose of anything, so ignore any
            // errors that occour during this step.
        }

        ScopedDataBus.NodeTreeLoaderRefreshRequested -= RefreshRequested;
    }
}
