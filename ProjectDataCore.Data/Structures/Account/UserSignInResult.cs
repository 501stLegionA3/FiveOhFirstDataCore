using Microsoft.AspNetCore.Identity;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Account;
public class UserSignInResult : SignInResult
{
    public bool RequiresAccountLinking { get; set; }
    public Guid UserId { get; set; }
}
