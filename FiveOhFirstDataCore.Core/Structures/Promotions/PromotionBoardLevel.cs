using System.ComponentModel;

namespace FiveOhFirstDataCore.Data.Structures.Promotions
{
    public enum PromotionBoardLevel : int
    {
        [Description("Squad")]
        Squad = 0,
        [Description("Platoon")]
        Platoon = 1,
        [Description("Company")]
        Company = 2,
        [Description("Battalion")]
        Battalion = 3,
        [Description("Razor")]
        Razor = 4,

        [Description("Warden")]
        Warden = 5,
    }
}
