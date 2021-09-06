using FiveOhFirstDataCore.Core.Account;

namespace FiveOhFirstDataCore.Core.Structures.Notification
{
    public class NotificationTrackerBase
    {
        public Guid Key { get; set; }
        public Trooper NotificationFor { get; set; }
        public int NotificationForId { get; set; }

        public DateTime LastView { get; set; }
    }
}
