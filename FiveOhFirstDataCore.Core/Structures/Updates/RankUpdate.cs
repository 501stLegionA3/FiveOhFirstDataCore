using FiveOhFirstDataCore.Data.Account;

namespace FiveOhFirstDataCore.Data.Structures.Updates
{
    public class RankUpdate : UpdateBase
    {
        public int ChangedFrom { get; set; }
        public int ChangedTo { get; set; }

        public Trooper RequestedBy { get; set; }
        public int? RequestedById { get; set; }

        public List<Trooper> ApprovedBy { get; set; } = new();

        public bool Approved { get; set; }

        public Trooper DeniedBy { get; set; }
        public int? DeniedById { get; set; }
    }
}
