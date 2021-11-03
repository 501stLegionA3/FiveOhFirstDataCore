using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using ProjectDataCore.Data.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Database;

public class ApplicationDbContext : IdentityDbContext<DataCoreUser, DataCoreRole, int>
{

}
