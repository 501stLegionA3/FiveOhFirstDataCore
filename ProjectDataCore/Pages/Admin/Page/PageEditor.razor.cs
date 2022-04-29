using ProjectDataCore.Data.Structures.Page.Components.Layout;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Pages.Admin.Page;
public partial class PageEditor : ComponentBase
{
    #region States
    public enum LeftMenu
    {
        PageSelection,
        ComponentSelection,
        SettingsSelection
    }

    public enum RightMenu
    {
        Empty,
        PageEditor,
        SettingsEditor
    }
    #endregion

#pragma warning disable CS8618 // Inject is always non-null.
    [Inject]
    public IPageEditService PageEditService { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public LeftMenu LeftMenuState { get; set; } = LeftMenu.PageSelection;
    public RightMenu RightMenuState { get; set; } = RightMenu.Empty;

    #region Inital Setup
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if(firstRender)
        {
            await RefreshPageListAsync();
        }
    }
    #endregion

    #region Page Control
    private bool SidebarVisible { get; set; } = true;

    private void ToggleSidebar()
    {
        SidebarVisible = !SidebarVisible;
        StateHasChanged();
    }
    #endregion

    #region Left Menu - Page Selection
    private List<CustomPageSettings> Pages { get; set; } = new();
    private List<CustomPageSettings> FilteredPages { get; set; } = new();

    private bool SearchingByName { get; set; } = true;
    private string NameSearch { get; set; } = "";
    private string RouteSearch { get; set; } = "";

    private CustomPageSettings? PageToEdit { get; set; }

    private async Task RefreshPageListAsync()
    {
        Pages = await PageEditService.GetAllPagesAsync();

        SearchingByName = !SearchingByName;
        ChangeSearchType();
    }

    private void SearchPagesByName(string search)
    {
        NameSearch = search;

        FilteredPages = Pages.Where(x => x.Name.StartsWith(search, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    private void SearchPagesByRoute(string search)
    {
        RouteSearch = search;

        FilteredPages = Pages.Where(x => x.Route.StartsWith(search, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    private void ChangeSearchType()
    {
        SearchingByName = !SearchingByName;

        if (SearchingByName)
            SearchPagesByName(RouteSearch);
        else SearchPagesByRoute(RouteSearch);
    }

    private void StartPageEdit(CustomPageSettings? settings)
    {
        PageToEdit = settings;
    }

    private async Task AbortPageEditAsync()
    {
        StartPageEdit(null);
        await RefreshPageListAsync();
    }

    private async Task SavePageEditAsync()
    {
        // TODO: Save page edits code.

        StartPageEdit(null);
        await RefreshPageListAsync();
    }
    #endregion
}
