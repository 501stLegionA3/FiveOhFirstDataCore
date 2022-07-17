using Cloudcrate.AspNetCore.Blazor.Browser.Storage;

using Microsoft.JSInterop;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Components.Framework.Component;
public partial class HTMLEditor : ComponentBase, IDisposable, IAsyncDisposable
{
    private bool disposedValue;
#pragma warning disable CS8618 // Injections are never null.
    [Inject]
    public IJSRuntime JSRuntime { get; set; }
    [Inject]
    public LocalStorage LocalStorage { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

#pragma warning disable CS8618 // Editor required values are never null.
    [Parameter, EditorRequired]
    public string Value { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    [Parameter]
    public Func<string, Task>? OnUpdate { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            await InitalizeEditor();
        }
    }

    #region Interop
    private string EditorKey { get; } = Guid.NewGuid().ToString();
    private DotNetObjectReference<HTMLEditor>? DotNetRef { get; set; }
    private string EditorTheme { get; set; } = "vs";
    protected DotNetObjectReference<HTMLEditor> GetDotNetReference()
    {
        if (DotNetRef is null)
        {
            DotNetRef = DotNetObjectReference.Create(this);
        }

        return DotNetRef;
    }

    private async Task InitalizeEditor()
    {
        EditorTheme = await LocalStorage.GetItemAsync("DataCore.MonacoTheme");
        if (string.IsNullOrWhiteSpace(EditorTheme))
            EditorTheme = "vs";

        await JSRuntime.InvokeVoidAsync("LightweightMonacoInterop.init", EditorKey, GetDotNetReference(), EditorKey, Value, "html", EditorTheme, nameof(OnTextUpdated));

        StateHasChanged();
    }

    private async Task ChangeTheme(string newTheme)
    {
        EditorTheme = newTheme;

        await JSRuntime.InvokeVoidAsync("LightweightMonacoInterop.changeTheme", EditorKey, EditorTheme);
        await LocalStorage.SetItemAsync("DataCore.MonacoTheme", EditorTheme);
    }

    [JSInvokable]
    public async Task OnTextUpdated(string newContents)
    {
        if (OnUpdate is not null)
            await OnUpdate.Invoke(newContents);

        // Keep our local copy up to date as well.
        Value = newContents;
    }

    private async ValueTask DisposeEditorAsync()
    {
        await JSRuntime.InvokeVoidAsync("LightweightMonacoInterop.dispose", EditorKey);
    }
    #endregion

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                try
                {
                    Task.Run(async () => await DisposeEditorAsync()).RunSynchronously();
                }
                catch { /* No JS interop? Thats fine - its diposed for us then. */ }
            }

            OnUpdate = null;
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            await DisposeEditorAsync();
        }
        catch { /* No JS interop? Thats fine - its diposed for us then. */ }

        OnUpdate = null;
    }
}
