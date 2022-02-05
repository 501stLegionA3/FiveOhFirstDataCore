using ProjectDataCore.Data.Account;
using ProjectDataCore.Data.Structures.Model.User;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Services.User;

public interface IUserService
{
    public Task<DataCoreUser?> GetUserFromClaimsPrinciaplAsync(ClaimsPrincipal claims);
    public Task<DataCoreUser?> GetUserFromIdAsync(Guid id);
    public Task<List<DataCoreUser>> GetAllUsersAsync();
    public Task<List<DataCoreUser>> GetAllUnregisteredUsersAsync();
    public Task<ActionResult> UpdateUserAsync(Guid user, Action<DataCoreUserEditModel> action);
    public Task<ActionResult> CreateUserAsync(DataCoreUser user, Action<DataCoreUserEditModel> model);
}
