using ProjectDataCore.Data.Account;
using ProjectDataCore.Data.Structures.Policy;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Services.User;
public interface ILocalUserService : IDisposable
{
    public Task InitalizeAsync(Guid userId, ClaimsPrincipal principal);
    public Task InitalizeIfDeinitalizedAsync(Guid userId, ClaimsPrincipal principal);
    public void Deinitalize();
    public void DeinitalizeIfInitalized();
    public bool ValidateWithPolicy(DynamicAuthorizationPolicy policy);
}
