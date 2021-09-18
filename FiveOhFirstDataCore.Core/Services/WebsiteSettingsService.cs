using FiveOhFirstDataCore.Data.Account;
using FiveOhFirstDataCore.Data.Structures;
using FiveOhFirstDataCore.Data.Structures.Promotions;
using FiveOhFirstDataCore.Data.Structuresbase;
using FiveOhFirstDataCore.Data.Structures;
using FiveOhFirstDataCore.Core.Structures.Auth;
using FiveOhFirstDataCore.Core.Structures.Policy;

using Microsoft.EntityFrameworkCore;

using System.Security.Claims;

namespace FiveOhFirstDataCore.Data.Services
{
    public class WebsiteSettingsService : IWebsiteSettingsService
    {
        #region Defaults
        private static readonly Dictionary<int, PromotionDetails> NeededTimeForPromotion = new()
        {
            // recruit and cadet ranks are not handled by promotion boards.
            [(int)TrooperRank.Cadet] = new()
            {
                CanPromoteTo = new() { (int)TrooperRank.Trooper }
            },
            [(int)TrooperRank.Trooper] = new()
            {
                RequiredRank = new() { (int)TrooperRank.Cadet },
                RequiredTimeInGrade = 30,
                NeededLevel = PromotionBoardLevel.Platoon,
                CanPromoteTo = new() { (int)TrooperRank.SeniorTrooper }
            },
            [(int)TrooperRank.SeniorTrooper] = new()
            {
                RequiredRank = new() { (int)TrooperRank.Trooper },
                RequiredTimeInGrade = 122,
                NeededLevel = PromotionBoardLevel.Company,
                CanPromoteTo = new() { (int)TrooperRank.VeteranTrooper, (int)TrooperRank.Corporal }
            },
            [(int)TrooperRank.VeteranTrooper] = new()
            {
                RequiredRank = new() { (int)TrooperRank.SeniorTrooper },
                RequiredTimeInGrade = 243,
                NeededLevel = PromotionBoardLevel.Battalion,
                CanPromoteTo = new() { (int)TrooperRank.Corporal }
            },
            [(int)TrooperRank.Corporal] = new()
            {
                RequiredRank = new() { (int)TrooperRank.SeniorTrooper, (int)TrooperRank.VeteranTrooper },
                RequiredTimeInGrade = 61,
                RequiredBillet = new() { Role.Lead },
                RequiredTimeInBillet = 61,
                RequiredQualifications = Qualification.RTOQualified | Qualification.ZeusPermit,
                NeededLevel = PromotionBoardLevel.Battalion,
                CanPromoteTo = new() { (int)TrooperRank.SeniorCorporal, (int)TrooperRank.Sergeant }
            },
            [(int)TrooperRank.SeniorCorporal] = new()
            {
                RequiredRank = new() { (int)TrooperRank.Corporal },
                RequiredTimeInGrade = 152,
                NeededLevel = PromotionBoardLevel.Battalion,
                CanPromoteTo = new() { (int)TrooperRank.Sergeant }
            },
            [(int)TrooperRank.Sergeant] = new()
            {
                RequiredRank = new() { (int)TrooperRank.Corporal },
                InherentRankAuth = new() { (int)TrooperRank.SeniorCorporal },
                RequiredTimeInBillet = 91,
                RequiredBillet = new() { Role.Lead },
                TeamMustBeNull = true,
                RequiredTimeInGrade = 61,
                NeededLevel = PromotionBoardLevel.Battalion,
                CanPromoteTo = new() { (int)TrooperRank.SeniorSergeant }
            },
            [(int)TrooperRank.SeniorSergeant] = new()
            {
                RequiredRank = new() { (int)TrooperRank.Sergeant },
                RequiredTimeInBillet = 182,
                NeededLevel = PromotionBoardLevel.Battalion
            },
            [(int)TrooperRank.SergeantMajor] = new()
            {
                RequiredBillet = new() { Role.SergeantMajor },
                RequiredTimeInBillet = 122,
                NeededLevel = PromotionBoardLevel.Battalion,
                DoesNotRequireLinearProgression = true
            },
            [(int)TrooperRank.CompanySergeantMajor] = new()
            {
                RequiredBillet = new() { Role.NCOIC, Role.Adjutant },
                RequiredTimeInBillet = 122,
                NeededLevel = PromotionBoardLevel.Battalion,
                DoesNotRequireLinearProgression = true
            },
            [(int)TrooperRank.BattalionSergeantMajor] = new()
            {
                RequiredBillet = new() { Role.NCOIC },
                SlotMin = Slot.Hailstorm,
                SlotMax = Slot.Hailstorm,
                RequiredTimeInBillet = 122,
                NeededLevel = PromotionBoardLevel.Battalion,
                DoesNotRequireLinearProgression = true
            },
            [(int)TrooperRank.SecondLieutenant] = new()
            {
                RequiredRank = new() { (int)TrooperRank.Sergeant },
                RankOrHigher = true,
                RequiredTimeInGrade = 182,
                TiGWaivedFor = new() { (int)WarrantRank.Five },
                RequiredBillet = new() { Role.Commander, Role.XO },
                RequiredTimeInBillet = 122,
                NeededLevel = PromotionBoardLevel.Battalion,
                DoesNotRequireLinearProgression = true,
                CanPromoteTo = new() { (int)TrooperRank.FirstLieutenant }
            },
            [(int)TrooperRank.FirstLieutenant] = new()
            {
                RequiredRank = new() { (int)TrooperRank.SecondLieutenant },
                RequiredTimeInGrade = 243,
                NeededLevel = PromotionBoardLevel.Battalion,
                CanPromoteTo = new() { (int)TrooperRank.Captain }
            },
            [(int)TrooperRank.Captain] = new()
            {
                RequiredRank = new() { (int)TrooperRank.FirstLieutenant },
                RequiredTimeInGrade = 243,
                RequiredBillet = new() { Role.Commander, Role.XO },
                DivideEqualsZero = 100,
                RequiredTimeInBillet = 122,
                NeededLevel = PromotionBoardLevel.Battalion,
                CanPromoteTo = new() { (int)TrooperRank.Major }
            },
            [(int)TrooperRank.Major] = new()
            {
                RequiredRank = new() { (int)TrooperRank.Captain },
                RequiredTimeInGrade = 730,
                NeededLevel = PromotionBoardLevel.Battalion
            },

            // Medical cadet not handled by promotion boards.
            [(int)MedicRank.Cadet] = new()
            {
                CanPromoteTo = new() { (int)MedicRank.Medic }
            },
            [(int)MedicRank.Medic] = new()
            {
                RequiredRank = new() { (int)MedicRank.Cadet },
                RequiredTimeInGrade = 61,
                NeededLevel = PromotionBoardLevel.Platoon,
                CanPromoteTo = new() { (int)MedicRank.Technician }
            },
            [(int)MedicRank.Technician] = new()
            {
                RequiredRank = new() { (int)MedicRank.Medic },
                RequiredTimeInGrade = 183,
                NeededLevel = PromotionBoardLevel.Battalion
            },
            [(int)MedicRank.Corporal] = new()
            {
                RequiredBillet = new() { Role.Medic },
                DivideEqualsZero = 10,
                RequiredTimeInBillet = 61,
                NeededLevel = PromotionBoardLevel.Battalion,
                DoesNotRequireLinearProgression = true,
                CanPromoteTo = new() { (int)MedicRank.Sergeant }
            },
            [(int)MedicRank.Sergeant] = new()
            {
                RequiredRank = new() { (int)MedicRank.Corporal },
                RequiredTimeInGrade = 122,
                RequiredBillet = new() { Role.Medic },
                DivideEqualsZero = 100,
                RequiredTimeInBillet = 183,
                NeededLevel = PromotionBoardLevel.Battalion
            },
            [(int)MedicRank.BattalionSergeantMajor] = new()
            {
                RequiredBillet = new() { Role.Medic },
                SlotMin = Slot.Hailstorm,
                SlotMax = Slot.Hailstorm,
                RequiredTimeInBillet = 122,
                NeededLevel = PromotionBoardLevel.Battalion,
                DoesNotRequireLinearProgression = true
            },

            // RTO cadet not handled by promotion boards.
            [(int)RTORank.Cadet] = new()
            {
                CanPromoteTo = new() { (int)RTORank.Intercommunicator }
            },
            [(int)RTORank.Intercommunicator] = new()
            {
                RequiredRank = new() { (int)RTORank.Cadet },
                NeededLevel = PromotionBoardLevel.Platoon,
                CanPromoteTo = new() { (int)RTORank.Technician }
            },
            [(int)RTORank.Technician] = new()
            {
                RequiredRank = new() { (int)RTORank.Intercommunicator },
                RequiredTimeInGrade = 183,
                NeededLevel = PromotionBoardLevel.Battalion
            },
            [(int)RTORank.Corporal] = new()
            {
                RequiredBillet = new() { Role.RTO },
                DivideEqualsZero = 10,
                RequiredTimeInBillet = 61,
                NeededLevel = PromotionBoardLevel.Battalion,
                DoesNotRequireLinearProgression = true,
                CanPromoteTo = new() { (int)RTORank.Sergeant }
            },
            [(int)RTORank.Sergeant] = new()
            {
                RequiredRank = new() { (int)RTORank.Corporal },
                RequiredTimeInGrade = 122,
                RequiredBillet = new() { Role.RTO },
                DivideEqualsZero = 100,
                RequiredTimeInBillet = 122,
                NeededLevel = PromotionBoardLevel.Battalion
            },
            [(int)RTORank.BattalionSergeantMajor] = new()
            {
                RequiredBillet = new() { Role.RTO },
                SlotMin = Slot.Hailstorm,
                SlotMax = Slot.Hailstorm,
                RequiredTimeInBillet = 122,
                NeededLevel = PromotionBoardLevel.Battalion,
                DoesNotRequireLinearProgression = true
            },

            // CW rank not handled here (or maybe) - leaving it in for now.

            [(int)WarrantRank.Chief] = new()
            {
                RequiresCShop = true,
                RequiredRank = new() { (int)TrooperRank.Trooper },
                RankOrHigher = true,
                RequiredTimeInGrade = 61,
                NeededLevel = PromotionBoardLevel.Battalion,
                DoesNotRequireLinearProgression = true,
                CanPromoteTo = new() { (int)WarrantRank.One }
            },
            [(int)WarrantRank.One] = new()
            {
                RequiredRank = new() { (int)WarrantRank.Chief },
                RequiredTimeInGrade = 91,
                NeededLevel = PromotionBoardLevel.Battalion,
                CanPromoteTo = new() { (int)WarrantRank.Two }
            },
            [(int)WarrantRank.Two] = new()
            {
                RequiredRank = new() { (int)WarrantRank.One },
                RequiredTimeInGrade = 122,
                NeededLevel = PromotionBoardLevel.Battalion,
                CanPromoteTo = new() { (int)WarrantRank.Three }
            },
            [(int)WarrantRank.Three] = new()
            {
                RequiredRank = new() { (int)WarrantRank.Two },
                RequiredTimeInGrade = 122,
                RequiresCShopLeadership = true,
                RequiredTimeInBillet = 91,
                NeededLevel = PromotionBoardLevel.Battalion,
                CanPromoteTo = new() { (int)WarrantRank.Four }
            },
            [(int)WarrantRank.Four] = new()
            {
                RequiredRank = new() { (int)WarrantRank.Three },
                RequiredTimeInGrade = 152,
                RequiresCShopLeadership = true,
                RequiredTimeInBillet = 122,
                NeededLevel = PromotionBoardLevel.Battalion,
                CanPromoteTo = new() { (int)WarrantRank.Five }
            },
            [(int)WarrantRank.Five] = new()
            {
                RequiredRank = new() { (int)WarrantRank.Four },
                RequiredTimeInGrade = 183,
                RequiresCShopCommand = true,
                RequiredBillet = new() { Role.Adjutant },
                RequiredTimeInBillet = 122,
                NeededLevel = PromotionBoardLevel.Battalion
            },

            // Flight ranks us the squad, platoon, battalion board names but aren't actually moving to those locations.
            // They are reouted seprately as pilot ranks.
            [(int)PilotRank.Cadet] = new()
            {
                CanPromoteTo = new() { (int)PilotRank.SeniorCadet }
            },
            [(int)PilotRank.SeniorCadet] = new()
            {
                RequiredRank = new() { (int)PilotRank.Cadet },
                RequiredTimeInGrade = 60,
                NeededLevel = PromotionBoardLevel.Razor,
                CanPromoteTo = new() { (int)PilotRank.Ensign }
            },
            [(int)PilotRank.Ensign] = new()
            {
                RequiredRank = new() { (int)PilotRank.SeniorCadet },
                RequiredTimeInGrade = 80,
                NeededLevel = PromotionBoardLevel.Razor,
                CanPromoteTo = new() { (int)PilotRank.SeniorEnsign }
            },
            [(int)PilotRank.SeniorEnsign] = new()
            {
                RequiredRank = new() { (int)PilotRank.Ensign },
                RequiredTimeInGrade = 100,
                NeededLevel = PromotionBoardLevel.Razor,
                CanPromoteTo = new() { (int)PilotRank.Master, (int)PilotRank.FlightOfficer }
            },
            [(int)PilotRank.Master] = new()
            {
                RequiredRank = new() { (int)PilotRank.SeniorEnsign },
                RequiredTimeInGrade = 250,
                NeededLevel = PromotionBoardLevel.Razor,
                CanPromoteTo = new() { (int)PilotRank.FlightOfficer }
            },
            [(int)PilotRank.FlightOfficer] = new()
            {
                RequiredRank = new() { (int)PilotRank.SeniorEnsign },
                RequiredTimeInGrade = 140,
                NeededLevel = PromotionBoardLevel.Razor,
                CanPromoteTo = new() { (int)PilotRank.JuniorLieutenant }
            },
            [(int)PilotRank.JuniorLieutenant] = new()
            {
                RequiredRank = new() { (int)PilotRank.FlightOfficer },
                RequiredTimeInGrade = 170,
                NeededLevel = PromotionBoardLevel.Razor,
                CanPromoteTo = new() { (int)PilotRank.SecondLieutenant }
            },
            [(int)PilotRank.SecondLieutenant] = new()
            {
                RequiredRank = new() { (int)PilotRank.JuniorLieutenant },
                RequiredTimeInGrade = 200,
                NeededLevel = PromotionBoardLevel.Razor,
                CanPromoteTo = new() { (int)PilotRank.FirstLieutenant }
            },
            [(int)PilotRank.FirstLieutenant] = new()
            {
                RequiredRank = new() { (int)PilotRank.SecondLieutenant },
                RequiredTimeInGrade = 243,
                NeededLevel = PromotionBoardLevel.Razor,
                CanPromoteTo = new() { (int)PilotRank.Captain }
            },
            [(int)PilotRank.Captain] = new()
            {
                RequiredRank = new() { (int)PilotRank.FirstLieutenant },
                RequiredTimeInGrade = 243,
                NeededLevel = PromotionBoardLevel.Razor
            },

            // Save with warden ranks as the pilot ranks.
            [(int)WardenRank.Warden] = new()
            {
                CanPromoteTo = new() { (int)WardenRank.Senior }
            },
            [(int)WardenRank.Senior] = new()
            {
                RequiredRank = new() { (int)WardenRank.Warden },
                RequiredTimeInGrade = 90,
                NeededLevel = PromotionBoardLevel.Warden,
                CanPromoteTo = new() { (int)WardenRank.Veteran }
            },
            [(int)WardenRank.Veteran] = new()
            {
                RequiredRank = new() { (int)WardenRank.Senior },
                RequiredTimeInGrade = 120,
                NeededLevel = PromotionBoardLevel.Warden
            },
            [(int)WardenRank.Chief] = new()
            {
                RequiredBillet = new() { Role.ChiefWarden },
                RequiredTimeInBillet = 60,
                NeededLevel = PromotionBoardLevel.Warden,
                DoesNotRequireLinearProgression = true
            },
            [(int)WardenRank.Master] = new()
            {
                RequiredBillet = new() { Role.MasterWarden },
                RequiredTimeInBillet = 60,
                NeededLevel = PromotionBoardLevel.Warden,
                DoesNotRequireLinearProgression = true
            },
        };
        private static readonly Dictionary<CShop, CShopClaim> ClaimsTree = new()
        {
            [CShop.DepartmentLead] = new()
            {
                ClaimData = new()
                {
                    ["Department Lead"] = new()
                    {
                        "C1",
                        "C3",
                        "C4",
                        "C5",
                        "C6",
                        "C7",
                        "C8"
                    }
                },
                CShopCommand = new()
                {
                    "C1",
                    "C3",
                    "C4",
                    "C5",
                    "C6",
                    "C7",
                    "C8"
                }
            },
            // C1
            [CShop.RosterStaff] = new()
            {
                ClaimData = new()
                {
                    [CShop.RosterStaff.AsFull()] = new()
                    {
                        "Lead",
                        "Assistant",
                        "Sr",
                        "Clerk",
                        "Jr"
                    }
                },
                CShopLeadership = new()
                {
                    "Lead",
                    "Assistant"
                }
            },
            [CShop.DocMainCom] = new()
            {
                ClaimData = new()
                {
                    [CShop.DocMainCom.AsFull()] = new()
                    {
                        "Lead",
                        "Assistant",
                        "Curator",
                        "Proofreader",
                        "Distribution Overseer",
                        "Project Assistant"
                    }
                },
                CShopLeadership = new()
                {
                    "Lead",
                    "Assistant",
                }
            },
            [CShop.RecruitingStaff] = new()
            {
                ClaimData = new()
                {
                    [CShop.RecruitingStaff.AsFull()] = new()
                    {
                        "Lead",
                        "Assistant",
                        "Sr",
                        "Recruiter"
                    }
                },
                CShopLeadership = new()
                {
                    "Lead",
                    "Assistant",
                }
            },
            [CShop.ReturningMemberStaff] = new()
            {
                ClaimData = new()
                {
                    [CShop.ReturningMemberStaff.AsFull()] = new()
                    {
                        "Lead",
                        "Assistant",
                        "Staff"
                    }
                },
                CShopLeadership = new()
                {
                    "Lead",
                    "Assistant",
                }
            },
            [CShop.MedalsStaff] = new()
            {
                ClaimData = new()
                {
                    [CShop.MedalsStaff.AsFull()] = new()
                    {
                        "Lead",
                        "Assistant",
                        "Staff"
                    }
                },
                CShopLeadership = new()
                {
                    "Lead",
                    "Assistant",
                }
            },
            [CShop.BigBrother] = new()
            {
                ClaimData = new()
                {
                    [CShop.BigBrother.AsFull()] = new()
                    {
                        "Lead",
                        "Assistant",
                        "Staff"
                    }
                },
                CShopLeadership = new()
                {
                    "Lead",
                    "Assistant",
                }
            },

            // C3
            [CShop.CampaignManagement] = new()
            {
                ClaimData = new()
                {
                    [CShop.CampaignManagement.AsFull()] = new()
                    {
                        "DeptAssistant"
                    },
                    ["Story Writer"] = new()
                    {
                        "Lead",
                        "Assistant",
                        "Writer"
                    },
                    ["Zeus"] = new()
                    {
                        "Lead",
                        "Assistant",
                        "Zeus"
                    },
                    ["Mission Builder"] = new()
                    {
                        "Lead",
                        "Assistant",
                        "Sr",
                        "Builder",
                        "Jr"
                    },
                    ["Artisan"] = new()
                    {
                        "Lead",
                        "Assistant",
                        "Artisan"
                    },
                    ["Logistics"] = new()
                    {
                        "Staff"
                    }
                },
                CShopLeadership = new()
                {
                    "DeptAssistant",
                    "Lead",
                    "Assistant"
                }
            },
            [CShop.EventManagement] = new()
            {
                ClaimData = new()
                {
                    [CShop.EventManagement.AsFull()] = new()
                    {
                        "Lead",
                        "Assistant",
                        "Documentation",
                        "Host",
                        "Helper"
                    }
                },
                CShopLeadership = new()
                {
                    "Lead",
                    "Assistant"
                }
            },
            // C4
            [CShop.Logistics] = new()
            {
                ClaimData = new()
                {
                    [CShop.Logistics.AsFull()] = new()
                    {
                        "Donations",
                        "Patreon",
                        "TeamSpeak",
                        "Website"
                    }
                },
                CShopLeadership = new()
                {
                    "Donations",
                    "Patreon",
                    "TeamSpeak",
                    "Website"
                }
            },

            // C5
            [CShop.TeamSpeakAdmin] = new()
            {
                ClaimData = new()
                {
                    [CShop.TeamSpeakAdmin.AsFull()] = new()
                    {
                        "Lead",
                        "Assistant",
                        "Staff"
                    }
                },
                CShopLeadership = new()
                {
                    "Lead",
                    "Assistant",
                }
            },
            [CShop.HolositeSupport] = new()
            {
                ClaimData = new()
                {
                    [CShop.HolositeSupport.AsFull()] = new()
                    {
                        "Lead",
                        "Staff"
                    }
                },
                CShopLeadership = new()
                {
                    "Lead",
                }
            },
            [CShop.DiscordManagement] = new()
            {
                ClaimData = new()
                {
                    [CShop.DiscordManagement.AsFull()] = new()
                    {
                        "Lead",
                        "Staff"
                    }
                },
                CShopLeadership = new()
                {
                    "Lead",
                }
            },
            [CShop.TechSupport] = new()
            {
                ClaimData = new()
                {
                    [CShop.TechSupport.AsFull()] = new()
                    {
                        "Lead",
                        "Assistant",
                        "Staff"
                    }
                },
                CShopLeadership = new()
                {
                    "Lead",
                    "Assistant"
                }
            },

            // C6
            [CShop.BCTStaff] = new()
            {
                ClaimData = new()
                {
                    [CShop.BCTStaff.AsFull()] = new()
                    {
                        "Lead",
                        "Assistant",
                        "Sr",
                        "Instructor",
                        "Cadre"
                    }
                },
                CShopLeadership = new()
                {
                    "Lead",
                    "Assistant"
                }
            },
            [CShop.PrivateTrainingInstructor] = new()
            {
                ClaimData = new()
                {
                    [CShop.PrivateTrainingInstructor.AsFull()] = new()
                    {
                        "Lead",
                        "Assistant",
                        "Instructor",
                        "Cadre"
                    }
                },
                CShopLeadership = new()
                {
                    "Lead",
                    "Assistant"
                }
            },
            [CShop.UTCStaff] = new()
            {
                ClaimData = new()
                {
                    [CShop.UTCStaff.AsFull()] = new()
                    {
                        "Lead",
                        "Assistant",
                        "Documentation",
                        "Phase Blue Lead",
                        "Chief",
                        "Sr",
                        "Instructor",
                        "Jr"
                    }
                },
                CShopLeadership = new()
                {
                    "Lead",
                    "Assistant"
                }
            },
            [CShop.QualTrainingStaff] = new()
            {
                ClaimData = new()
                {
                    [CShop.QualTrainingStaff.AsFull()] = new()
                    {
                        "Lead"
                    },
                    ["RTO"] = new()
                    {
                        "Lead",
                        "Instructor",
                        "Cadre"
                    },
                    ["Medical"] = new()
                    {
                        "Lead",
                        "Instructor",
                        "Cadre"
                    },
                    ["Assault"] = new()
                    {
                        "Lead",
                        "Instructor",
                        "Cadre"
                    },
                    ["Support"] = new()
                    {
                        "Lead",
                        "Instructor",
                        "Cadre"
                    },
                    ["Marksman"] = new()
                    {
                        "Lead",
                        "Instructor",
                        "Cadre"
                    },
                    ["Grenadier"] = new()
                    {
                        "Lead",
                        "Instructor",
                        "Cadre"
                    },
                    ["Jump-Master"] = new()
                    {
                        "Lead",
                        "Instructor",
                        "Cadre"
                    },
                    ["Combat Engineer"] = new()
                    {
                        "Lead",
                        "Instructor",
                        "Cadre"
                    }
                },
                CShopLeadership = new()
                {
                    "Lead",
                }
            },

            // C7
            [CShop.ServerManagement] = new()
            {
                ClaimData = new()
                {
                    [CShop.ServerManagement.AsFull()] = new()
                    {
                        "Lead",
                        "Assistant",
                        "Staff",
                        "Minecraft",
                        "Space Engineers"
                    }
                },
                CShopLeadership = new()
                {
                    "Lead",
                    "Assistant"
                }
            },
            [CShop.AuxModTeam] = new()
            {
                ClaimData = new()
                {
                    [CShop.AuxModTeam.AsFull()] = new()
                    {
                        "Lead Developer",
                        "Developer",
                        "Texture Lead",
                        "Texture Team"
                    }
                },
                CShopLeadership = new()
                {
                    "Lead Developer",
                    "Texture Lead"
                }
            },

            // C8
            [CShop.PublicAffairs] = new()
            {
                ClaimData = new()
                {
                    [CShop.PublicAffairs.AsFull()] = new()
                    {
                        "Units",
                        "TCW",
                        "Steam"
                    }
                },
                CShopLeadership = new()
                {
                    "Units",
                    "TCW",
                    "Steam"
                }
            },
            [CShop.MediaOutreach] = new()
            {
                ClaimData = new()
                {
                    [CShop.MediaOutreach.AsFull()] = new()
                    {
                        "Lead",
                        "Assistant",
                        "Mod"
                    },
                    ["YouTube"] = new()
                    {
                        "Lead",
                        "Assistant",
                        "Staff"
                    },
                    ["Reddit"] = new()
                    {
                        "Staff"
                    },
                    ["Facebook"] = new()
                    {
                        "Staff"
                    },
                    ["Twitter"] = new()
                    {
                        "Staff"
                    },
                    ["TikTok"] = new()
                    {
                        "Lead",
                        "Staff"
                    }
                },
                CShopLeadership = new()
                {
                    "Lead",
                    "Assistant"
                }
            },
            [CShop.NewsTeam] = new()
            {
                ClaimData = new()
                {
                    [CShop.NewsTeam.AsFull()] = new()
                    {
                        "Lead",
                        "Managing Editor",
                        "Copy Editor",
                        "News Editor",
                        "Graphics Editor",
                        "Writer",
                        "Photographer"
                    }
                },
                CShopLeadership = new()
                {
                    "Lead"
                }
            }
        };
        #endregion

        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

