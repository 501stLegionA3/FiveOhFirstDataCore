using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Mail
{
    public class MailSender : IEmailSender
    {
        private readonly MailConfiguration _config;
        private readonly ILogger _logger;
        private readonly SmtpClient _noReplyClient;
        private readonly SmtpClient _userClient;

        public MailSender(MailConfiguration config, ILogger<MailSender> logger, SmtpClient noReplyClient, SmtpClient userClient)
        {
            _config = config;
            _logger = logger;
            _noReplyClient = noReplyClient;
            _userClient = userClient;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            if(MailboxAddress.TryParse(_config.Email, out var sender))
            {
                if(MailboxAddress.TryParse(email, out var receiver))
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
                throw new Exception("The configured email is inavlid.");
            }
            
        }
    }
}
