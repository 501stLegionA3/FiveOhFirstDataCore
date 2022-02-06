// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using ProjectDataCore.Data.Account;
using ProjectDataCore.Data.Structures.Account;
using ProjectDataCore.Data.Services.Account;
using ProjectDataCore.Data.Services.User;

namespace ProjectDataCore.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly DataCoreSignInManager _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly IAssignableDataService _assignableDataService;
        private readonly IAccountLinkService _linkService;
        private readonly IUserService _userService;

        public LoginModel(DataCoreSignInManager signInManager, IAssignableDataService assignableDataService, ILogger<LoginModel> logger,
            IAccountLinkService linkService, IUserService userService)
        {
            _signInManager = signInManager;
            _assignableDataService = assignableDataService;
            _logger = logger;
            _linkService = linkService;
            _userService = userService;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public LoginInputModel Login { get; set; }

        [BindProperty]
        public RegisterInputModel Register { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string ErrorMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class LoginInputModel
        {
            [Required]
            [Display(Name = "Username")]
            public string LoginUserName { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [DataType(DataType.Password)]
            public string LoginPassword { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Display(Name = "Remember me?")]
            public bool LoginRememberMe { get; set; }
        }
        
        public class RegisterInputModel
        {
            [Required]
            [Display(Name = "Username")]
            [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 4)]
            public string RegisterUserName { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Access Code", Description = "The access code your recruiter gave you to register with.")]
            public string RegisterAccessCode { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password", Description = "Your new password.")]
            public string RegisterPassword { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("RegisterPassword", ErrorMessage = "The password and confirmation password do not match.")]
            public string RegisterConfirmPassword { get; set; }
        }

        public bool RequireAccessCode { get; set; } = false;
        [TempData]
        public string Notice { get; set; }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ReturnUrl = returnUrl;

            var settings = await _linkService.GetLinkSettingsAsync();
            RequireAccessCode = settings.RequireAccessCodeForRegister;
        }

        public async Task<IActionResult> OnPostRegisterAsync(string? returnUrl = null)
        {
            var settings = await _linkService.GetLinkSettingsAsync();
            RequireAccessCode = settings.RequireAccessCodeForRegister;
            Notice = null;

            bool valid = true;
            foreach (var item in ModelState)
            {
                if (item.Key.StartsWith("Register"))
                {
                    if (item.Key.Contains("AccessCode") && !RequireAccessCode)
                        continue;

                    if (item.Value.Errors.Count > 0)
                    {
                        valid = false;
                        break;
                    }
                }
            }

            if (valid)
            {
                var res = await _userService.CreateOrUpdateAccountAsync(RequireAccessCode ? Register.RegisterAccessCode : null,
                    Register.RegisterUserName, Register.RegisterPassword);

                if(res.GetResult(out var err))
                {
                    Notice = "Account registered successfully. You may now log in.";
                }
                else
                {
                    ModelState.AddModelError(String.Empty, err.FirstOrDefault());
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostLoginAsync(string returnUrl = null)
        {
            var settings = await _linkService.GetLinkSettingsAsync();
            RequireAccessCode = settings.RequireAccessCodeForRegister;
            Notice = null;

            returnUrl ??= Url.Content("~/");

            bool valid = true;
            foreach (var item in ModelState)
            {
                if (item.Key.StartsWith("Login"))
                {
                    if (item.Value.Errors.Count > 0)
                    {
                        valid = false;
                        break;
                    }
                }
            }

            if (valid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(Login.LoginUserName, Login.LoginPassword, Login.LoginRememberMe, lockoutOnFailure: false);

                if (result is UserSignInResult res)
                {
                    if(res.RequiresAccountLinking)
                    {
                        var token = await _linkService.StartAsync(res.UserId, Login.LoginUserName, Login.LoginPassword, Login.LoginRememberMe);
                        return Redirect($"/api/link/token/{token}");
                    }
                }

                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");

                    return LocalRedirect(returnUrl);
                } 
                else if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Login.LoginRememberMe });
                }
                else if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
