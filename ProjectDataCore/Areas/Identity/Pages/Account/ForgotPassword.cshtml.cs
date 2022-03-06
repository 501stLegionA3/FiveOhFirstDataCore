using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

using ProjectDataCore.Data.Services.Alert;
using ProjectDataCore.Data.Services.Mail;
using ProjectDataCore.Data.Structures.Model.Alert;

using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;

namespace ProjectDataCore.Areas.Identity.Pages.Account;

[AllowAnonymous]
public class ForgotPasswordModel : PageModel
{
    private readonly UserManager<DataCoreUser> _userManager;
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
    private readonly ICustomMailSender _emailSender;
    private readonly IAlertService _alertService;

    public ForgotPasswordModel(UserManager<DataCoreUser> userManager, ICustomMailSender emailSender,
        IDbContextFactory<ApplicationDbContext> dbContextFactory, IAlertService alertService)
    {
        _userManager = userManager;
        _emailSender = emailSender;
        _dbContextFactory = dbContextFactory;
        _alertService = alertService;
    }

    [BindProperty]
    public InputModel Input { get; set; }

    public class InputModel
    {
        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (ModelState.IsValid)
        {
            await using var _dbContext = _dbContextFactory.CreateDbContext();
            var user = await _dbContext.Users
                .Where(x => x.UserName == Input.UserName)
                .FirstOrDefaultAsync();
            if (user == null)
            {
                return Redirect($"/?alert={HtmlEncoder.Default.Encode("No account was found for the provided username.")};" +
                    $"{AlertType.Warn};" +
                    $"true;" +
                    $"3600");
            }

            if(!await _userManager.IsEmailConfirmedAsync(user))
            {
                var msg = "No email is configured for the provided username. Please contact" +
                    "a website administartor to reset your password.";

                return Redirect($"/?alert={HtmlEncoder.Default.Encode(msg)};" +
                    $"{AlertType.Warn};" +
                    $"true;" +
                    $"3600");
            }

            // For more information on how to enable account confirmation and password reset please 
            // visit https://go.microsoft.com/fwlink/?LinkID=532713
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/password/reset",
                pageHandler: null,
                values: new { area = "Identity", code },
                protocol: Request.Scheme);

            await _emailSender.SendEmailAsync(
                user.Email,
                "Reset Password",
                $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            return Redirect($"/?alert={HtmlEncoder.Default.Encode("A password reset has been sent to the email on your account.")};" +
                $"{AlertType.Success};" +
                $"true;" +
                $"3600");
        }

        return Page();
    }
}