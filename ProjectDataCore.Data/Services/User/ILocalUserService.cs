using ProjectDataCore.Data.Account;

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
    public void DeInitalize();
}
