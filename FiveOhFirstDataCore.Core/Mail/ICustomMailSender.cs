using Microsoft.AspNetCore.Identity.UI.Services;

using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Mail
{
    public interface ICustomMailSender : IEmailSender
    {
        public Task TriggerRemoteResetPasswordAsync(string id, string email, string redirectBase);
    }
}
