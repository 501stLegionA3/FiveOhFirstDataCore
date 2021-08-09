using System;
using System.ComponentModel;

namespace FiveOhFirstDataCore.Core.Data
{
    public enum Role
    {
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
        [Description("Commander")]
        Commander,
        [Description("Sergeant-Major")]
        SergeantMajor,
        [Description("XO")]
        XO,
        [Description("NCOIC")]
        NCOIC,
        [Description("Adjutant")]
        Adjutant,
        [Description("C-Shop XO")]
        CShopXO,
        [Description("C-Shop Commander")]
        CShopCommander,
        [Description("Sub-Commander")]
        SubCommander,
        [Description("Master Warden")]
        MasterWarden,
        [Description("Chief Warden")]
        ChiefWarden,
        [Description("Warden")]
        Warden,
        [Description("Pilot")]
        Pilot,
        [Description("Subordinate")]
        Subordinate,
        [Description("FOIC")]
        FOIC
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
