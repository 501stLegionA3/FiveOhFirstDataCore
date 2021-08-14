using FiveOhFirstDataCore.Core.Data;
using FiveOhFirstDataCore.Core.Structures.Updates;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Account
{
    public class TrooperChangeRequestData
    {
        public Guid Key { get; set; }

        public Trooper ChangeFor { get; set; }
        public int ChangeForId { get; set; }

        public TrooperRank? Rank { get; set; }
        public RTORank? RTORank{ get; set; }
        public MedicRank? MedicRank { get; set; }
        public WarrantRank? WarrantRank { get; set; }
        public WardenRank? WardenRank { get; set; }
        public PilotRank? PilotRank { get; set; }

        public Role? Role { get; set; }
        public Team? Team { get; set; }
        public Flight? Flight { get; set; }
        public Slot? Slot { get; set; }

        public Qualification Qualifications { get; set; }

        public DateTime? LastPromotion { get; set; }
        public DateTime? StartOfService { get; set; }
        public DateTime? LastBilletChange { get; set; }
        public DateTime? GraduatedBCTOn { get; set; }
        public DateTime? GraduatedUTCOn { get; set; }

        public string? AdditionalChanges { get; set; }

        public string? Reason { get; set; }
    }
}
