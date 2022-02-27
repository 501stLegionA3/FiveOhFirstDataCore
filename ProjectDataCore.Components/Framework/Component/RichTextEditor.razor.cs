using Microsoft.JSInterop;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Components.Framework.Component;
public partial class RichTextEditor
{
#pragma warning disable CS8618 // Injections are neveer null.
    [Inject]
    public IJSRuntime JSRuntime { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    protected string uid = Guid.NewGuid().ToString().ToLower().Replace("-", string.Empty);

    protected MarkupString Display { get => new(CurrentValue); }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
#if DEBUG
                await JSRuntime.InvokeVoidAsync("DebugCKEditorInterop.init", uid, DotNetObjectReference.Create(this));
#else 
                await JSRuntime.InvokeVoidAsync("CKEditorInterop.init", uid, DotNetObjectReference.Create(this));
#endif
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    [JSInvokable]
    public Task EditorDataChanged(string data)
    {
        CurrentValue = data;
        StateHasChanged();
        return Task.CompletedTask;
    }

    protected override void Dispose(bool disposing)
    {
#if DEBUG
        JSRuntime.InvokeVoidAsync("DebugCKEditorInterop.destory", uid);
#else
        JSRuntime.InvokeVoidAsync("CKEditorInterop.destory", uid);
#endif
        base.Dispose(disposing);
    }
}
