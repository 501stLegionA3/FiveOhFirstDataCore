using Microsoft.AspNetCore.Identity;

namespace FiveOhFirstDataCore.Data.Account
{
    public class TrooperRole : IdentityRole<int>
    {
        public TrooperRole() : base() { }
        public TrooperRole(string name) : base(name) { }
    }
}
