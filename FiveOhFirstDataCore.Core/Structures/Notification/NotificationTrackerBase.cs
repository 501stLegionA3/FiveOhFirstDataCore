using FiveOhFirstDataCore.Data.Account;

namespace FiveOhFirstDataCore.Data.Structures.Notification
{
    public class NotificationTrackerBase
    {
        public Guid Key { get; set; }
        public Trooper NotificationFor { get; set; }
        public int NotificationForId { get; set; }

        public DateTime LastView { get; set; }
    }
}
