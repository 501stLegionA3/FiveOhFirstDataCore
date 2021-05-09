using System;
using System.Linq;
using System.Reflection;

namespace FiveOhFirstDataCore.Core.Data
{
    public enum Role
    {
        [RoleDetails("Leader")]
        Lead,
        [RoleDetails("RT")]
        RTO,
        [RoleDetails("Trooper")]
        Trooper,
        [RoleDetails("Medic")]
        Medic,
        [RoleDetails("ARC Trooper")]
        ARC,
        [RoleDetails("Commander")]
        Commander,
        [RoleDetails("Sergeant Major")]
        SergeantMajor,
        [RoleDetails("XO")]
        XO,
        [RoleDetails("NCOIC")]
        NCOIC,
        [RoleDetails("Adjutant")]
        Adjutant,
        [RoleDetails("C-Shop XO")]
        CShopXO,
        [RoleDetails("C-Shop Commander")]
        CShopCommander,
        [RoleDetails("Sub-Commander")]
        SubCommander,
        [RoleDetails("Master Warden")]
        MasterWarden,
        [RoleDetails("Cheif Warden")]
        CheifWarden,
        [RoleDetails("Warden")]
        Warden,
        [RoleDetails("Pilot")]
        Pilot,
        [RoleDetails("Subordinate")]
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
                ?.GetCustomAttributes<RoleDetailsAttribute>()
                .SingleOrDefault();

            if (attribute is null) return "";
            else return attribute.Name;
        }
    }
}
