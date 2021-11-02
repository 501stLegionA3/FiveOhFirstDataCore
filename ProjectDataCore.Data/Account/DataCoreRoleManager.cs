using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Account;

public class DataCoreRoleManager : RoleManager<DataCoreRole>
{
    public DataCoreRoleManager(IRoleStore<DataCoreRole> store, IEnumerable<IRoleValidator<DataCoreRole>> roleValidators,
            ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, ILogger<RoleManager<DataCoreRole>> logger)
            : base(store, roleValidators, keyNormalizer, errors, logger)
    {

    }
}
