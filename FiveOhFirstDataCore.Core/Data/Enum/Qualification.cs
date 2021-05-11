using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Data
{
    [Flags]
    public enum Qualification : long
    {
        None = 0x0000000000,
        [Description("Zeus")]
        Zeus = 0x0000000001, // 1 << 0
        [Description("RTO")]
        RTO = 0x0000000002, // 1 << 1
        [Description("Assault")]
        Assault = 0x0000000004, // 1 << 2
        [Description("Marksman")]
        Marksman = 0x0000000008, // 1 << 3
        [Description("Grenadier")]
        Grenadier = 0x0000000010, // 1 << 4
        [Description("Support")]
        Support = 0x0000000020, // 1 << 5
        [Description("Medic")]
        Medic = 0x0000000040, // 1 << 6
        [Description("Jumpmaster")]
        Jumpmaster = 0x0000000080, // 1 << 7
        [Description("Combat Engineer")]
        CombatEngineer = 0x0000000100 // 1 << 8
    }

    public static class QualificationsExtensions
    {
        public static Qualification GetAllowedQualifications(this ClaimsPrincipal claim)
        {
            Qualification output = Qualification.None;
            foreach (var qual in Enum.GetValues<Qualification>())
            {
                switch (qual)
                {
                    case Qualification.Zeus:
                        if (claim.HasClaim("Instructor", "Zeus") || claim.IsInRole("Admin"))
                            output |= Qualification.Zeus;
                        break;
                    case Qualification.RTO:
                        if (claim.HasClaim("Instructor", "RTO") || claim.IsInRole("Admin"))
                            output |= Qualification.RTO;
                        break;
                    case Qualification.Assault:
                        if (claim.HasClaim("Instructor", "Assault") || claim.IsInRole("Admin"))
                            output |= Qualification.Assault;
                        break;
                    case Qualification.Marksman:
                        if (claim.HasClaim("Instructor", "Marksman") || claim.IsInRole("Admin"))
                            output |= Qualification.Marksman;
                        break;
                    case Qualification.Grenadier:
                        if (claim.HasClaim("Instructor", "Grenadier") || claim.IsInRole("Admin"))
                            output |= Qualification.Grenadier;
                        break;
                    case Qualification.Support:
                        if (claim.HasClaim("Instructor", "Support") || claim.IsInRole("Admin"))
                            output |= Qualification.Support;
                        break;
                    case Qualification.Medic:
                        if (claim.HasClaim("Instructor", "Medic") || claim.IsInRole("Admin"))
                            output |= Qualification.Medic;
                        break;
                    case Qualification.Jumpmaster:
                        if (claim.HasClaim("Instructor", "Jumpmaster") || claim.IsInRole("Admin"))
                            output |= Qualification.Jumpmaster;
                        break;
                    case Qualification.CombatEngineer:
                        if (claim.HasClaim("Instructor", "Combat Engineer") || claim.IsInRole("Admin"))
                            output |= Qualification.CombatEngineer;
                        break;
                }
            }

            return output;
        }
    }
}
