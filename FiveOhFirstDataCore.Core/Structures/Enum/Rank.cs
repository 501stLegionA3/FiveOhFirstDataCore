using System.ComponentModel;
using System.Reflection;

namespace FiveOhFirstDataCore.Data.Structures
{
    public enum TrooperRank : int
    {
        [RankDisplay("CR")]
        [Description("Recruit")]
        Recruit = 0,
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
        SeniorSergeant,
        [RankDisplay("CS-M")]
        [Description("Sergeant-Major")]
        SergeantMajor,
        [RankDisplay("CS-M")]
        [Description("Company Sergeant-Major")]
        CompanySergeantMajor,
        [RankDisplay("CS-M")]
        [Description("Battalion Command Sergeant-Major")]
        BattalionSergeantMajor,
        [RankDisplay("CC")]
        [Description("Second Lieutenant")]
        SecondLieutenant,
        [RankDisplay("CC")]
        [Description("First Lieutenant")]
        FirstLieutenant,
        [RankDisplay("CC")]
        [Description("Captain")]
        Captain,
        [RankDisplay("BC")]
        [Description("Major")]
        Major
    }


    public enum MedicRank : int
    {
        [RankDisplay("CM-C")]
        [Description("Medical Cadet")]
        Cadet = 100,
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
        [Description("Batallion Medical Sergeant-Major")]
        BattalionSergeantMajor
    }

    public enum RTORank : int
    {
        [RankDisplay("CI-C")]
        [Description("Intercommunicator Cadet")]
        Cadet = 200,
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

    public enum WarrantRank : int
    {
        [RankDisplay("CW")]
        [Description("Chief Warrant Officer")]
        Chief = 300,
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

    public enum PilotRank : int
    {
        [RankDisplay("CX-C")]
        [Description("Cadet")]
        Cadet = 400,
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
        [Description("Lieutenant Junior Grade")]
        JuniorLieutenant,
        [RankDisplay("CX")]
        [Description("Second Lieutenant")]
        SecondLieutenant,
        [RankDisplay("CX")]
        [Description("First Lieutenant")]
        FirstLieutenant,
        [RankDisplay("CX")]
        [Description("Captain")]
        Captain,
    }

    public enum WardenRank : int
    {
        [RankDisplay("CX-W")]
        [Description("Petty Officer")]
        Warden = 500,
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
        [RankDisplay("CX-W")]
        [Description("Command Master Chief Petty Officer")]
        Command
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
        public static Enum? GetRank(this int value)
        {
            if (value == -1) return null;

            foreach (var val in Enum.GetValues(typeof(TrooperRank)))
            {
                if ((int)val == value)
                    return (Enum)val;
            }

            foreach (var val in Enum.GetValues(typeof(MedicRank)))
            {
                if ((int)val == value)
                    return (Enum)val;
            }

            foreach (var val in Enum.GetValues(typeof(RTORank)))
            {
                if ((int)val == value)
                    return (Enum)val;
            }

            foreach (var val in Enum.GetValues(typeof(WardenRank)))
            {
                if ((int)val == value)
                    return (Enum)val;
            }

            foreach (var val in Enum.GetValues(typeof(WarrantRank)))
            {
                if ((int)val == value)
                    return (Enum)val;
            }

            foreach (var val in Enum.GetValues(typeof(PilotRank)))
            {
                if ((int)val == value)
                    return (Enum)val;
            }

            return null;
        }

        public static Enum? GetRank(this string value)
        {
            Type[] types = new Type[] { typeof(TrooperRank), typeof(MedicRank), typeof(RTORank), typeof(PilotRank),
                typeof(WardenRank), typeof(WarrantRank)};

            foreach (var type in types)
            {
                foreach (dynamic enumValue in Enum.GetValues(type))
                {
                    var compare = value.Replace("Sr.", "Senior");
                    compare = compare.Replace("Vt.", "Veteran");

                    if (AsShorthand(enumValue) == compare
                        || AsFull(enumValue) == compare)
                        return enumValue;
                }
            }

            return null;
        }

        public static string AsShorthand<T>(this T value) where T : Enum
        {
            var type = typeof(T);
            var name = Enum.GetName(type, value);

            if (name is null) return "";

            return type?.GetField(name)
                ?.GetCustomAttributes<RankDisplayAttribute>()
                .SingleOrDefault()
                ?.Shorthand ?? "";
        }

        public static string AsFull<T>(this T value) where T : Enum
        {
            if (value is null) return "";

            var type = value.GetType();
            var name = Enum.GetName(type, value);

            if (name is null) return "";

            return type?.GetField(name)
                ?.GetCustomAttribute<DescriptionAttribute>()
                ?.Description ?? "";
        }

        public static string AsQualified<T>(this T value) where T : Enum
            => $"{value.GetType().Name}.{value}";

        public static T? ValueFromString<T>(this string value) where T : Enum
        {
            var type = typeof(T);
            foreach (var v in Enum.GetValues(type))
            {
                if (type is null) continue;

                var name = Enum.GetName(type, v);

                if (name is not null)
                {
                    if (name == value)
                    {
                        return (T)v;
                    }
                }
            }

            return default;
        }
    }
}
