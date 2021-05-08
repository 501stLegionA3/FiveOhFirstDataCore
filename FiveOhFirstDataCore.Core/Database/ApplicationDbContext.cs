using FiveOhFirstDataCore.Core.Account;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FiveOhFirstDataCore.Core.Database
{
    public class ApplicationDbContext : IdentityDbContext<Trooper, TrooperRole, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }
    }
}
