using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;

using ProjectDataCore.Data.Services.InternalAuth;

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
    [Inject]
    public IInternalAuthorizationService InternalAuthorizationService { get; set; }
    [Inject]
    public IConfiguration Configuration { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    protected string uid = Guid.NewGuid().ToString().ToLower().Replace("-", string.Empty);

    protected MarkupString Display { get => new(CurrentValue); }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var uploadDest = Configuration["Config:Internal:ImageUpload"];
            await JSRuntime.InvokeVoidAsync("CKEditorInterop.init", uid, uploadDest, DotNetObjectReference.Create(this));
            InternalAuthorizationService.RegisterToken(uid, InternalAuthorizationType.ImageUpload);
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

    protected override async void Dispose(bool disposing)
    {
        await JSRuntime.InvokeVoidAsync("CKEditorInterop.destory", uid);

        InternalAuthorizationService.UnregisterToken(uid);
        base.Dispose(disposing);
    }
}
