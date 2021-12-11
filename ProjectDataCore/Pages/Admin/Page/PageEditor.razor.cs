

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Pages.Admin.Page;
public partial class PageEditor
{
#pragma warning disable CS8618 // Inject is always non-null.
    [Inject]
    public IPageEditService PageEditService { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public class NewPageDataModel
    {
        public string Route { get; set; }
        public string Name { get; set; }
    }

    public string? Error { get; set; } = null;

    public NewPageDataModel NewPageData { get; set; } = new();

    public List<CustomPageSettings> Pages { get; set; } = new();

    public string? EditRoute { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
            await ReloadPagesAsync();
    }

    private async Task OnNewPageSubmitAsync()
    {
        var res = await PageEditService.CreateNewPageAsync(NewPageData.Name, NewPageData.Route);

        if (!res.GetResult(out var err))
        {
            Error = err[0];
        }
        else
        {
            Error = null;
            NewPageData = new();
            await ReloadPagesAsync();
        }
    }

    private async Task ReloadPagesAsync()
    {
        Pages = await PageEditService.GetAllPagesAsync();
        StateHasChanged();
    }

    private void StartEdit(CustomPageSettings settings)
    {
        EditRoute = settings.Route;
        StateHasChanged();
    }
}
