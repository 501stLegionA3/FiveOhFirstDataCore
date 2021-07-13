using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Data.Promotions
{
    public enum PromotionBoardLevel : int
    {
        [Description("Squad")]
        Squad = 0,
        [Description("Platoon")]
        Platoon = 1,
        [Description("Company")]
        Compnay = 2,
        [Description("Battalion")]
        Battalion = 3
    }
}
