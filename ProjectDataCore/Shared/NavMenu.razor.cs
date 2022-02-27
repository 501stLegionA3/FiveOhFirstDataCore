using Microsoft.AspNetCore.Components.Routing;
using ProjectDataCore.Data.Services.Nav;
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
    public DataCoreSignInManager SignInManager { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    [CascadingParameter(Name = "ActiveUser")]
    public DataCoreUser? ActiveUser { get; set; }

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

    private async void LogOut()
    {
        return;
        await SignInManager.SignOutAsync();
        await InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        NavManager.LocationChanged -= LocationChanged;
    }
}