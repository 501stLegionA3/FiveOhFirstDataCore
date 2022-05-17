using ProjectDataCore.Data.Services.Alert;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Components.Parts.Text;

[TextDisplayComponent(Name = "Rich Text Display")]
public partial class RichTextDisplayPart : CustomComponentBase
{
    [Parameter]
    public TextDisplayComponentSettings? ComponentData { get; set; }
    public TextDisplayComponentSettings MockHandler { get; set; }

    public bool LiveEdit { get; set; } = false;

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        MockHandler = new()
        {
            AuthorizationPolicy = ComponentData?.EditPolicy,
            AuthorizationPolicyKey = ComponentData?.EditPolicyKey,
            RequireAuth = ComponentData?.PrivateEdit ?? true
        };
    }

    protected async Task RemoveCurrentDisplay()
    {
        if (ComponentData is not null)
        {
            var res = await PageEditService.DeleteTextDisplayComponentAsync(ComponentData.Key);

            if (CallRefreshRequest is not null)
                await CallRefreshRequest.Invoke();
        }
    }

    protected Task ToggleLiveEditAsync()
    {
        LiveEdit = !LiveEdit;

        return Task.CompletedTask;
    }

    protected async Task SaveEditAsync()
    {
        if(ComponentData is not null)
        {
            var res = await PageEditService.UpdateTextDisplayComponentAsync(ComponentData.Key, (x) =>
            {
                x.PrivateEdit = MockHandler.PrivateEdit;
                x.EditPolicyKey = Optional.FromValue(MockHandler.AuthorizationPolicyKey);
                x.RawContents = ComponentData.RawContents;
            });

            if(!res.GetResult(out var err))
            {
                AlertService.CreateErrorAlert(err);
            }

            await DiscardEditAsync();
        }
    }

    protected async Task DiscardEditAsync()
    {
        if (CallRefreshRequest is not null)
            await CallRefreshRequest.Invoke();
    }

    protected async Task SaveLiveEditAsync()
    {
        await ToggleLiveEditAsync();

        if(ComponentData is not null)
        {
            var res = await PageEditService.UpdateTextDisplayContentsAsync(ComponentData.Key, ComponentData.RawContents);

            if (!res.GetResult(out var err))
            {
                AlertService.CreateErrorAlert(err);
            }

            await DiscardLiveEditAsync();
        }
    }

    protected async Task DiscardLiveEditAsync()
    {
        await ToggleLiveEditAsync();

        if (CallRefreshRequest is not null)
            await CallRefreshRequest.Invoke();
    }
}
