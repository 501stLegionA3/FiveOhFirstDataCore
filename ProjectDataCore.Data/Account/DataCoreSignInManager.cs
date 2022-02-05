using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProjectDataCore.Data.Database;
using ProjectDataCore.Data.Services.Account;
using ProjectDataCore.Data.Structures.Account;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Account;

public class DataCoreSignInManager : SignInManager<DataCoreUser>
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
    private readonly IAccountLinkService _accountLinkService;

    public DataCoreSignInManager(UserManager<DataCoreUser> userManager, IDbContextFactory<ApplicationDbContext> dbContextFactory, IAccountLinkService accountLinkService, IHttpContextAccessor contextAccessor, IUserClaimsPrincipalFactory<DataCoreUser> claimsFactory, IOptions<IdentityOptions> optionsAccessor, ILogger<SignInManager<DataCoreUser>> logger, IAuthenticationSchemeProvider schemes, IUserConfirmation<DataCoreUser> confirmation)
            : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
            => (_dbContextFactory, _accountLinkService) = (dbContextFactory, accountLinkService);

    public override async Task<SignInResult> PasswordSignInAsync(DataCoreUser user, string password, bool isPersistent, bool lockoutOnFailure)
    {
        var res = await ValidateLinksAsync(user);

        if (res is not null) return res;

        return await base.PasswordSignInAsync(user, password, isPersistent, lockoutOnFailure);
    }

    public override async Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure)
    {
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

        var user = await _dbContext.Users.Where(x => x.UserName == userName).FirstOrDefaultAsync();

        if (user is null)
            return await base.PasswordSignInAsync(userName, password, isPersistent, lockoutOnFailure);

        return await PasswordSignInAsync(user, password, isPersistent, lockoutOnFailure);
    }

    private async Task<UserSignInResult?> ValidateLinksAsync(DataCoreUser user)
    {
        var settings = await _accountLinkService.GetLinkSettingsAsync();

        if ((settings.RequireSteamLink && user.SteamLink is null)
            || (settings.RequireDiscordLink && user.DiscordId is null))
        {
            return new UserSignInResult()
            {
                RequiresAccountLinking = true,
                UserId = user.Id
            };
        }

        return null;
    }
}