        public Dictionary<CShop, CShopClaim>? CShopClaimTree { get; set; } = null;
        public Dictionary<string, string>? SectionToPolicyNameDict { get; set; } = null;
        public Dictionary<string, DynamicPolicyAuthorizationPolicyBuilder>? PolicyBuilders { get; set; } = null;

        public WebsiteSettingsService(IDbContextFactory<ApplicationDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task InitalizeAsync()
        {
            await ReloadClaimTreeAsync();
            await ReloadPolicyCacheAsync();
        }

        public async Task ReloadClaimTreeAsync()
        {
            CShopClaimTree = await GetFullClaimsTreeAsync();
        }

        public async Task<Dictionary<CShop, CShopClaim>> GetCachedCShopClaimTreeAsync()
        {
            if (CShopClaimTree is null)
                await ReloadClaimTreeAsync();

            return CShopClaimTree!;
        }

        public async Task SetDefaultSettingsAsync()
        {
            using var _dbContext = _dbContextFactory.CreateDbContext();
            _dbContext.PromotionRequirements.RemoveRange(_dbContext.PromotionRequirements);
            foreach (var pair in NeededTimeForPromotion)
            {
                var old = await _dbContext.PromotionRequirements.FindAsync(pair.Key);
                if (old is not null)
                    _dbContext.Remove(old);

                pair.Value.RequirementsFor = pair.Key;
                _dbContext.PromotionRequirements.Add(pair.Value);
            }

            _dbContext.CShopRoles.RemoveRange(_dbContext.CShopRoles);
            _dbContext.CShopClaims.RemoveRange(_dbContext.CShopClaims);

            foreach (var item in ClaimsTree)
            {
                // CShop Discord Role Bindings
                var oldRole = await _dbContext.CShopRoles.FindAsync(item.Key);
                if (oldRole is not null)
                    _dbContext.CShopRoles.Remove(oldRole);

                var role = new CShopRoleBinding()
                {
                    Key = item.Key
                };

                foreach (var val in item.Value.ClaimData)
                {
                    var old = await _dbContext.CShopClaims.FindAsync(item.Key);
                    if (old is not null)
                        _dbContext.CShopClaims.Remove(old);

                    item.Value.Key = item.Key;
                    _dbContext.CShopClaims.Add(item.Value);

                    var set = new CShopDepartmentBinding()
                    {
                        Id = val.Key
                    };

                    foreach (var part in val.Value)
                        set.Roles.Add(new() { Id = part });

                    role.Departments.Add(set);
                }

                _dbContext.CShopRoles.Add(role);
            }

            List<Enum> vals = new();
            foreach (var i in Enum.GetValues<Role>())
                vals.Add(i);
            foreach (var i in Enum.GetValues<Slot>())
                vals.Add(i);
            foreach (var i in Enum.GetValues<Team>())
                vals.Add(i);
            foreach (var i in Enum.GetValues<Flight>())
                vals.Add(i);
            foreach (var i in Enum.GetValues<TrooperRank>())
                vals.Add(i);
            foreach (var i in Enum.GetValues<RTORank>())
                vals.Add(i);
            foreach (var i in Enum.GetValues<MedicRank>())
                vals.Add(i);
            foreach (var i in Enum.GetValues<PilotRank>())
                vals.Add(i);
            foreach (var i in Enum.GetValues<WarrantRank>())
                vals.Add(i);
            foreach (var i in Enum.GetValues<Qualification>())
                vals.Add(i);

            _dbContext.DiscordRoles.RemoveRange(_dbContext.DiscordRoles);

            foreach (Enum item in vals)
            {
                var name = item.AsQualified();
                var old = await _dbContext.FindAsync<DiscordRoleDetails>(name);
                if (old is not null)
                    _dbContext.DiscordRoles.Remove(old);

                _dbContext.Add(new DiscordRoleDetails()
                {
                    Key = name
                });
            }

            await _dbContext.SaveChangesAsync();

            await ReloadClaimTreeAsync();
        }

        public async Task OverrideCShopClaimSettingsAsync(Dictionary<CShop, CShopClaim> claimTree)
        {
            using var _dbContext = _dbContextFactory.CreateDbContext();
            _dbContext.CShopClaims.RemoveRange(_dbContext.CShopClaims);
            foreach (var item in claimTree)
            {
                var old = await _dbContext.CShopClaims.FindAsync(item.Key);
                if (old is not null)
                    _dbContext.CShopClaims.Remove(old);

                item.Value.Key = item.Key;
                _dbContext.CShopClaims.Add(item.Value);
            }

            await ReloadClaimTreeAsync();

            await _dbContext.SaveChangesAsync();
        }

        public async Task<PromotionDetails?> GetPromotionRequirementsAsync(int rank)
        {
            using var _dbContext = _dbContextFactory.CreateDbContext();
            return await _dbContext.FindAsync<PromotionDetails>(rank);
        }

        public async Task<IReadOnlyList<Promotion>> GetEligiblePromotionsAsync(Trooper forTrooper, bool filterOutTrooperToWarrant = false, bool filterOutPendingPromotions = true)
        {
            List<Promotion> promotions = new();

            using var _dbContext = _dbContextFactory.CreateDbContext();

            //gets Cshop levels
            var levels = await GetCshopLevelsAsync(forTrooper.Id);

            List<int> ranks = new();

            //adds any rank that the trooper has
            if (forTrooper.Rank is not null)
                ranks.Add((int)forTrooper.Rank);
            if (forTrooper.RTORank is not null)
                ranks.Add((int)forTrooper.RTORank);
            if (forTrooper.MedicRank is not null)
                ranks.Add((int)forTrooper.MedicRank);
            if (forTrooper.WarrantRank is not null)
                ranks.Add((int)forTrooper.WarrantRank);
            if (forTrooper.WardenRank is not null)
                ranks.Add((int)forTrooper.WardenRank);
            if (forTrooper.PilotRank is not null)
                ranks.Add((int)forTrooper.PilotRank);

            foreach (var rank in ranks)
            {
                var tempDetails = _dbContext.PromotionRequirements
                    .Where(x => (x.DoesNotRequireLinearProgression
                        || (x.RequiredRank != null
                            && x.RequiredRank.Contains(rank)))
                        && x.RequirementsFor > rank)
                    .AsAsyncEnumerable();

                await foreach (var req in tempDetails)
                {
                    if (req.TryGetPromotion(rank, forTrooper, levels, out var promo))
                    {
                        if (filterOutPendingPromotions)
                        {
                            //filters out any pending promotions.
                            if (!forTrooper.PendingPromotions.Any(x =>
                                promo.PromoteFrom == x.PromoteFrom
                                && promo.PromoteTo == x.PromoteTo))
                            {
                                promotions.Add(promo);
                            }
                            else
                            {
                                promotions.Add(promo);
                            }
                        }
                    }
                }
            }

            // Filter out any promotions from trooper to warrant officers
            if (filterOutTrooperToWarrant)
                promotions = promotions.Where(x => !((x.PromoteFrom < 300 || x.PromoteFrom >= 400)
                    && x.PromoteTo >= 300 && x.PromoteTo < 400))
                    .ToList();

            return promotions;
        }

        /// <summary>
        /// Gets a boolean tuple with values determining if a trooper is in C-Shop leadership or command.
        /// </summary>
        /// <param name="trooperId">The ID of the trooper to compare.</param>
        /// <returns>Returns <see cref="(bool, bool)"/> where Item1 is if the Trooper is in C-Shop command and Item2 is if the Trooper is in C-Shop leadership.</returns>
        public async Task<(bool, bool)> GetCshopLevelsAsync(int trooperId)
        {
            using var _dbContext = _dbContextFactory.CreateDbContext();

            var claims = await _dbContext.UserClaims.AsNoTracking()
                .Where(x => x.UserId == trooperId)
                .ToListAsync();

            var trooper = await _dbContext.Users.FindAsync(trooperId);

            if (trooper is null || claims is null) return (false, false);

            bool lead = false;
            bool cmd = false;

            foreach (CShop shop in Enum.GetValues(typeof(CShop)))
            {
                if ((shop & trooper.CShops) == shop && shop != CShop.None)
                {
                    var cData = await _dbContext.CShopClaims.FindAsync(shop);
                    if (cData is null) continue;

                    foreach (var c in claims)
                    {
                        if (lead && cmd) return (cmd, lead);

                        if (cData.ClaimData.TryGetValue(c.ClaimType, out var values))
                        {
                            if (values.Contains(c.ClaimValue, StringComparer.OrdinalIgnoreCase))
                            {
                                if (cData.CShopCommand.Contains(c.ClaimValue, StringComparer.OrdinalIgnoreCase))
                                    cmd = true;
                                if (cData.CShopLeadership.Contains(c.ClaimValue, StringComparer.OrdinalIgnoreCase))
                                    lead = true;
                            }
                        }
                    }
                }
            }

            return (cmd, lead);
        }

        public async Task<Dictionary<CShop, CShopClaim>> GetFullClaimsTreeAsync()
        {
            using var _dbContext = _dbContextFactory.CreateDbContext();

            Dictionary<CShop, CShopClaim> data = new();

            await _dbContext.CShopClaims.AsNoTracking().ForEachAsync(x =>
            {
                if (data.ContainsKey(x.Key))
                {
                    data[x.Key] = x;
                }
                else
                {
                    data.Add(x.Key, x);
                }
            });

            return data;
        }

        public async Task<Dictionary<string, List<string>>> GetClaimDataForCShopAsync(CShop key)
        {
            using var _dbContext = _dbContextFactory.CreateDbContext();
            var data = await _dbContext.CShopClaims.FindAsync(key);

            return data?.ClaimData ?? new();
        }

        public async Task<IReadOnlyList<ulong>?> GetCShopDiscordRolesAsync(Claim claim)
        {
            using var _dbContext = _dbContextFactory.CreateDbContext();
            var data = _dbContext.CShopRoleData
                .Where(x => x.Id == claim.Value)
                .Include(p => p.Parent)
                .AsAsyncEnumerable();

            await foreach (var item in data)
            {
                if (item.Parent.Id == claim.Type)
                    return item.Roles;
            }

            return null;
        }

        public async Task<DiscordRoleDetails?> GetDiscordRoleDetailsAsync(Enum key)
        {
            using var _dbContext = _dbContextFactory.CreateDbContext();
            var details = await _dbContext.FindAsync<DiscordRoleDetails>(key.AsQualified());
            return details;
        }

        public async Task<Dictionary<int, PromotionDetails>> GetSavedPromotionDetails()
        {
            Dictionary<int, PromotionDetails> dict = new();
            using var _dbContext = _dbContextFactory.CreateDbContext();
            var data = _dbContext.PromotionRequirements
                .AsAsyncEnumerable();

            await foreach (var item in data)
            {
                dict.Add(item.RequirementsFor, item);
            }

            return dict;
        }

        public async Task OverridePromotionRequirementsAsync(Dictionary<int, PromotionDetails> details)
        {
            using var _dbContext = _dbContextFactory.CreateDbContext();

            _dbContext.PromotionRequirements.RemoveRange(_dbContext.PromotionRequirements);

            foreach (var pair in details)
            {
                var old = await _dbContext.PromotionRequirements.FindAsync(pair.Key);
                if (old is not null)
                    _dbContext.Remove(old);

                pair.Value.RequirementsFor = pair.Key;
                _dbContext.PromotionRequirements.Add(pair.Value);
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task RemoveForcedTagAsync(Promotion promotion)
        {
            using var _dbContext = _dbContextFactory.CreateDbContext();
            var promo = await _dbContext.FindAsync<Promotion>(promotion.Id);
            if (promo is not null)
            {
                promo.Forced = false;
                await _dbContext.SaveChangesAsync();
            }
        }
        #region Dynamic Policies
        public async Task ReloadPolicyCacheAsync()
        {
            await GenerateSectionToPolicyDictionaryAsync();
            await GeneratePolicyBuildersAsync();
        }

        private async Task GenerateSectionToPolicyDictionaryAsync()
        {
            SectionToPolicyNameDict = new Dictionary<string, string>();

            using var _dbContext = _dbContextFactory.CreateDbContext();
            await _dbContext.PolicySections
                .ForEachAsync(section =>
                {
                    if(!SectionToPolicyNameDict.TryAdd(section.SectionName, section.PolicyName))
                    {
                        SectionToPolicyNameDict[section.SectionName] = section.PolicyName;
                    }
                });
        }

        private async Task GeneratePolicyBuildersAsync()
        {
            PolicyBuilders = new Dictionary<string, DynamicPolicyAuthorizationPolicyBuilder>();

            using var _dbContext = _dbContextFactory.CreateDbContext();
            await _dbContext.DynamicPolicies
                .Include(e => e.RequiredClaims)
                .ForEachAsync(policy =>
                {
                    var builder = new DynamicPolicyAuthorizationPolicyBuilder(policy);
                    builder.RequireAssertion((ctx, _policy) =>
                    {
                        if (ctx.User.IsInRole("Admin")
                               || ctx.User.IsInRole("Manager")) return true;

                        if (ctx.User.IsInRole("Archived")) return false;

                        foreach (var r in _policy.RequiredRoles)
                            if (ctx.User.IsInRole(r)) return true;

                        foreach (var c in _policy.RequiredClaims)
                            if (ctx.User.HasClaim(c.Claim, c.Value)) return true;

                        return false;
                    });

                    if(!PolicyBuilders.TryAdd(policy.PolicyName, builder))
                    {
                        PolicyBuilders[policy.PolicyName] = builder;
                    }
                });
        }

        public async Task<DynamicPolicyAuthorizationPolicyBuilder?> GetPolicyBuilderAsync(string sectionName, bool forceCacheReload = false)
        {
            if (forceCacheReload 
                || SectionToPolicyNameDict is null 
                || PolicyBuilders is null)
                await ReloadPolicyCacheAsync();

            if (SectionToPolicyNameDict!.TryGetValue(sectionName, out var policy))
                if (PolicyBuilders!.TryGetValue(policy, out var builder))
                    return builder;
            // A policy named default will be used as a fallback for any missing
            // policy names, or unassigned policy sections.
            if (PolicyBuilders!.TryGetValue("default".Normalize(), out var defaultBuilder))
                return defaultBuilder;

            return null;
        }

        public async Task<ResultBase> CreatePolicyAsync(DynamicPolicy policy)
        {
            policy.PolicyName = policy.PolicyName.Normalize();

            using var _dbContext = _dbContextFactory.CreateDbContext();
            var old = await _dbContext.FindAsync<DynamicPolicy>(policy.PolicyName);
            if (old is null)
            {
                await _dbContext.AddAsync(policy);
                await _dbContext.SaveChangesAsync();

                await ReloadPolicyCacheAsync();

                return new(true, null);
            }
            else
            {
                return new(false, new List<string>() { $"Failed to create {policy.PolicyName}: A policy by the name of {policy.PolicyName} already exsists." });
            }
        }

        public async Task<ResultBase> UpdatePolicyAsync(DynamicPolicy policy)
        {
            policy.PolicyName = policy.PolicyName.Normalize();

            using var _dbContext = _dbContextFactory.CreateDbContext();
            var old = await _dbContext.FindAsync<DynamicPolicy>(policy.PolicyName);
            if (old is not null)
            {
                _dbContext.Remove(old);
                await _dbContext.SaveChangesAsync();
                await _dbContext.AddAsync(policy);
                await _dbContext.SaveChangesAsync();

                await ReloadPolicyCacheAsync();

                return new(true, null);
            }
            else
            {
                return new(false, new List<string>() { $"Failed to update {policy.PolicyName}: A policy by the name of {policy.PolicyName} does not exsist." });
            }
        }

        public async Task<ResultBase> UpdateOrCreatePolicyAsync(DynamicPolicy policy)
        {
            policy.PolicyName = policy.PolicyName.Normalize();

            using var _dbContext = _dbContextFactory.CreateDbContext();
            var old = await _dbContext.FindAsync<DynamicPolicy>(policy.PolicyName);
            if (old is not null)
            {
                _dbContext.Remove(old);
                await _dbContext.SaveChangesAsync();
            }

            await _dbContext.AddAsync(policy);
            await _dbContext.SaveChangesAsync();

            await ReloadPolicyCacheAsync();

            return new(true, null);
        }

        public async Task<ResultBase> UpdatePolicySectionAsync(PolicySection policySection)
        {
            policySection.SectionName = policySection.SectionName.Normalize();

            using var _dbContext = _dbContextFactory.CreateDbContext();
            var old = await _dbContext.FindAsync<PolicySection>(policySection.SectionName);
            if (old is null)
            {
                old = new()
                {
                    SectionName = policySection.SectionName
                };
            }

            old.PolicyName = policySection.PolicyName;
            await _dbContext.SaveChangesAsync();

            return new(true, null);
        }

        public async Task<ResultBase> DeletePolicyAsync(DynamicPolicy policy, DynamicPolicy? assignFloatingSectionsTo = null)
        {
            policy.PolicyName = policy.PolicyName.Normalize();

            using var _dbContext = _dbContextFactory.CreateDbContext();
            var old = await _dbContext.FindAsync<DynamicPolicy>(policy.PolicyName);
            if (old is not null)
            {
                if (assignFloatingSectionsTo is not null)
                {
                    await _dbContext.Entry(old).Collection(e => e.PolicySections).LoadAsync();
                    foreach(var i in old.PolicySections)
                    {
                        i.PolicyName = assignFloatingSectionsTo.PolicyName;
                    }
                }

                _dbContext.Remove(old);
                await _dbContext.SaveChangesAsync();

                return new(true, null);
            }
            else
            {
                return new(false, new List<string>() { "No policy found to delete." });
            }
        }

        public async Task<PolicySectionResult> GetPolicySectionAsync(string sectionName)
        {
            var name = sectionName.Normalize();

            using var _dbContext = _dbContextFactory.CreateDbContext();
            var section = await _dbContext.FindAsync<PolicySection>(name);

            if (section is not null)
                return new(true, section, null);

            return new(false, null, new List<string>() { "" });
        }
        #endregion
    }
}
