using FiveOhFirstDataCore.Core.Account;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Structures.Notification
{
    public class NotificationTrackerBase
    {
        public Guid Key { get; set; }
        public Trooper NotificationFor { get; set; }
        public int NotificationForId { get; set; }

        public DateTime LastView { get; set; }

        public Guid TrackerFor { get; set; }
    }
}
