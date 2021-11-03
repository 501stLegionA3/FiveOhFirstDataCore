using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProjectDataCore.Data.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Account;

internal class DataCoreSignInManager : SignInManager<DataCoreUser>
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

    public DataCoreSignInManager(UserManager<DataCoreUser> userManager, IDbContextFactory<ApplicationDbContext> dbContextFactory, IHttpContextAccessor contextAccessor, IUserClaimsPrincipalFactory<DataCoreUser> claimsFactory, IOptions<IdentityOptions> optionsAccessor, ILogger<SignInManager<DataCoreUser>> logger, IAuthenticationSchemeProvider schemes, IUserConfirmation<DataCoreUser> confirmation)
            : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
            => (_dbContextFactory) = (dbContextFactory);
}
