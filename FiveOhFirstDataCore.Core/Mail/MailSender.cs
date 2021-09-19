using FiveOhFirstDataCore.Data.Account;

using MailKit.Net.Smtp;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

using MimeKit;

using System.Text.Encodings.Web;

namespace FiveOhFirstDataCore.Data.Mail
{
    public class MailSender : ICustomMailSender
    {
        private readonly MailConfiguration _config;
        private readonly ILogger _logger;
        private readonly SmtpClient _noReplyClient;
        private readonly SmtpClient _userClient;
        private readonly UserManager<Trooper> _userManager;

        public MailSender(MailConfiguration config, ILogger<MailSender> logger, SmtpClient noReplyClient, SmtpClient userClient,
            UserManager<Trooper> userManager)
        {
            _config = config;
            _logger = logger;
            _noReplyClient = noReplyClient;
            _userClient = userClient;
            _userManager = userManager;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            if (MailboxAddress.TryParse(_config.Email, out var sender))
            {
                if (MailboxAddress.TryParse(email, out var receiver))
                {
                    var message = new MimeMessage();
                    message.From.Add(sender);
                    message.To.Add(receiver);
                    message.Subject = subject;
                    message.Body = new TextPart("html")
                    {
                        Text = htmlMessage
                    };
                    try
                    {
                        if (!_noReplyClient.IsConnected)
                        {
                            await _noReplyClient.ConnectAsync(_config.Client, _config.Port, false);
                            if (_config.RequireLogin)
                                await _noReplyClient.AuthenticateAsync(_config.User, _config.Password);
                        }

                        await _noReplyClient.SendAsync(message);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"An email failed to send.");
                    }
                }
                else
                {
                    _logger.LogError($"Invalid email address received: {email}");
                    return;
                }
            }
            else
            {
                throw new Exception("The configured email is invalid.");
            }

        }

        public async Task TriggerRemoteResetPasswordAsync(string id, string email, string redirectBase)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user is null) return;

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = $"{redirectBase}/Identity/Account/ResetPassword?code={code}";

            await SendEmailAsync(
                    email,
                    "Reset Password",
                    $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
        }
    }
}
