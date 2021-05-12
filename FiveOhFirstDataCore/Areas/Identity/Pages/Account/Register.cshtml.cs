using FiveOhFirstDataCore.Core.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly TrooperSignInManager _signInManager;
        private readonly UserManager<Trooper> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly AccountLinkService _link;

        public RegisterModel(
            UserManager<Trooper> userManager,
            TrooperSignInManager signInManager,
            ILogger<RegisterModel> logger,
            AccountLinkService link)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _link = link;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string? ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 4)]
            [Display(Name = "Account Username (Not your Trooper name)", Description = "The username you will use to sign in with. This is not your display/trooper name.")]
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
                var user = await _userManager.FindByIdAsync(Input.BirthNumber.ToString());

                if (user is not null)
                {
                    if (user.AccessCode == Input.AccessCode)
                    {
                        var passChangeRes = await _userManager.ChangePasswordAsync(user, user.AccessCode, Input.Password);

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
                                var res = await _signInManager.PasswordSignInAsync(user, Input.Password, false, false);

                                if (res is TrooperSignInResult result)
                                {
                                    if (result.RequiresAccountLinking)
                                    {
                                        var token = await _link.StartAsync(user.Id, user.UserName, Input.Password, false);
                                        returnUrl = $"/api/link/token/{token}";
                                    }
                                }

                                return Redirect(returnUrl);
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
                        ModelState.AddModelError("Invalid Access Code", "The inputed Access Code did not match the one saved to this trooper.");
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
