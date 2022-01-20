using Microsoft.AspNetCore.Components.Routing;
using ProjectDataCore.Data.Services.Nav;
using ProjectDataCore.Data.Structures.Nav;

namespace ProjectDataCore.Shared;

public partial class NavMenu : ComponentBase, IDisposable
{
    private List<NavModule> _modules = new List<NavModule>();

    public string URI { get; set; } = "";

    [Inject] public NavigationManager _navManager { get; set; }
    [Inject] public INavModuleService _navModuleService { get; set; }

    public void Dispose()
    {
        _navManager.LocationChanged -= LocationChanged;
    }

    protected async override Task OnInitializedAsync()
    {
        URI = _navManager.Uri;
        _navManager.LocationChanged += LocationChanged;
        _modules = await _navModuleService.GetAllModulesWithChildren();
        
    }

    private void LocationChanged(object? sender, LocationChangedEventArgs e)
    {
        URI = e.Location;
        InvokeAsync(StateHasChanged);
    }

    private void Navigate(string href)
    {
        _navManager.NavigateTo(href, true);
    }
}