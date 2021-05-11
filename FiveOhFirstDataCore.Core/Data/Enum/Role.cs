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

    [AttributeUsage(AttributeTargets.All)]
    public class RoleDetailsAttribute : Attribute
    {
        public string Name { get; set; }
        public RoleDetailsAttribute(string name)
        {
            Name = name;
        }
    }

    public static class RoleExtensions
    {
        public static string AsName(this Role role)
        {
            var type = typeof(Role);
            var name = Enum.GetName(type, role);

            if (name is null) return "";

            var attribute = type?.GetField(name)
                ?.GetCustomAttributes<DescriptionAttribute>()
                .SingleOrDefault();

            if (attribute is null) return "";
            else return attribute.Description;
        }
    }
}
