using Microsoft.AspNetCore.Identity.UI.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Services.Mail;
public interface ICustomMailSender : IEmailSender
{
    public Task TriggerRemoteResetPasswordAsync(Guid id, string email, string redirectBase);
}
