using System;
using System.Linq;
using System.Reflection;

namespace FiveOhFirstDataCore.Core.Data
{
    public enum TrooperRank
    {
        [RankDisplay("CR", "Clone Recruit")]
        Recruit,
        [RankDisplay("CR-C", "Clone Cadet")]
        Cadet,
        [RankDisplay("CT", "Clone Trooper")]
        Trooper,
        [RankDisplay("CT", "Clone Senior Trooper")]
        SeniorTrooper,
        [RankDisplay("CT", "Clone Veteran Trooper")]
        VeteranTrooper,
        [RankDisplay("CP", "Clone Corporal")]
        Corporal,
        [RankDisplay("CP", "Clone Senior Corporal")]
        SeniorCorporal,
        [RankDisplay("CS", "Clone Sergeant")]
        Sergeant,
        [RankDisplay("CS", "Clone Senior Sergeant")]
        Seniorsergeant,
        [RankDisplay("CS-M", "Clone Sergeant Major")]
        SergeantMajor,
        [RankDisplay("CS-M", "Clone Company Sergeant Major")]
        CompanySergeantMajor,
        [RankDisplay("CS-M", "Clone Battalion Command Sergeant Major")]
        BattalionSergeantMajor,
        [RankDisplay("CC", "Second Lieutenant")]
        SecondLieutenant,
        [RankDisplay("CC", "First Lieutenant")]
        FirstLieutenant,
        [RankDisplay("CC", "Captian")]
        Captian,
        [RankDisplay("BC", "Major")]
        Major
    }


    public enum MedicRank
    {
        [RankDisplay("CM-C", "Clone Medical Cadet")]
        Cadet,
        [RankDisplay("CM", "Clone Medic")]
        Medic,
        [RankDisplay("CM-T", "Clone Medical Technician")]
        Technician,
        [RankDisplay("CM-P", "Clone Medical Corporal")]
        Corporal,
        [RankDisplay("CM-S", "Clone Medical Sergeant")]
        Sergeant,
        [RankDisplay("CM-O", "Clone Batallion Medical Sergeant Major")]
        BattalionSergeantMajor
    }

    public enum RTORank
    {
        [RankDisplay("CI-C", "Clone Intercommunicator Cadet")]
        Cadet,
        [RankDisplay("CI", "Clone Intercommunicator")]
        Intercommunicator,
        [RankDisplay("CI-T", "Clone Intercommunicator Technician")]
        Technician,
        [RankDisplay("CI-P", "Clone Intercommunicator Corporal")]
        Corporal,
        [RankDisplay("CI-S", "Clone Intercommunicator Sergeant")]
        Sergeant,
        [RankDisplay("CI-O", "Clone Batallion Radio Sergeant Major")]
        BattalionSergeantMajor
    }

    public enum WarrantRank
    {
        [RankDisplay("CW", "Chief Warrant Officer")]
        Chief,
        [RankDisplay("CW-T", "Chief Warrant Officer One")]
        One,
        [RankDisplay("CW-P", "Chief Warrant Officer Two")]
        Two,
        [RankDisplay("CW-S", "Chief Warrant Officer Three")]
        Three,
        [RankDisplay("CW-M", "Chief Warrant Officer Four")]
        Four,
        [RankDisplay("CW-O", "Chief Warrant Officer Five")]
        Five
    }

    public enum PilotRank
    {
        [RankDisplay("CX-C", "Cadet")]
        Cadet,
        [RankDisplay("CX-C", "Senior Cadet")]
        SeniorCadet,
        [RankDisplay("CX-X", "Ensign")]
        Ensign,
        [RankDisplay("CX-X", "Senior Ensign")]
        SeniorEnsign,
        [RankDisplay("CX-M", "Master Aviator")]
        Master,
        [RankDisplay("CX-P", "Flight Officer")]
        FlightOfficer,
        [RankDisplay("CX-S", "Junior Lieutenant")]
        JuniorLieutenant,
        [RankDisplay("CX", "Second Lieutenant")]
        SecondLieutenant,
        [RankDisplay("CX", "First Lieutenant")]
        FirstLieutenant,
        [RankDisplay("CX", "Captian")]
        Captian,
        [RankDisplay("CX-W", "Warden")]
        Warden
    }

    public enum WardenRank
    {
        [RankDisplay("CX-W", "Petty Officer")]
        Warden,
        [RankDisplay("CX-W", "Senior Petty Officer")]
        Senior,
        [RankDisplay("CX-W", "Veteran Petty Officer")]
        Veteran,
        [RankDisplay("CX-W", "Chief Petty Officer")]
        Chief,
        [RankDisplay("CX-W", "Master Chief Petty Officer")]
        Master,
    }

    [AttributeUsage(AttributeTargets.All)]
    public class RankDisplayAttribute : Attribute
    {
        public string Shorthand { get; set; } = "";
        public string Full { get; set; } = "";

        public RankDisplayAttribute(string shorthand, string full)
        {
            Shorthand = shorthand;
            Full = full;
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
                ?.GetCustomAttribute<RankDisplayAttribute>()
                ?.Full ?? "";
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
