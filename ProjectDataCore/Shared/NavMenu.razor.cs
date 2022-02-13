using Microsoft.AspNetCore.Components.Routing;
using ProjectDataCore.Data.Services.Nav;
using ProjectDataCore.Data.Structures.Nav;

namespace ProjectDataCore.Shared;

public partial class NavMenu : ComponentBase, IDisposable
{
    private List<NavModule> _modules = new();

    public string URI { get; set; } = "";

    [Inject]
    public NavigationManager NavManager { get; set; }
    [Inject]
    public INavModuleService NavModuleService { get; set; }

    protected async override Task OnInitializedAsync()
    {
        URI = NavManager.Uri;
        NavManager.LocationChanged += LocationChanged;
        _modules = await NavModuleService.GetAllModulesWithChildren();
        if (_modules.Count == 0)
            _modules.Add(new("Edit NavBar", "admin/navbar/edit", new(), true));
        
    }

    private void LocationChanged(object? sender, LocationChangedEventArgs e)
    {
        URI = e.Location;
        InvokeAsync(StateHasChanged);
    }

    private void Navigate(string href)
    {
        NavManager.NavigateTo(href, false);
    }

    public void Dispose()
    {
        NavManager.LocationChanged -= LocationChanged;
    }
}