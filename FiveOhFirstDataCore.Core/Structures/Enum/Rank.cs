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
        Cadet = 1,
        [RankDisplay("CT")]
        [Description("Trooper")]
        Trooper = 2,
        [RankDisplay("CT")]
        [Description("Senior Trooper")]
        SeniorTrooper = 3,
        [RankDisplay("CT")]
        [Description("Veteran Trooper")]
        VeteranTrooper = 4,
        [RankDisplay("CP")]
        [Description("Corporal")]
        Corporal = 5,
        [RankDisplay("CP")]
        [Description("Senior Corporal")]
        SeniorCorporal = 6,
        [RankDisplay("CS")]
        [Description("Sergeant")]
        Sergeant = 7,
        [RankDisplay("CS")]
        [Description("Senior Sergeant")]
        SeniorSergeant = 8,
        [RankDisplay("CS-M")]
        [Description("Sergeant-Major")]
        SergeantMajor = 9,
        [RankDisplay("CS-M")]
        [Description("Company Sergeant-Major")]
        CompanySergeantMajor = 10,
        [RankDisplay("CS-M")]
        [Description("Battalion Command Sergeant-Major")]
        BattalionSergeantMajor = 11,
        [RankDisplay("CC")]
        [Description("Second Lieutenant")]
        SecondLieutenant = 12,
        [RankDisplay("CC")]
        [Description("First Lieutenant")]
        FirstLieutenant = 13,
        [RankDisplay("CC")]
        [Description("Captain")]
        Captain = 14,
        [RankDisplay("BC")]
        [Description("Major")]
        Major = 15
    }


    public enum MedicRank : int
    {
        [RankDisplay("CM-C")]
        [Description("Medical Cadet")]
        Cadet = 100,
        [RankDisplay("CM")]
        [Description("Medic")]
        Medic = 101,
        [RankDisplay("CM-V")]
        [Description("Medical Veteran")]
        Veteran = 107,
        [RankDisplay("CM-T")]
        [Description("Medical Technician")]
        Technician = 102,
        [RankDisplay("CM-L")]
        [Description("Medical Lance Corporal")]
        LanceCorporal = 106,
        [RankDisplay("CM-P")]
        [Description("Medical Corporal")]
        Corporal = 103,
        [RankDisplay("CM-S")]
        [Description("Medical Sergeant")]
        Sergeant = 104,
        [RankDisplay("CM-O")]
        [Description("Batallion Medical Sergeant-Major")]
        BattalionSergeantMajor = 105
    }

    public enum RTORank : int
    {
        [RankDisplay("CI-C")]
        [Description("Intercommunicator Cadet")]
        Cadet = 200,
        [RankDisplay("CI")]
        [Description("Intercommunicator")]
        Intercommunicator = 201,
        [RankDisplay("CI-E")]
        [Description("Intercommunicator Expert")]
        Expert = 207,
        [RankDisplay("CI-T")]
        [Description("Intercommunicator Technician")]
        Technician = 202,
        [RankDisplay("CI-L")]
        [Description("Intercommunicator Lance Corporal")]
        LanceCorporal = 206,
        [RankDisplay("CI-P")]
        [Description("Intercommunicator Corporal")]
        Corporal = 203,
        [RankDisplay("CI-S")]
        [Description("Intercommunicator Sergeant")]
        Sergeant = 204,
        [RankDisplay("CI-O")]
        [Description("Batallion Radio Sergeant Major")]
        BattalionSergeantMajor = 205
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
        [Description("Senior Chief Petty Officer")]
        SeniorChief,
        [RankDisplay("CX-W")]
        [Description("Master Chief Petty Officer")]
        Master,
        [RankDisplay("CX-W")]
        [Description("Command Master Chief Petty Officer")]
        Command
    }
    
    public enum NCORank : int
    {
        [RankDisplay("CLC")]
        [Description("Lance Corporal")]
        Lance
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

            foreach (var val in Enum.GetValues(typeof(NCORank)))
            {
                if ((int)val == value)
                    return (Enum)val;
            }

            return null;
        }

        public static Enum? GetRank(this string value)
        {
            Type[] types = new Type[] { typeof(TrooperRank), typeof(MedicRank), typeof(RTORank), typeof(PilotRank),
                typeof(WardenRank), typeof(WarrantRank), typeof(NCORank)};

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
