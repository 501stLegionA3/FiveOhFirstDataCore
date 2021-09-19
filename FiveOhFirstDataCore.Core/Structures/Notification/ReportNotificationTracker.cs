using FiveOhFirstDataCore.Data.Account.Detail;

namespace FiveOhFirstDataCore.Data.Structures.Notification
{
    public class ReportNotificationTracker : NotificationTrackerBase
    {
        public Guid ReportId { get; set; }
        public TrooperReport Report { get; set; }
    }
}
