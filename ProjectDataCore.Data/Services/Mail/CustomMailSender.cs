using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MailKit;
using MailKit.Net.Smtp;
using ProjectDataCore.Data.Account;
using Microsoft.AspNetCore.Identity;
using MimeKit;
using System.Text.Encodings.Web;
using ProjectDataCore.Data.Structures.Mail;

namespace ProjectDataCore.Data.Services.Mail;
public class CustomMailSender : ICustomMailSender
{
    private readonly MailConfiguration _config;
    private readonly ILogger _logger;
    private readonly SmtpClient _noReplyClient;
    private readonly SmtpClient _userClient;
    private readonly UserManager<DataCoreUser> _userManager;

    public CustomMailSender(MailConfiguration config, ILogger<CustomMailSender> logger, SmtpClient noReplyClient, SmtpClient userClient,
        UserManager<DataCoreUser> userManager)
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

    public async Task TriggerRemoteResetPasswordAsync(Guid id, string email, string redirectBase)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user is null) return;

        var code = await _userManager.GeneratePasswordResetTokenAsync(user);
        var callbackUrl = $"{redirectBase}/password/reset?code={code}";

        await SendEmailAsync(
                email,
                "Reset Password",
                $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
    }
}
