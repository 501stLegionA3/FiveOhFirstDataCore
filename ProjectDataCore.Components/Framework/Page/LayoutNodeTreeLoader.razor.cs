using Microsoft.JSInterop;
using ProjectDataCore.Data.Services.Routing;
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
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    [CascadingParameter(Name = "PageEdit")]
    public bool IsEditingScope { get; set; }

    [Parameter]
    public PageEditComponent? EditComponent { get; set; }

    [Parameter]
    public LayoutNode ParentNode { get; set; }

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
    }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        if (EditComponent is not null)
            EditComponent.OnDragRefreshRequested += ReloadDragabbles;

        if (ParentNode.Component is not null)
        {
            // ... set the component params ...
            ComponentParams["ComponentData"] = ParentNode.Component;
            // ... and the component type ...
            ComponentType = RoutingService.GetComponentType(ParentNode.Component.QualifiedTypeName);
        }
    }

    protected async Task ReloadDragabbles(object sender)
    {
        var handle = await GetJSHandle();
        var dotRef = GetDotNetReference();

        if (sender != this)
        {
            await DisposeDragabbles(handle);
        }

        var rows = new List<string>();
        var cols = new List<string>();

        for (int i = 0; i < ParentNode.Nodes.Count; i++)
        {
            if (ParentNode.Rows)
                rows.Add($"gutter-node-{i}");
            else
                cols.Add($"gutter-node-{i}");
        }

        // This is the first render, don't worry about disposing of any scopes.
        await handle.InvokeVoidAsync("createSplit", ParentNode.Key.ToString(), dotRef,
            nameof(UpdateSizes), rows.ToArray(), cols.ToArray());
    }

    protected async Task DisposeDragabbles(IJSObjectReference? handle = null)
    {
        if(handle is null)
            handle = await GetJSHandle();

        await handle.InvokeVoidAsync("destroy", ParentNode.Key.ToString());
    }

    protected async Task<IJSObjectReference> GetJSHandle()
    {
        if (InteropHandle is null)
        {
            var handle = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "./split/splitHandler.js");
            InteropHandle = handle;
        }

        return InteropHandle;
    }

    protected DotNetObjectReference<LayoutNodeTreeLoader> GetDotNetReference()
    {
        if(DotNetRef is null)
        {
            DotNetRef = DotNetObjectReference.Create(this);
        }

        return DotNetRef;
    }

    public void UpdateSizes(string method, string size)
    {
        ParentNode.SetNodeWidths(size);
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
    }
}
