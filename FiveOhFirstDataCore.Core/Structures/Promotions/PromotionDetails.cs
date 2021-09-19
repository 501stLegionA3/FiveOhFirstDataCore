﻿using FiveOhFirstDataCore.Data.Account;
using FiveOhFirstDataCore.Data.Extensions;

using System.Diagnostics.CodeAnalysis;

namespace FiveOhFirstDataCore.Data.Structures.Promotions
{
    public class PromotionDetails
    {
        public int RequirementsFor { get; set; }

        public List<int> RequiredRank { get; set; } = new();
        public List<int> InherentRankAuth { get; set; } = new();
        public bool RankOrHigher { get; set; } = false;
        public int RequiredTimeInGrade { get; set; } = -1;
        public List<int> TiGWaivedFor { get; set; } = new();

        public List<Role> RequiredBillet { get; set; } = new();
        public int RequiredTimeInBillet { get; set; } = -1;

        /// <summary>
        /// If a slot value divided by the first item must be equal to 0 or not.
        /// If the value is null, this check is skipped.
        /// </summary>
        public int? DivideEqualsZero { get; set; } = null;

        public Slot? SlotMin { get; set; } = null;
        public Slot? SlotMax { get; set; } = null;

        public bool TeamMustBeNull { get; set; } = false;

        public Qualification RequiredQualifications { get; set; } = Qualification.None;
        public bool RequiresCShop { get; set; } = false;
        public bool RequiresCShopLeadership { get; set; } = false;
        public bool RequiresCShopCommand { get; set; } = false;

        public PromotionBoardLevel NeededLevel { get; set; } = PromotionBoardLevel.Platoon;

        public List<int> CanPromoteTo { get; set; } = new();
        public bool DoesNotRequireLinearProgression { get; set; } = false;

        public bool TryGetPromotion(int currentRank, Trooper trooper, (bool, bool) cshopLevels,
            [NotNullWhen(true)] out Promotion? promotion)
        {
            promotion = null;

            if (RequiredTimeInGrade != -1 && RequiredRank.Count > 0)
                if (!RequiredRank.Contains(currentRank)
                    || trooper.LastPromotion.DaysFromToday() < RequiredTimeInGrade)
                    if (!(RankOrHigher && RequiredRank.Min() <= currentRank))
                        if (TiGWaivedFor.Count <= 0 || !TiGWaivedFor.Contains(currentRank))
                            return false;

            // Test to see if the non-linear promotion is in the same promotion tree
            // (medic to medic, RTO to RTO).
            if (DoesNotRequireLinearProgression)
                if ((currentRank / 100) != (RequirementsFor / 100))
                    return false;

            if (RequiredTimeInBillet != -1 && RequiredBillet.Count > 0)
                if (!RequiredBillet.Contains(trooper.Role))
                    return false;

            if (DivideEqualsZero is not null
                && ((int)trooper.Slot / DivideEqualsZero) != 0)
                return false;

            if ((SlotMin is not null && trooper.Slot < SlotMin)
                || (SlotMax is not null && trooper.Slot > SlotMax))
                return false;

            if (TeamMustBeNull && trooper.Team is not null)
                return false;

            if (RequiredQualifications != Qualification.None)
                foreach (Qualification q in Enum.GetValues(typeof(Qualification)))
                    if (q != Qualification.None
                        && (RequiredQualifications & q) == q
                        && (trooper.Qualifications & q) != q)
                        return false;

            if (RequiresCShop)
                if (trooper.CShops == CShop.None)
                    return false;

            if (RequiresCShopLeadership && !cshopLevels.Item2)
                return false;

            if (RequiresCShopCommand && !cshopLevels.Item1)
                return false;

            promotion = new()
            {
                PromoteFrom = currentRank,
                PromoteTo = RequirementsFor,
                PromotionFor = trooper,
                PromotionForId = trooper.Id
            };

            return true;
        }
    }
}
