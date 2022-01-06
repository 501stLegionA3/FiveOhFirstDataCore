using Microsoft.AspNetCore.Identity;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Account;

public class DataCoreRole : IdentityRole<Guid>
{
    public DataCoreRole() : base() { }
    public DataCoreRole(string name) : base(name) { }
}
