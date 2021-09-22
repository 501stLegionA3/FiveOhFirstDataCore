using FiveOhFirstDataCore.Data.Account;
using FiveOhFirstDataCore.Data.Structuresbase;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using System.ComponentModel.DataAnnotations;
using System.Net;

namespace FiveOhFirstDataCore.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly TrooperSignInManager _signInManager;
        private readonly UserManager<Trooper> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

        public RegisterModel(
            UserManager<Trooper> userManager,
            TrooperSignInManager signInManager,
            ILogger<RegisterModel> logger,
            IDbContextFactory<ApplicationDbContext> dbContextFactory)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _dbContextFactory = dbContextFactory;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string? ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 4)]
            [Display(Name = "Account Username\n(Does not have to be your Trooper name)", Description = "The username you will use to sign in with. This is not your display/trooper name.")]
            public string UserName { get; set; }

            [Required]
            [Display(Name = "Birth Number", Description = "The birth number you picked with your recruiter.")]
            public int BirthNumber { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Access Code", Description = "The access code your recruiter gave you to register with.")]
            public string AccessCode { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password", Description = "Your new password.")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task OnGetAsync(string? returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            if (ModelState.IsValid)
            {
                await using var _dbContext = _dbContextFactory.CreateDbContext();
                var userObject = await _dbContext.Users.AsNoTracking()
                    .Where(x => x.BirthNumber == Input.BirthNumber)
                    .SingleOrDefaultAsync();

                if (userObject is not null)
                {

                    var user = await _userManager.FindByIdAsync(userObject.Id.ToString());

                    if (user is not null)
                    {
                        if (user.AccessCode == Input.AccessCode)
                        {
                            var removeRes = await _userManager.RemovePasswordAsync(user);

                            if (removeRes.Succeeded)
                            {

                                var passChangeRes = await _userManager.AddPasswordAsync(user, Input.Password);

                                if (passChangeRes.Succeeded)
                                {
                                    user.UserName = Input.UserName;
                                    user.AccessCode = null;
                                    var identResult = await _userManager.UpdateAsync(user);

                                    if (!identResult.Succeeded)
                                    {
                                        foreach (var error in identResult.Errors)
                                        {
                                            ModelState.AddModelError(error.Code, error.Description);
                                        }
                                    }
                                    else
                                    {
                                        return Redirect($"/Identity/Account/Login?message={WebUtility.HtmlEncode("Your account has been created. Login to link your account.")}");
                                    }
                                }
                                else
                                {
                                    foreach (var error in passChangeRes.Errors)
                                    {
                                        ModelState.AddModelError(error.Code, error.Description);
                                    }
                                }
                            }
                            else
                            {
                                foreach (var error in removeRes.Errors)
                                {
                                    ModelState.AddModelError(error.Code, error.Description);
                                }
                            }
                        }
                        else
                        {
                            ModelState.AddModelError("Invalid Access Code", "The inputed Access Code did not match the one saved to this trooper.");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("Invalid Birth Number", "The inputed Birth Number does match an exsisting ID.");
                    }
                }
                else
                {
                    ModelState.AddModelError("Invalid Birth Number", "The inputed Birth Number does not exsist.");
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
