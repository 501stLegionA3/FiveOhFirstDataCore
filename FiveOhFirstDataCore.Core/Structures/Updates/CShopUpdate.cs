using FiveOhFirstDataCore.Data.Account;
using FiveOhFirstDataCore.Data.Structures;

namespace FiveOhFirstDataCore.Data.Structures.Updates
{
    public class CShopUpdate : UpdateBase
    {
        public CShop Added { get; set; }
        public CShop Removed { get; set; }

        public CShop OldCShops { get; set; }

        public Trooper ChangedBy { get; set; }
        public int? ChangedById { get; set; }
    }
}
