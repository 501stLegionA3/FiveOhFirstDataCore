using ProjectDataCore.Data.Services.User;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Services.Bus.Global;
public partial interface IGlobalDataBus
{
    public void RegisterLocalUserService(ILocalUserService localUser, Guid userId);
    public void UnregisterLocalUserService(ILocalUserService localUser);
    public ILocalUserService? GetLoaclUserServiceFromClaimsPrincipal(ClaimsPrincipal principal);
}
