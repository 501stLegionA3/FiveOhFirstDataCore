using FiveOhFirstDataCore.Core.Data;

namespace FiveOhFirstDataCore.Core.Structures
{
    public class CShopClaim
    {
        public CShop Key { get; set; }
        public Dictionary<string, List<string>> ClaimData { get; set; } = new();
        public List<string> CShopLeadership { get; set; } = new();
        public List<string> CShopCommand { get; set; } = new();
    }
}
