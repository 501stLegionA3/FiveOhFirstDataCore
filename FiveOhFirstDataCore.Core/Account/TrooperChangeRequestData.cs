using FiveOhFirstDataCore.Data.Structures;
using FiveOhFirstDataCore.Data.Structures.Updates;

namespace FiveOhFirstDataCore.Data.Account
{
    public class TrooperChangeRequestData : UpdateBase
    {
        public TrooperRank? Rank { get; set; }
        public RTORank? RTORank { get; set; }
        public MedicRank? MedicRank { get; set; }
        public WarrantRank? WarrantRank { get; set; }
        public WardenRank? WardenRank { get; set; }
        public PilotRank? PilotRank { get; set; }

        public Role? Role { get; set; }
        public Team? Team { get; set; }
        public Flight? Flight { get; set; }
        public Slot? Slot { get; set; }

        public Qualification Qualifications { get; set; }
        public bool QualsChanged { get; set; } = false;

        public DateTime? LastPromotion { get; set; }
        public DateTime? StartOfService { get; set; }
        public DateTime? LastBilletChange { get; set; }
        public DateTime? GraduatedBCTOn { get; set; }
        public DateTime? GraduatedUTCOn { get; set; }

        public string? AdditionalChanges { get; set; }

        public string? Reason { get; set; }

        public Trooper? FinalizedBy { get; set; }
        public int? FinalizedById { get; set; }

        public bool Finalized { get; set; } = false;
        public bool Approved { get; set; } = false;

        public bool IsValid()
        {
            if (Reason is null) return false;

            // If one of these has a value, its a valid change request.
            return HasChange();
        }

        public bool HasChange(bool includeAdditionalChanges = true)
        {
            var val = QualsChanged 
                || !(Rank is null
                    && RTORank is null
                    && MedicRank is null
                    && WarrantRank is null
                    && WardenRank is null
                    && PilotRank is null
                    && Role is null
                    && Team is null
                    && Flight is null
                    && Slot is null
                    && LastPromotion is null
                    && StartOfService is null
                    && LastBilletChange is null
                    && GraduatedBCTOn is null
                    && GraduatedUTCOn is null);

            if (includeAdditionalChanges)
                val = val || AdditionalChanges is not null;

            return val;
        }
    }
}
