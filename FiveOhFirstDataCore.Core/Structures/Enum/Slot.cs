﻿using System.ComponentModel;
using System.Text.Json.Serialization;

namespace FiveOhFirstDataCore.Data.Structures
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
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
        [Description("Mynock HQ")]
        Mynock = 400,
        [Description("Mynock 1-1")]
        MynockOneOne,
        [Description("Mynock 1-2")]
        MynockOneTwo,
        [Description("Mynock 1-3")]
        MynockOneThree,
        // Aviators
        [Description("Razor HQ")]
        Razor = 500,
        [Description("Broadside HQ")]
        RazorB = 510,
        [Description("Broadside 1")]
        RazorBOne,
        [Description("Broadside 2")]
        RazorBTwo,
        [Description("Broadside 3")]
        RazorBThree,
        [Description("Broadside 4")]
        RazorBFour,
        [Description("Zillo HQ")]
        RazorZ = 520,
        [Description("Zillo 1")]
        RazorZOne,
        [Description("Zillo 2")]
        RazorZTwo,
        [Description("Zillo 3")]
        RazorZThree,
        [Description("Zillo 4")]
        RazorZFour,
        // Warden
        [Description("Warden HQ")]
        Warden = 530,
        [Description("Warden 1")]
        WardenOne,
        [Description("Warden 2")]
        WardenTwo,
        [Description("Warden 3")]
        WardenThree,
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
        [Description("Zeta 1-4")]
        ZetaOneFour,
        [Description("Zeta 2 HQ")]
        ZetaTwo = 620,
        [Description("Zeta 2-1")]
        ZetaTwoOne,
        [Description("Zeta 2-2")]
        ZetaTwoTwo,
        [Description("Zeta 2-3")]
        ZetaTwoThree,
        [Description("Zeta 2-4")]
        ZetaTwoFour,
        // Inactive
        [Description("Inactive Reserves")]
        InactiveReserve = 700,
        [Description("Mynock Reserves")]
        MynockReserve,
        [Description("Acklay Reserves")]
        AcklayReserve,
        [Description("Razor Reserves")]
        RazorReserve,
        // Archives
        [Description("Archived")]
        Archived = 1000
    }

    public static class SlotExtensions
    {
        public static Slot? GetSlot(this string value)
        {
            foreach (Slot enumValue in Enum.GetValues(typeof(Slot)))
            {
                if (enumValue.AsFull() == value)
                    return enumValue;
            }

            return null;
        }

        public static Slot GetRazorSlot(this int squad, int wing)
        {
            var slot = 500 + (10 * squad) + wing;
            return (Slot)slot;
        }

        #region Infanry

        public static bool IsSquad(this Slot slot)
        {
            if (slot >= Slot.AvalancheCompany && slot < Slot.Mynock
                || slot >= Slot.ZetaCompany && slot < Slot.InactiveReserve)
            {
                var val = (int)slot % 10;
                if (val > 0) return true;
            }

            return false;
        }

        public static bool IsPlatoon(this Slot slot)
        {
            if (slot >= Slot.AvalancheCompany && slot < Slot.Mynock
                || slot >= Slot.ZetaCompany && slot < Slot.InactiveReserve)
            {
                var s = (int)slot;
                var val = s % 10;
                if (val == 0 && s % 100 != 0) return true;
            }

            return false;
        }

        public static bool IsCompany(this Slot slot)
        {
            if (slot >= Slot.AvalancheCompany && slot < Slot.Mynock
                || slot >= Slot.ZetaCompany && slot < Slot.InactiveReserve)
            {
                var val = (int)slot / 10 % 10;
                if (val == 0) return true;
            }

            return false;
        }

        #endregion

        #region Razor/Warden
        public static bool IsRazor(this Slot slot)
        {
            return (slot >= Slot.Razor && slot < Slot.ZetaCompany);
        }

        public static bool IsWardenTeam(this Slot slot)
        {
            if (slot >= Slot.Warden && slot < Slot.ZetaCompany)
            {
                var val = (int)slot % 10;
                if (val > 0) return true;
            }
            return false;
        }

        public static bool IsFlight(this Slot slot)
        {
            if (slot >= Slot.Razor && slot < Slot.Warden)
            {
                var val = (int)slot % 10;
                if (val != 0) return true;
            }
            return false;
        }

        public static bool IsSquadron(this Slot slot)
        {
            if (slot >= Slot.Razor && slot < Slot.Warden)
            {
                var val = (int)slot % 10;
                if (val == 0 && (int)slot % 100 != 0) return true;
            }
            return false;
        }

        public static bool IsWing(this Slot slot)
        {
            if (slot >= Slot.Razor && slot < Slot.Warden)
            {
                var val = (int)slot / 10 % 10;
                if (val == 0) return true;
            }
            return false;
        }

        #endregion

        #region Mynock
        public static bool IsMynock(this Slot slot)
        {
            return (slot >= Slot.Mynock && slot < Slot.Razor);
        }
        public static bool IsMynockSection(this Slot slot)
        {
            if (slot >= Slot.Mynock && slot < Slot.Razor)
            {
                var val = (int)slot % 10;
                if (val > 0) return true;
            }
            return false;
        }
        #endregion

        public static bool IsBattalion(this Slot slot)
        {
            return slot == Slot.Hailstorm;
        }

        public static Slot GetPlatoon(this Slot slot)
        {
            int lsd = (int)slot % 10;
            int newSlot = (int)slot - lsd;
            return (Slot)newSlot;
        }

        public static Slot GetCompany(this Slot slot)
        {
            int lsd = (int)slot % 100;
            int newSlot = (int)slot - lsd;
            return (Slot)newSlot;
        }
    }
}
