using FiveOhFirstDataCore.Core.Structures;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Services
{
    public interface IAlertService
    {
        public event EventHandler<AlertData> OnAlertPosted;
        public void PostAlert(object sender, AlertData alert);
    }
}
