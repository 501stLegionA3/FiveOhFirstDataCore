using FiveOhFirstDataCore.Core.Structures;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Services
{
    public class AlertService : IAlertService
    {
        public event EventHandler<AlertData> OnAlertPosted;

        public void PostAlert(object sender, AlertData alert)
        {
            try
            {
                OnAlertPosted.Invoke(sender, alert);
            }
            catch
            {
                // do nothing, as we have nothing listening.
            }
        }
    }
}
