using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace FiveOhFirstDataCore.Core.Data
{
    public enum Slot : int
    {
        // Command
        [Description("Hailstorm HQ")]
        Hailstorm = 0,
        // Avalanche
        [Description("Avalanche HQ")]
        AvalancheCompany = 100,
        [Description("Avalanche 1 HQ")]
        AvalancheOne = 110,
        [Description("Avalanche 1-1")]
        AvalancheOneOne,
        [Description("Avalanche 1-2")]
        AvalancheOneTwo,
        [Description("Avalanche 1-3")]
        AvalancheOneThree,
        [Description("Avalanche 2 HQ")]
        AvalancheTwo = 120,
        [Description("Avalanche 2-1")]
        AvalancheTwoOne,
        [Description("Avalanche 2-2")]
        AvalancheTwoTwo,
        [Description("Avalanche 2-3")]
        AvalancheTwoThree,
        [Description("Avalanche 3 HQ")]
        AvalancheThree = 130,
        [Description("Avalanche 3-1")]
        AvalancheThreeOne,
        [Description("Avalanche 3-2")]
        AvalancheThreeTwo,
        [Description("Avalanche 3-3")]
        AvalancheThreeThree,
        // Cyclone
        [Description("Cyclone HQ")]
        CycloneCompany = 200,
        [Description("Cyclone 1 HQ")]
        CycloneOne = 210,
        [Description("Cyclone 1-1")]
        CycloneOneOne,
        [Description("Cyclone 1-2")]
        CycloneOneTwo,
        [Description("Cyclone 1-3")]
        CycloneOneThree,
        [Description("Cyclone 2 HQ")]
        CycloneTwo = 220,
        [Description("Cyclone 2-1")]
        CycloneTwoOne,
        [Description("Cyclone 2-2")]
        CycloneTwoTwo,
        [Description("Cyclone 2-3")]
        CycloneTwoThree,
        [Description("Cyclone 3 HQ")]
        CycloneThree = 230,
        [Description("Cyclone 3-1")]
        CycloneThreeOne,
        [Description("Cyclone 3-2")]
        CycloneThreeTwo,
        [Description("Cyclone 3-3")]
        CycloneThreeThree,
        // Airborne.
        [Description("Acklay HQ")]
        AcklayCompany = 300,
        [Description("Acklay 1 HQ")]
        AcklayOne = 310,
        [Description("Acklay 1-1")]
        AcklayOneOne,
        [Description("Acklay 1-2")]
        AcklayOneTwo,
        [Description("Acklay 1-3")]
        AcklayOneThree,
        [Description("Acklay 2 HQ")]
        AcklayTwo = 320,
        [Description("Acklay 2-1")]
        AcklayTwoOne,
        [Description("Acklay 2-2")]
        AcklayTwoTwo,
        [Description("Acklay 2-3")]
        AcklayTwoThree,
        // Mynock
        [Description("Bastion Detachment")]
        Mynock = 400,
        [Description("Bastion Detachment")]
        MynockOneOne,
        [Description("Bastion Detachment")]
        MynockOneTwo,
        [Description("Bastion Detachment")]
        MynockOneThree,
        // Aviators
        [Description("Razor Squadron")]
        Razor = 500,
        [Description("Razor Squadron")]
        RazorOne = 510,
        [Description("Razor Squadron")]
        RazorOneOne,
        [Description("Razor Squadron")]
        RazorOneTwo,
        [Description("Razor Squadron")]
        RazorTwo = 520,
        [Description("Razor Squadron")]
        RazorTwoOne,
        [Description("Razor Squadron")]
        RazorTwoTwo,
        [Description("Razor Squadron")]
        RazorThree = 530,
        [Description("Razor Squadron")]
        RazorThreeOne,
        [Description("Razor Squadron")]
        RazorThreeTwo,
        [Description("Razor Squadron")]
        RazorFour = 540,
        [Description("Razor Squadron")]
        RazorFourOne,
        [Description("Razor Squadron")]
        RazorFourTwo,
        [Description("Razor Squadron")]
        RazorFive = 550,
        [Description("Razor Squadron")]
        RazorFiveOne,
        [Description("Razor Squadron")]
        RazorFiveTwo,
        [Description("Razor Squadron")]
        Warden = 560,
        // Reserve.
        [Description("Zeta HQ")]
        ZetaCompany = 600,
        [Description("Zeta 1 HQ")]
        ZetaOne = 610,
        [Description("Zeta 1-1")]
        ZetaOneOne,
        [Description("Zeta 1-2")]
        ZetaOneTwo,
        [Description("Zeta 1-3")]
        ZetaOneThree,
        [Description("Zeta 2 HQ")]
        ZetaTwo = 620,
        [Description("Zeta 2-1")]
        ZetaTwoOne,
        [Description("Zeta 2-2")]
        ZetaTwoTwo,
        [Description("Zeta 2-3")]
        ZetaTwoThree,
        [Description("Zeta 3 HQ")]
        ZetaThree = 630,
        [Description("Zeta 3-1")]
        ZetaThreeOne,
        [Description("Zeta 3-2")]
        ZetaThreeTwo,
        [Description("Zeta 3-3")]
        ZetaThreeThree,
        [Description("Zeta 3-4")]
        ZetaThreeFour,
        // Inactive
        [Description("Inactive Reserves")]
        InactiveReserve = 700,
        // Archives
        [Description("Archived")]
        Archived = 1000
    }

    public static class SlotExtensions
    {
        public static string AsName(this Slot value)
        {
            var type = typeof(Slot);
            var name = Enum.GetName(type, value);

            if (name is null) return "";

            return type?.GetField(name)
                ?.GetCustomAttributes<DescriptionAttribute>()
                .SingleOrDefault()
                ?.Description ?? "";
        }
    }
}
