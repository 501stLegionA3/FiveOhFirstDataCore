using FiveOhFirstDataCore.Core.Data;
using FiveOhFirstDataCore.Core.Structures.Notification;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Account.Detail
{
    public class TrooperReport
    {
        public Guid Id { get; set; }

        public Trooper ReportedBy { get; set; }
        public int ReportedById { get; set; }

        public Slot ReportViewableAt { get; set; }

        public bool ElevatedToBattalion { get; set; } = false;

        public bool Public { get; set; } = false;

        public List<string> Responses { get; set; } = new();

        public bool Resolved { get; set; } = false;

        public string Summary { get; set; }

        public DateTime LastUpdate { get; set; }
        public DateTime SubmittedOn { get; set; }
        public List<ReportNotificationTracker> NotificationTrackers { get; set; } = new();
    }
}
