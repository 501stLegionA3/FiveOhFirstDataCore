using FiveOhFirstDataCore.Data.Account;
using FiveOhFirstDataCore.Data.Mail;
using FiveOhFirstDataCore.Data.Structuresbase;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;

using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;

namespace FiveOhFirstDataCore.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<Trooper> _userManager;
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
        private readonly ICustomMailSender _emailSender;

        public ForgotPasswordModel(UserManager<Trooper> userManager, ICustomMailSender emailSender,
            IDbContextFactory<ApplicationDbContext> dbContextFactory)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _dbContextFactory = dbContextFactory;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "Birth Number")]
            public int BirthNumber { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                await using var _dbContext = _dbContextFactory.CreateDbContext();
                var user = await _dbContext.Users
                    .Where(x => x.BirthNumber == Input.BirthNumber)
                    .FirstOrDefaultAsync();
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToPage("./ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please 
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ResetPassword",
                    pageHandler: null,
                    values: new { area = "Identity", code },
                    protocol: Request.Scheme);

                await _emailSender.SendEmailAsync(
                    user.Email,
                    "Reset Password",
                    $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            return Page();
        }
    }
}
