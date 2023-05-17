using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Data.Mail;
public class MailConfiguration
{
    public bool UseEmailServer { get; set; } = false;
    public GoogleConfiguration? Google { get; set; }
    public SmtpMailConfiguration? Server { get; set; }
}
