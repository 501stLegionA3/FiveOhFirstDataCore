using ProjectDataCore.Data.Structures.Account;
using ProjectDataCore.Data.Structures.Model.Account;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static ProjectDataCore.Data.Services.Account.AccountLinkService;

namespace ProjectDataCore.Data.Services.Account;
public interface IAccountLinkService
{
    public Task<AccountSettings> GetLinkSettingsAsync();
    public Task UpdateLinkSettingsAsync(Action<AccountSettingsEditModel> action);

    public Task<string> StartAsync(Guid userId, string username, string password, bool rememberMe);
    public Task<string> BindDiscordAsync(string token, ulong accountId, string email);
    public Task<string> BindSteamUserAsync(string token, string steamId);
    public Task AbortLinkAsync(string token);
    public (LinkStatus, string?) GetLinkStatus(string token);
    public Task<(string, string, bool)> FinalizeLink(string token);
}
