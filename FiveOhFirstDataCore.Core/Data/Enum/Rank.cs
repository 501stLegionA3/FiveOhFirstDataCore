using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace FiveOhFirstDataCore.Core.Data
{
    public enum TrooperRank
    {
        [RankDisplay("CR")]
        [Description("Recruit")]
        Recruit,
        [RankDisplay("CR-C")]
        [Description("Cadet")]
        Cadet,
        [RankDisplay("CT")]
        [Description("Trooper")]
        Trooper,
        [RankDisplay("CT")]
        [Description("Senior Trooper")]
        SeniorTrooper,
        [RankDisplay("CT")]
        [Description("Veteran Trooper")]
        VeteranTrooper,
        [RankDisplay("CP")]
        [Description("Corporal")]
        Corporal,
        [RankDisplay("CP")]
        [Description("Senior Corporal")]
        SeniorCorporal,
        [RankDisplay("CS")]
        [Description("Sergeant")]
        Sergeant,
        [RankDisplay("CS")]
        [Description("Senior Sergeant")]
        Seniorsergeant,
        [RankDisplay("CS-M")]
        [Description("Sergeant Major")]
        SergeantMajor,
        [RankDisplay("CS-M")]
        [Description("Company Sergeant Major")]
        CompanySergeantMajor,
        [RankDisplay("CS-M")]
        [Description("Battalion Command Sergeant Major")]
        BattalionSergeantMajor,
        [RankDisplay("CC")]
        [Description("Second Lieutenant")]
        SecondLieutenant,
        [RankDisplay("CC")]
        [Description("First Lieutenant")]
        FirstLieutenant,
        [RankDisplay("CC")]
        [Description("Captian")]
        Captian,
        [RankDisplay("BC")]
        [Description("Major")]
        Major
    }


    public enum MedicRank
    {
        [RankDisplay("CM-C")]
        [Description("Medical Cadet")]
        Cadet,
        [RankDisplay("CM")]
        [Description("Medic")]
        Medic,
        [RankDisplay("CM-T")]
        [Description("Medical Technician")]
        Technician,
        [RankDisplay("CM-P")]
        [Description("Medical Corporal")]
        Corporal,
        [RankDisplay("CM-S")]
        [Description("Medical Sergeant")]
        Sergeant,
        [RankDisplay("CM-O")]
        [Description("Batallion Medical Sergeant Major")]
        BattalionSergeantMajor
    }

    public enum RTORank
    {
        [RankDisplay("CI-C")]
        [Description("Intercommunicator Cadet")]
        Cadet,
        [RankDisplay("CI")]
        [Description("Intercommunicator")]
        Intercommunicator,
        [RankDisplay("CI-T")]
        [Description("Intercommunicator Technician")]
        Technician,
        [RankDisplay("CI-P")]
        [Description("Intercommunicator Corporal")]
        Corporal,
        [RankDisplay("CI-S")]
        [Description("Intercommunicator Sergeant")]
        Sergeant,
        [RankDisplay("CI-O")]
        [Description("Batallion Radio Sergeant Major")]
        BattalionSergeantMajor
    }

    public enum WarrantRank
    {
        [RankDisplay("CW")]
        [Description("Chief Warrant Officer")]
        Chief,
        [RankDisplay("CW-T")]
        [Description("Chief Warrant Officer One")]
        One,
        [RankDisplay("CW-P")]
        [Description("Chief Warrant Officer Two")]
        Two,
        [RankDisplay("CW-S")]
        [Description("Chief Warrant Officer Three")]
        Three,
        [RankDisplay("CW-M")]
        [Description("Chief Warrant Officer Four")]
        Four,
        [RankDisplay("CW-O")]
        [Description("Chief Warrant Officer Five")]
        Five
    }

    public enum PilotRank
    {
        [RankDisplay("CX-C")]
        [Description("Cadet")]
        Cadet,
        [RankDisplay("CX-C")]
        [Description("Senior Cadet")]
        SeniorCadet,
        [RankDisplay("CX-X")]
        [Description("Ensign")]
        Ensign,
        [RankDisplay("CX-X")]
        [Description("Senior Ensign")]
        SeniorEnsign,
        [RankDisplay("CX-M")]
        [Description("Master Aviator")]
        Master,
        [RankDisplay("CX-P")]
        [Description("Flight Officer")]
        FlightOfficer,
        [RankDisplay("CX-S")]
        [Description("Junior Lieutenant")]
        JuniorLieutenant,
        [RankDisplay("CX")]
        [Description("Second Lieutenant")]
        SecondLieutenant,
        [RankDisplay("CX")]
        [Description("First Lieutenant")]
        FirstLieutenant,
        [RankDisplay("CX")]
        [Description("Captian")]
        Captian,
        [RankDisplay("CX-W")]
        [Description("Warden")]
        Warden
    }

    public enum WardenRank
    {
        [RankDisplay("CX-W")]
        [Description("Petty Officer")]
        Warden,
        [RankDisplay("CX-W")]
        [Description("Senior Petty Officer")]
        Senior,
        [RankDisplay("CX-W")]
        [Description("Veteran Petty Officer")]
        Veteran,
        [RankDisplay("CX-W")]
        [Description("Chief Petty Officer")]
        Chief,
        [RankDisplay("CX-W")]
        [Description("Master Chief Petty Officer")]
        Master,
    }

    [AttributeUsage(AttributeTargets.All)]
    public class RankDisplayAttribute : Attribute
    {
        public string Shorthand { get; set; } = "";

        public RankDisplayAttribute(string shorthand)
        {
            Shorthand = shorthand;
        }
    }

    public static class RankExtensions
    {
        public static string AsShorthand<T>(this T value) where T : System.Enum
        {
            var type = typeof(T);
            var name = Enum.GetName(type, value);

            if (name is null) return "";

            return type?.GetField(name)
                ?.GetCustomAttributes<RankDisplayAttribute>()
                .SingleOrDefault()
                ?.Shorthand ?? "";
        }

        public static string AsFull<T>(this T value) where T : System.Enum
        {
            var type = typeof(T);
            var name = Enum.GetName(type, value);

            if (name is null) return "";

            return type?.GetField(name)
                ?.GetCustomAttribute<DescriptionAttribute>()
                ?.Description ?? "";
        }

        public static T? ValueFromString<T>(this string value) where T : Enum
        {
            var type = typeof(T);
            foreach (var v in Enum.GetValues(type))
            {
                if (type is null) continue;

                var name = Enum.GetName(type, v);

                if(name is not null)
                {
                    if(name == value)
                    {
                        return (T)v;
                    }   
                }
            }

            return default;
        }
    }
}
