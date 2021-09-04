using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Components.Base
{
    public interface INotificationBase : IDisposable
    {
        public void TriggerNotificationUpdate();
    }
}
