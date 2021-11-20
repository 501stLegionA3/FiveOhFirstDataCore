using FiveOhFirstDataCore.Data.Account;

namespace FiveOhFirstDataCore.Data.Structures.Promotions
{
    public class Promotion
    {
        public Guid Id { get; set; }

        public Trooper PromotionFor { get; set; }
        public int PromotionForId { get; set; }

        public Trooper RequestedBy { get; set; }
        public int? RequestedById { get; set; }

        public List<Trooper> ApprovedBy { get; set; } = new();

        public PromotionBoardLevel StartingBoard { get; set; }
        public PromotionBoardLevel NeededBoard { get; set; }
        public PromotionBoardLevel CurrentBoard { get; set; }

        public int PromoteFrom { get; set; }
        public int PromoteTo { get; set; }

        public string Reason { get; set; } = "";
        public bool Forced { get; set; } = false;
    }
}
