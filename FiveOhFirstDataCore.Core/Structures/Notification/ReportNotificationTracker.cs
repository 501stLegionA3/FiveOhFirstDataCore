using FiveOhFirstDataCore.Core.Account.Detail;

namespace FiveOhFirstDataCore.Core.Structures.Notification
{
    public class ReportNotificationTracker : NotificationTrackerBase
    {
        public Guid ReportId { get; set; }
        public TrooperReport Report { get; set; }
    }
}
