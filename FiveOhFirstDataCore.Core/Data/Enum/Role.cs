using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

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
        [Description("ARC Trooper")]
        ARC,
        [Description("Commander")]
        Commander,
        [Description("Sergeant Major")]
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
        [Description("Cheif Warden")]
        CheifWarden,
        [Description("Warden")]
        Warden,
        [Description("Pilot")]
        Pilot,
        [Description("Subordinate")]
        Subordiante
    }
}
