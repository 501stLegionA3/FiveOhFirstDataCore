using FiveOhFirstDataCore.Data.Account;
using FiveOhFirstDataCore.Data.Components.Base;
using FiveOhFirstDataCore.Data.Structures;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
namespace FiveOhFirstDataCore.Local; 

public partial class NavMenu : IRefreshBase {
    [CascadingParameter]
    public Trooper? CurrentUser { get; set; }

    [CascadingParameter]
    public Task<AuthenticationState> AuthStateTask { get; set; }
    
    [Inject]
    public NavigationManager Nav { get; set; }

    public bool InSquad { get; set; } = false;
    public bool InPlatoon {get;set;} = false;
    public bool InCompany { get; set; } = false;

    private string Version { get; set; } = "";

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        if(CurrentUser is not null)
        {
            _advRefresh.AddUserSpecificRefreshListener(CurrentUser.Id, 
            "UserDetails", CallRefreshRequest);
        }

        Version = _config["Version"];

        if(CurrentUser is not null)
        {
            var user = (await AuthStateTask).User;

            bool manager = (await _auth.AuthorizeAsync(user, "RequireManager")).Succeeded;

            InSquad = manager || CurrentUser.Slot.IsSquad();
            InPlatoon = manager || InSquad || CurrentUser.Slot.IsPlatoon();
            InCompany = manager || InPlatoon || CurrentUser.Slot.IsCompany();
        }
    }
    
    void ChangeTrooper()
    {
        Nav.NavigateTo($"/trooper/me", true);
    }

    public async Task CallRefreshRequest()
    {
        await InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        _advRefresh.RemoveRefreshListeners(CallRefreshRequest);
    }
}