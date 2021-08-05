﻿using System;
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
        Company = 2,
        [Description("Battalion")]
        Battalion = 3,
        [Description("Razor")]
        Razor = 4,

        [Description("Warden")]
        Warden = 5,
    }
}
