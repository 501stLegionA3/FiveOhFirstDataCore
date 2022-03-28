using ProjectDataCore.Data.Account;
using ProjectDataCore.Data.Structures.Model.User;
using ProjectDataCore.Data.Structures.Page.Components;
using ProjectDataCore.Data.Structures.Policy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Services.User;

public interface IUserService
{
    public Task<ActionResult> CreateOrUpdateAccountAsync(string? accessCode, string username, string password);
    public Task<DataCoreUser?> GetUserFromClaimsPrinciaplAsync(ClaimsPrincipal claims);
    public Task<DataCoreUser?> GetUserFromIdAsync(Guid id);
    public Task<List<DataCoreUser>> GetAllUsersAsync();
    public Task<List<DataCoreUser>> GetAllUnregisteredUsersAsync();
    public Task<ActionResult> UpdateUserAsync(Guid user, Action<DataCoreUserEditModel> action);
    public Task<ActionResult> CreateUserAsync(DataCoreUser user, Action<DataCoreUserEditModel> model);
    public Task<bool> AuthorizeUserAsync(ClaimsPrincipal claims, PageComponentSettingsBase policy, bool forceReload = false);
}
