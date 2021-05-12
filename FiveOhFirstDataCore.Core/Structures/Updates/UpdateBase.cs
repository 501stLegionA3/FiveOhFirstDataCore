using FiveOhFirstDataCore.Core.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Structures.Updates
{
    public abstract class UpdateBase
    {
        public Guid ChangeId { get; set; }
        public DateTime ChangedOn { get; set; }
        public Trooper ChangeFor { get; set; }
        public int ChangeForId { get; set; }
        public bool SubmittedByRosterClerk { get; set; } = false;
    }
}
