using ProjectDataCore.Data.Services.Account;
using ProjectDataCore.Data.Structures.Account;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Pages.Admin.Account;
public partial class AccountHome
{
#pragma warning disable CS8618 // Injections are never null.
    [Inject]
    public IAccountLinkService AccountLinkService { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public AccountSettings Settings { get; set; } = new();

    public bool RequireSteamLink { get; set; }
    public bool RequireDiscordLink { get; set; }
    public bool RequireAccessCodeForRegister { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            await ReloadCurrentSettngs();
        }
    }

    private async Task ReloadCurrentSettngs()
    {
        Settings = await AccountLinkService.GetLinkSettingsAsync();
        RequireSteamLink = Settings.RequireSteamLink;
        RequireDiscordLink = Settings.RequireDiscordLink;
        RequireAccessCodeForRegister = Settings.RequireAccessCodeForRegister;

        StateHasChanged();
    }

    public async Task OnSaveSettingsAsync()
    {
        await AccountLinkService.UpdateLinkSettingsAsync(x =>
        {
            x.RequireSteamLink = RequireSteamLink;
            x.RequireDiscordLink = RequireDiscordLink;
            x.RequireAccessCodeForRegister = RequireAccessCodeForRegister;
        });

        await ReloadCurrentSettngs();
    }
}
