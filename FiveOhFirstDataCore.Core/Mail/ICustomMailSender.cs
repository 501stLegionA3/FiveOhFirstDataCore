using Microsoft.AspNetCore.Identity.UI.Services;

namespace FiveOhFirstDataCore.Data.Mail
{
    public interface ICustomMailSender : IEmailSender
    {
        public Task TriggerRemoteResetPasswordAsync(string id, string email, string redirectBase);
    }
}
