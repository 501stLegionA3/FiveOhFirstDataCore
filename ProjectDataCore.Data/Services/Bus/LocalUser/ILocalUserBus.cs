using ProjectDataCore.Data.Services.User;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Services.Bus;
public partial interface IDataBus
{
    public void RegisterLocalUserService(ILocalUserService localUser, ClaimsPrincipal principal);
    public void UnregisterLocalUserService(ILocalUserService localUser);
    public ILocalUserService? GetLoaclUserServiceFromClaimsPrincipal(ClaimsPrincipal principal);
}
