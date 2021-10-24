using System.ComponentModel;

namespace FiveOhFirstDataCore.Data.Structures
{
    public enum Role
    {
        /// <summary>
        /// SL/TL
        /// </summary>
        [Description("Leader")]
        Lead,
        [Description("RT")]
        RTO,
        [Description("Trooper")]
        Trooper,
        [Description("Medic")]
        Medic,
        [Description("ARC")]
        ARC,
        /// <summary>
        /// Platoon/Company/Squadron/Battalion Commander
        /// </summary>
        [Description("Commander")]
        Commander,
        /// <summary>
        /// Platoon SGM
        /// </summary>
        [Description("Sergeant-Major")]
        SergeantMajor,
        /// <summary>
        /// Company/Battalion XO
        /// </summary>
        [Description("XO")]
        XO,
        /// <summary>
        /// Company/Battalion NCOIC
        /// </summary>
        [Description("NCOIC")]
        NCOIC,
        [Description("Adjutant")]
        Adjutant,
        [Description("C-Shop XO")]
        CShopXO,
        [Description("C-Shop Commander")]
        CShopCommander,
        /// <summary>
        /// Squadron Sub-Commander
        /// </summary>
        [Description("Sub-Commander")]
        SubCommander,
        /// <summary>
        /// Warden CO
        /// </summary>
        [Description("Master Warden")]
        MasterWarden,
        /// <summary>
        /// Warden XO
        /// </summary>
        [Description("Chief Warden")]
        ChiefWarden,
        [Description("Warden")]
        Warden,
        [Description("Pilot")]
        Pilot,
        [Description("Subordinate")]
        Subordinate,
        /// <summary>
        /// Razor Adjutant
        /// </summary>
        [Description("Squadron Chief Logistics Officer")]
        SCLO
    }

    public static class RoleExtensions
    {
        public static Role? GetRole(this string value)
        {
            // RT and RTO are used interchangeably, so we want to check for the use of RTO.
            if (value.Equals("RTO")) return Role.RTO;
            // ARC and ARC Trooper are used interchangeably, so we want to check for the use of ARC Trooper.
            if (value.Equals("ARC Trooper")) return Role.ARC;

            foreach (Role enumValue in Enum.GetValues(typeof(Role)))
            {
                if (enumValue.AsFull() == value)
                    return enumValue;
            }

            return null;
        }
    }
}
