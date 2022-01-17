using ProjectDataCore.Data.Services.Routing;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Components.Framework.Select;
public partial class ScopedComponentsSelectOptions
{
    [Inject]
    public IPageEditService PageEditService { get; set; }

    List<PageComponentSettingsBase> AvalibleScopes { get; set; } = new();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            var res = await PageEditService.GetAvalibleScopesAsync();

            if (res.GetResult(out var resData, out var err))
            {
                AvalibleScopes = resData;
            }
            else
            {
                // TODO handle errors
            }

            StateHasChanged();
        }
    }
}
