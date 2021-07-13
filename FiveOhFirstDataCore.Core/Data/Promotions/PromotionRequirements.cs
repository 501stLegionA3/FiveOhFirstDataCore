using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Data.Promotions
{
    public class PromotionRequirements
    {
        public int RequirementsFor { get; set; }

        public List<int>? RequiredRank { get; set; } = null;
        public List<int>? InherentRankAuth { get; set; } = null;
        public bool RankOrHigher { get; set; } = false;
        public int RequiredTimeInGrade { get; set; } = -1;
        public List<int>? TiGWaivedFor { get; set; } = null;

        public List<Role>? RequiredBillet { get; set; } = null;
        public int RequiredTimeInBillet { get; set; } = -1;

        /// <summary>
        /// If a slot value divided by the first item must be equal to 0 or not.
        /// If the value is null, this check is skipped.
        /// </summary>
        public int? DivideEqualsZero { get; set; } = null;

        /// <summary>
        /// Require a slot value between item 1 incluseve and item 2 exclusive. 
        /// Follows slot >= Item1 && slot < Item2
        /// </summary>
        public (Slot, Slot)? RequireSlotBetween { get; set; } = null;

        public bool TeamMustBeNull { get; set; } = false;

        public Qualification RequiredQualifications { get; set; } = Qualification.None;
        public bool RequiresCShop { get; set; } = false;
        public bool RequiresCShopLeadership { get; set; } = false;
        public bool RequiresCShopCommand { get; set; } = false;

        public PromotionBoardLevel NeededLevel { get; set; } = PromotionBoardLevel.Platoon;
    }
}
