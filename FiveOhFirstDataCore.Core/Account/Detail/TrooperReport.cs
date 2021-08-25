using FiveOhFirstDataCore.Core.Data;

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

        public Slot ReportFor { get; set; }

        public string ReportDetails { get; set; }
    }
}
