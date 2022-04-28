using System.ComponentModel;
using System.Security.Claims;

namespace FiveOhFirstDataCore.Data.Structures
{
    [Flags]
    public enum Qualification : long
    {
        None = 0x0000000000,
        [Description("Zeus Permit")]
        ZeusPermit = 0x0000000001, // 1 << 0
        [Description("Zeus Qualified")]
        ZeusQualified = 0x0000000002, // 1 << 1
        [Description("RTO Basic")]
        RTOBasic = 0x0000000004, // 1 << 2
        [Description("RTO Qualified")]
        RTOQualified = 0x0000000008, // 1 << 3
        [Description("Assault")]
        Assault = 0x0000000010, // 1 << 4
        [Description("RAMR")]
        RAMR = 0x0000000020, // 1 << 5
        [Description("Marksman")]
        Marksman = 0x0000000040, // 1 << 6
        [Description("Grenadier")]
        Grenadier = 0x0000000080, // 1 << 7
        [Description("Support")]
        Support = 0x0000000100, // 1 << 8
        [Description("Z1000")]
        Z1000 = 0x0000000200, // 1 << 9
        [Description("Medic")]
        Medic = 0x0000000400, // 1 << 10
        [Description("Jumpmaster")]
        Jumpmaster = 0x0000000800, // 1 << 11
        [Description("Combat Engineer")]
        CombatEngineer = 0x0000001000, // 1 << 12
        [Description("Advanced Combat Engineer")]
        AdvancedCombatEngineer = 0x0000002000, // 1 << 13
        [Description("Close Quarters Specalist")]
        CloseQuartersSpecialist = 0x0000004000 // 1 << 14
    }

    public static class QualificationsExtensions
    {
        public static Qualification GetAllowedQualifications(this ClaimsPrincipal claim)
        {
            Qualification output = Qualification.None;
            var staff = CShop.QualTrainingStaff.AsFull();
            foreach (var qual in Enum.GetValues<Qualification>())
            {
                switch (qual)
                {
                    case Qualification.ZeusPermit:
                        if (claim.HasClaim(x => x.Type == "Zeus") || claim.IsInRole("Admin")
                            || claim.IsInRole("Manager") || claim.HasClaim(x => x.Type == staff))
                            output |= Qualification.ZeusPermit | Qualification.ZeusQualified;
                        break;
                    case Qualification.RTOBasic:
                        if (claim.HasClaim(x => x.Type == "RTO") || claim.IsInRole("Admin")
                            || claim.IsInRole("Manager") || claim.HasClaim(x => x.Type == staff))
                            output |= Qualification.RTOBasic | Qualification.RTOQualified;
                        break;
                    case Qualification.Assault:
                        if (claim.HasClaim(x => x.Type == "Assault") || claim.IsInRole("Admin")
                            || claim.IsInRole("Manager") || claim.HasClaim(x => x.Type == staff))
                            output |= Qualification.Assault | Qualification.RAMR;
                        break;
                    case Qualification.Marksman:
                        if (claim.HasClaim(x => x.Type == "Marksman") || claim.IsInRole("Admin")
                            || claim.IsInRole("Manager") || claim.HasClaim(x => x.Type == staff))
                            output |= Qualification.Marksman;
                        break;
                    case Qualification.Grenadier:
                        if (claim.HasClaim(x => x.Type == "Grenadier") || claim.IsInRole("Admin")
                            || claim.IsInRole("Manager") || claim.HasClaim(x => x.Type == staff))
                            output |= Qualification.Grenadier;
                        break;
                    case Qualification.Support:
                        if (claim.HasClaim(x => x.Type == "Support") || claim.IsInRole("Admin")
                            || claim.IsInRole("Manager") || claim.HasClaim(x => x.Type == staff))
                            output |= Qualification.Support | Qualification.Z1000;
                        break;
                    case Qualification.Medic:
                        if (claim.HasClaim(x => x.Type == "Medic") || claim.IsInRole("Admin")
                            || claim.IsInRole("Manager") || claim.HasClaim(x => x.Type == staff))
                            output |= Qualification.Medic;
                        break;
                    case Qualification.Jumpmaster:
                        if (claim.HasClaim(x => x.Type == "Jumpmaster") || claim.IsInRole("Admin")
                            || claim.IsInRole("Manager") || claim.HasClaim(x => x.Type == staff))
                            output |= Qualification.Jumpmaster;
                        break;
                    case Qualification.CombatEngineer:
                        if (claim.HasClaim(x => x.Type == "Combat Engineer") || claim.IsInRole("Admin")
                            || claim.IsInRole("Manager") || claim.HasClaim(x => x.Type == staff))
                            output |= Qualification.CombatEngineer;
                        break;
                    case Qualification.AdvancedCombatEngineer:
                        if (claim.HasClaim(x => x.Type == "Advanced Combat Engineer") || claim.IsInRole("Admin")
                            || claim.IsInRole("Manager") || claim.HasClaim(x => x.Type == staff))
                            output |= Qualification.AdvancedCombatEngineer;
                        break;
                }
            }

            return output;
        }
    }
}
