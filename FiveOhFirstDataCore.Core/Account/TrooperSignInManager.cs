using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FiveOhFirstDataCore.Data.Account
{
    public class TrooperSignInManager : SignInManager<Trooper>
    {
        public TrooperSignInManager(UserManager<Trooper> userManager, IHttpContextAccessor contextAccessor, IUserClaimsPrincipalFactory<Trooper> claimsFactory, IOptions<IdentityOptions> optionsAccessor, ILogger<SignInManager<Trooper>> logger, IAuthenticationSchemeProvider schemes, IUserConfirmation<Trooper> confirmation)
            : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation) { }

        public override async Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure)
        {
            Trooper user;
            if (int.TryParse(userName, out _))
            {
                user = await UserManager.FindByIdAsync(userName);
            }
            else
            {
                user = await UserManager.FindByNameAsync(userName);
            }

            if (user is not null)
            {
                return await PasswordSignInAsync(user, password, isPersistent, lockoutOnFailure);
            }

            return await base.PasswordSignInAsync(userName, password, isPersistent, lockoutOnFailure);
        }

        public override Task<SignInResult> PasswordSignInAsync(Trooper user, string password, bool isPersistent, bool lockoutOnFailure)
        {
            if (user.DiscordId is null || user.SteamLink is null)
            {
                return Task.FromResult<SignInResult>(new TrooperSignInResult()
                {
                    RequiresAccountLinking = true,
                    TrooperId = user.Id
                });
            }

            return base.PasswordSignInAsync(user, password, isPersistent, lockoutOnFailure);
        }

        public override Task SignInAsync(Trooper user, bool isPersistent, string? authenticationMethod = null)
        {
            if (user.DiscordId is null || user.SteamLink is null)
            {
                return Task.FromResult<SignInResult>(new TrooperSignInResult()
                {
                    RequiresAccountLinking = true,
                    TrooperId = user.Id
                });
            }

            return base.SignInAsync(user, isPersistent, authenticationMethod);
        }
    }

    public class TrooperSignInResult : SignInResult
    {
        public bool RequiresAccountLinking { get; set; }
        public int TrooperId { get; set; }
    }
}
