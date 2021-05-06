using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Data
{
    public enum Slot : int
    {
        // Command
        [SlotDetails("Hailstorm HQ")]
        Hailstorm = 0,
        // Avalanche
        [SlotDetails("Avalanche HQ")]
        AvalancheCompany = 100,
        [SlotDetails("Avalanche 1 HQ")]
        AvalancheOne = 110,
        [SlotDetails("Avalanche 1-1")]
        AvalancheOneOne,
        [SlotDetails("Avalanche 1-2")]
        AvalancheOneTwo,
        [SlotDetails("Avalanche 1-3")]
        AvalancheOneThree,
        [SlotDetails("Avalanche 2 HQ")]
        AvalancheTwo = 120,
        [SlotDetails("Avalanche 2-1")]
        AvalancheTwoOne,
        [SlotDetails("Avalanche 2-2")]
        AvalancheTwoTwo,
        [SlotDetails("Avalanche 2-3")]
        AvalancheTwoThree,
        [SlotDetails("Avalanche 3 HQ")]
        AvalancheThree = 130,
        [SlotDetails("Avalanche 3-1")]
        AvalancheThreeOne,
        [SlotDetails("Avalanche 3-2")]
        AvalancheThreeTwo,
        [SlotDetails("Avalanche 3-3")]
        AvalancheThreeThree,
        // Cyclone
        [SlotDetails("Cyclone HQ")]
        CycloneCompany = 200,
        [SlotDetails("Cyclone 1 HQ")]
        CycloneOne = 210,
        [SlotDetails("Cyclone 1-1")]
        CycloneOneOne,
        [SlotDetails("Cyclone 1-2")]
        CycloneOneTwo,
        [SlotDetails("Cyclone 1-3")]
        CycloneOneThree,
        [SlotDetails("Cyclone 2 HQ")]
        CycloneTwo = 220,
        [SlotDetails("Cyclone 2-1")]
        CycloneTwoOne,
        [SlotDetails("Cyclone 2-2")]
        CycloneTwoTwo,
        [SlotDetails("Cyclone 2-3")]
        CycloneTwoThree,
        [SlotDetails("Cyclone 3 HQ")]
        CycloneThree = 230,
        [SlotDetails("Cyclone 3-1")]
        CycloneThreeOne,
        [SlotDetails("Cyclone 3-2")]
        CycloneThreeTwo,
        [SlotDetails("Cyclone 3-3")]
        CycloneThreeThree,
        // Airborne.
        [SlotDetails("Acklay HQ")]
        AcklayCompany = 300,
        [SlotDetails("Acklay 1 HQ")]
        AcklayOne = 310,
        [SlotDetails("Acklay 1-1")]
        AcklayOneOne,
        [SlotDetails("Acklay 1-2")]
        AcklayOneTwo,
        [SlotDetails("Acklay 1-3")]
        AcklayOneThree,
        [SlotDetails("Acklay 2 HQ")]
        AcklayTwo = 320,
        [SlotDetails("Acklay 2-1")]
        AcklayTwoOne,
        [SlotDetails("Acklay 2-2")]
        AcklayTwoTwo,
        [SlotDetails("Acklay 2-3")]
        AcklayTwoThree,
        // Mynock
        [SlotDetails("Bastion Detachment")]
        Mynock = 400,
        [SlotDetails("Bastion Detachment")]
        MynockOneOne,
        [SlotDetails("Bastion Detachment")]
        MynockOneTwo,
        [SlotDetails("Bastion Detachment")]
        MynockOneThree,
        // Aviators
        [SlotDetails("Razor Squadron")]
        Razor = 500,
        [SlotDetails("Razor Squadron")]
        RazorOne = 510,
        [SlotDetails("Razor Squadron")]
        RazorOneOne,
        [SlotDetails("Razor Squadron")]
        RazorOneTwo,
        [SlotDetails("Razor Squadron")]
        RazorTwo = 520,
        [SlotDetails("Razor Squadron")]
        RazorTwoOne,
        [SlotDetails("Razor Squadron")]
        RazorTwoTwo,
        [SlotDetails("Razor Squadron")]
        RazorThree = 530,
        [SlotDetails("Razor Squadron")]
        RazorThreeOne,
        [SlotDetails("Razor Squadron")]
        RazorThreeTwo,
        [SlotDetails("Razor Squadron")]
        RazorFour = 540,
        [SlotDetails("Razor Squadron")]
        RazorFourOne,
        [SlotDetails("Razor Squadron")]
        RazorFourTwo,
        [SlotDetails("Razor Squadron")]
        RazorFive = 550,
        [SlotDetails("Razor Squadron")]
        RazorFiveOne,
        [SlotDetails("Razor Squadron")]
        RazorFiveTwo,
        [SlotDetails("Razor Squadron")]
        Warden = 560,
        // Reserve.
        [SlotDetails("Zeta HQ")]
        ZetaCompany = 600,
        [SlotDetails("Zeta 1 HQ")]
        ZetaOne = 610,
        [SlotDetails("Zeta 1-1")]
        ZetaOneOne,
        [SlotDetails("Zeta 1-2")]
        ZetaOneTwo,
        [SlotDetails("Zeta 1-3")]
        ZetaOneThree,
        [SlotDetails("Zeta 2 HQ")]
        ZetaTwo = 620,
        [SlotDetails("Zeta 2-1")]
        ZetaTwoOne,
        [SlotDetails("Zeta 2-2")]
        ZetaTwoTwo,
        [SlotDetails("Zeta 2-3")]
        ZetaTwoThree,
        [SlotDetails("Zeta 3 HQ")]
        ZetaThree = 630,
        [SlotDetails("Zeta 3-1")]
        ZetaThreeOne,
        [SlotDetails("Zeta 3-2")]
        ZetaThreeTwo,
        [SlotDetails("Zeta 3-3")]
        ZetaThreeThree,
        [SlotDetails("Zeta 3-4")]
        ZetaThreeFour,
        // Inactive
        [SlotDetails("Inactive Reserves")]
        InactiveReserve = 700
    }

    [AttributeUsage(AttributeTargets.All)]
    public class SlotDetailsAttribute : Attribute
    {
        public string Name { get; set; } = "";

        public SlotDetailsAttribute(string name)
        {
            Name = name;
        }
    }

    public static class SlotExtensions
    {
        public static string AsName(this Slot value)
        {
            var type = typeof(Slot);
            var name = Enum.GetName(type, value);

            if (name is null) return "";

            return type?.GetField(name)
                ?.GetCustomAttributes<SlotDetailsAttribute>()
                .SingleOrDefault()
                ?.Name ?? "";
        }
    }
}
