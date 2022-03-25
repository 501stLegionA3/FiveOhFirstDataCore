using Microsoft.AspNetCore.Components.Routing;
using ProjectDataCore.Data.Services.Nav;
using ProjectDataCore.Data.Services.Policy;
using ProjectDataCore.Data.Structures.Nav;

namespace ProjectDataCore.Shared;

public partial class NavMenu : ComponentBase, IDisposable
{
    private List<NavModule> _modules = new();

    public string URI { get; set; } = "";

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    [Inject]
    public NavigationManager NavManager { get; set; }
    [Inject]
    public INavModuleService NavModuleService { get; set; }
    [Inject]
    public IPolicyService PolicyService { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    [CascadingParameter(Name = "ActiveUser")]
    public DataCoreUser? ActiveUser { get; set; }

    protected async override Task OnInitializedAsync()
    {
        URI = NavManager.Uri;
        NavManager.LocationChanged += LocationChanged;
        _modules = await NavModuleService.GetAllModulesWithChildren();
        
    }

    private void LocationChanged(object? sender, LocationChangedEventArgs e)
    {
        URI = e.Location;
        InvokeAsync(StateHasChanged);
    }

    private void Navigate(string href, bool refresh = false)
    {
        NavManager.NavigateTo(href, refresh);
    }

    public void Dispose()
    {
        NavManager.LocationChanged -= LocationChanged;
    }
}