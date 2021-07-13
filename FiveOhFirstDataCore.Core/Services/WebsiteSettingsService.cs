using FiveOhFirstDataCore.Core.Data;
using FiveOhFirstDataCore.Core.Data.Promotions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Services
{
    public class WebsiteSettingsService : IWebsiteSettingsService
    {
        #region Defaults
        private static readonly Dictionary<int, PromotionBoardLevel> NeededBoardLevels = new()
        {
            // recruit and cadet ranks are not handled by promotion boards.

            [(int)TrooperRank.Trooper] = PromotionBoardLevel.Platoon,
            [(int)TrooperRank.SeniorTrooper] = PromotionBoardLevel.Compnay,
            [(int)TrooperRank.VeteranTrooper] = PromotionBoardLevel.Battalion,
            [(int)TrooperRank.Corporal] = PromotionBoardLevel.Battalion,
            [(int)TrooperRank.SeniorCorporal] = PromotionBoardLevel.Battalion,
            [(int)TrooperRank.Sergeant] = PromotionBoardLevel.Battalion,
            [(int)TrooperRank.SeniorSergeant] = PromotionBoardLevel.Battalion,
            [(int)TrooperRank.SergeantMajor] = PromotionBoardLevel.Battalion,
            [(int)TrooperRank.CompanySergeantMajor] = PromotionBoardLevel.Battalion,
            [(int)TrooperRank.BattalionSergeantMajor] = PromotionBoardLevel.Battalion,
            [(int)TrooperRank.SecondLieutenant] = PromotionBoardLevel.Battalion,
            [(int)TrooperRank.FirstLieutenant] = PromotionBoardLevel.Battalion,
            [(int)TrooperRank.Captain] = PromotionBoardLevel.Battalion,
            [(int)TrooperRank.Major] = PromotionBoardLevel.Battalion,

            // Medical cadet not handled by promotion boards.

            [(int)MedicRank.Medic] = PromotionBoardLevel.Platoon,
            [(int)MedicRank.Technician] = PromotionBoardLevel.Battalion,
            [(int)MedicRank.Corporal] = PromotionBoardLevel.Battalion,
            [(int)MedicRank.Sergeant] = PromotionBoardLevel.Battalion,
            [(int)MedicRank.BattalionSergeantMajor] = PromotionBoardLevel.Battalion,

            // RTO cadet not handled by promotion boards.

            [(int)RTORank.Intercommunicator] = PromotionBoardLevel.Platoon,
            [(int)RTORank.Technician] = PromotionBoardLevel.Battalion,
            [(int)RTORank.Corporal] = PromotionBoardLevel.Battalion,
            [(int)RTORank.Sergeant] = PromotionBoardLevel.Battalion,
            [(int)RTORank.BattalionSergeantMajor] = PromotionBoardLevel.Battalion,

            // CW rank not handled here (or maybe) - leaving it in for now.

            [(int)WarrantRank.Chief] = PromotionBoardLevel.Battalion,
            [(int)WarrantRank.One] = PromotionBoardLevel.Battalion,
            [(int)WarrantRank.Two] = PromotionBoardLevel.Battalion,
            [(int)WarrantRank.Three] = PromotionBoardLevel.Battalion,
            [(int)WarrantRank.Four] = PromotionBoardLevel.Battalion,
            [(int)WarrantRank.Five] = PromotionBoardLevel.Battalion,

            // Flight ranks us the squad, platoon, battalion board names but aren't actually moving to those locations.
            // They are reouted seprately as pilot ranks.

            [(int)PilotRank.SeniorCadet] = PromotionBoardLevel.Squad,
            [(int)PilotRank.Ensign] = PromotionBoardLevel.Squad,
            [(int)PilotRank.SeniorEnsign] = PromotionBoardLevel.Platoon,
            [(int)PilotRank.Master] = PromotionBoardLevel.Platoon,
            [(int)PilotRank.FlightOfficer] = PromotionBoardLevel.Platoon,
            [(int)PilotRank.JuniorLieutenant] = PromotionBoardLevel.Platoon,
            [(int)PilotRank.SecondLieutenant] = PromotionBoardLevel.Platoon,
            [(int)PilotRank.FirstLieutenant] = PromotionBoardLevel.Platoon,
            [(int)PilotRank.Captain] = PromotionBoardLevel.Platoon,

            // Save with warden ranks as the pilot ranks.

            [(int)WardenRank.Senior] = PromotionBoardLevel.Platoon,
            [(int)WardenRank.Veteran] = PromotionBoardLevel.Platoon,
            [(int)WardenRank.Chief] = PromotionBoardLevel.Platoon,
            [(int)WardenRank.Master] = PromotionBoardLevel.Platoon,
        };
        private static readonly Dictionary<int, PromotionRequirements> NeededTimeForPromotion = new()
        {
            // recruit and cadet ranks are not handled by promotion boards.

            [(int)TrooperRank.Trooper] = new()
            {
                RequiredRank = new() { (int)TrooperRank.Cadet },
                RequiredTimeInGrade = 30
            },
            [(int)TrooperRank.SeniorTrooper] = new()
            {
                RequiredRank = new() { (int)TrooperRank.Trooper },
                RequiredTimeInGrade = 122
            },
            [(int)TrooperRank.VeteranTrooper] = new()
            {
                RequiredRank = new() { (int)TrooperRank.SeniorTrooper },
                RequiredTimeInGrade = 243
            },
            [(int)TrooperRank.Corporal] = new()
            {
                RequiredRank = new() { (int)TrooperRank.SeniorTrooper, (int)TrooperRank.VeteranTrooper },
                RequiredTimeInGrade = 61,
                RequiredBillet = new() { Role.Lead },
                RequiredTimeInBillet = 61,
                RequiredQualifications = Qualification.RTOQualified | Qualification.ZeusPermit
            },
            [(int)TrooperRank.SeniorCorporal] = new()
            {
                RequiredRank = new() { (int)TrooperRank.Corporal },
                RequiredTimeInGrade = 152
            },
            [(int)TrooperRank.Sergeant] = new()
            {
                RequiredRank = new() { (int)TrooperRank.Corporal },
                InherentRankAuth = new() { (int)TrooperRank.SeniorCorporal },
                RequiredTimeInBillet = 91,
                RequiredBillet = new() { Role.Lead },
                TeamMustBeNull = true,
                RequiredTimeInGrade = 61
            },
            [(int)TrooperRank.SeniorSergeant] = new()
            {
                RequiredRank = new() { (int)TrooperRank.Sergeant },
                RequiredTimeInBillet = 182
            },
            [(int)TrooperRank.SergeantMajor] = new()
            {
                RequiredBillet = new() { Role.SergeantMajor },
                RequiredTimeInBillet = 122
            },
            [(int)TrooperRank.CompanySergeantMajor] = new()
            {
                RequiredBillet = new() { Role.NCOIC, Role.Adjutant },
                RequiredTimeInBillet = 122
            },
            [(int)TrooperRank.BattalionSergeantMajor] = new()
            {
                RequiredBillet = new() { Role.NCOIC },
                RequireSlotBetween = (Slot.Hailstorm, Slot.AvalancheCompany),
                RequiredTimeInBillet = 122
            },
            [(int)TrooperRank.SecondLieutenant] = new()
            {
                RequiredRank = new() { (int)TrooperRank.Sergeant },
                RequiredTimeInGrade = 182,
                TiGWaivedFor = new() { (int)WarrantRank.Five },
                RequiredBillet = new() { Role.Commander, Role.XO },
                RequiredTimeInBillet = 122
            },
            [(int)TrooperRank.FirstLieutenant] = new()
            {
                RequiredRank = new() { (int)TrooperRank.SecondLieutenant },
                RequiredTimeInGrade = 243
            },
            [(int)TrooperRank.Captain] = new()
            {
                RequiredRank = new() { (int)TrooperRank.FirstLieutenant },
                RequiredTimeInGrade = 243,
                RequiredBillet = new() { Role.Commander, Role.XO },
                DivideEqualsZero = 100,
                RequiredTimeInBillet = 122
            },
            [(int)TrooperRank.Major] = new()
            {
                RequiredRank = new() { (int)TrooperRank.Captain },
                RequiredTimeInGrade = 730
            },

            // Medical cadet not handled by promotion boards.

            [(int)MedicRank.Medic] = new()
            {
                RequiredRank = new() { (int)MedicRank.Cadet },
                RequiredTimeInGrade = 61
            },
            [(int)MedicRank.Technician] = new()
            {
                RequiredRank = new() { (int)MedicRank.Medic },
                RequiredTimeInGrade = 183
            },
            [(int)MedicRank.Corporal] = new()
            {
                RequiredBillet = new() { Role.Medic },
                DivideEqualsZero = 10,
                RequiredTimeInBillet = 61
            },
            [(int)MedicRank.Sergeant] = new()
            {
                RequiredRank = new() { (int)MedicRank.Corporal },
                RequiredTimeInGrade = 122,
                RequiredBillet = new() { Role.Medic },
                DivideEqualsZero = 100,
                RequiredTimeInBillet = 183
            },
            [(int)MedicRank.BattalionSergeantMajor] = new()
            {
                RequiredBillet = new() { Role.Medic },
                RequireSlotBetween = (Slot.Hailstorm, Slot.AvalancheCompany),
                RequiredTimeInBillet = 122
            },

            // RTO cadet not handled by promotion boards.

            [(int)RTORank.Intercommunicator] = new()
            {
                RequiredRank = new() { (int)RTORank.Cadet }
            },
            [(int)RTORank.Technician] = new()
            {
                RequiredRank = new() { (int)RTORank.Intercommunicator },
                RequiredTimeInGrade = 183
            },
            [(int)RTORank.Corporal] = new()
            {
                RequiredBillet = new() { Role.RTO },
                DivideEqualsZero = 10,
                RequiredTimeInBillet = 61
            },
            [(int)RTORank.Sergeant] = new()
            {
                RequiredRank = new() { (int)RTORank.Corporal },
                RequiredTimeInGrade = 122,
                RequiredBillet = new() { Role.RTO },
                DivideEqualsZero = 100,
                RequiredTimeInBillet = 122
            },
            [(int)RTORank.BattalionSergeantMajor] = new()
            {
                RequiredBillet = new() { Role.RTO },
                RequireSlotBetween = (Slot.Hailstorm, Slot.AvalancheCompany),
                RequiredTimeInBillet = 122
            },

            // CW rank not handled here (or maybe) - leaving it in for now.

            [(int)WarrantRank.Chief] = new()
            {
                RequiresCShop = true,
                RequiredRank = new() { (int)TrooperRank.Trooper },
                RankOrHigher = true,
                RequiredTimeInGrade = 61
            },
            [(int)WarrantRank.One] = new()
            {
                RequiredRank = new() { (int)WarrantRank.Chief },
                RequiredTimeInGrade = 91
            },
            [(int)WarrantRank.Two] = new()
            {
                RequiredRank = new() { (int)WarrantRank.One },
                RequiredTimeInGrade = 122
            },
            [(int)WarrantRank.Three] = new()
            {
                RequiredRank = new() { (int)WarrantRank.Two },
                RequiredTimeInGrade = 122,
                RequiresCShopLeadership = true,
                RequiredTimeInBillet = 91
            },
            [(int)WarrantRank.Four] = new()
            {
                RequiredRank = new() { (int)WarrantRank.Three },
                RequiredTimeInGrade = 152,
                RequiresCShopLeadership = true,
                RequiredTimeInBillet = 122
            },
            [(int)WarrantRank.Five] = new()
            {
                RequiredRank = new() { (int)WarrantRank.Four },
                RequiredTimeInGrade = 183,
                RequiresCShopCommand = true,
                RequiredBillet = new() { Role.Adjutant },
                RequiredTimeInBillet = 122
            },

            // Flight ranks us the squad, platoon, battalion board names but aren't actually moving to those locations.
            // They are reouted seprately as pilot ranks.

            [(int)PilotRank.SeniorCadet] = new()
            {
                RequiredRank = new() { (int)PilotRank.Cadet },
                RequiredTimeInGrade = 60
            },
            [(int)PilotRank.Ensign] = new()
            {
                RequiredRank = new() { (int)PilotRank.SeniorCadet },
                RequiredTimeInGrade = 80
            },
            [(int)PilotRank.SeniorEnsign] = new()
            {
                RequiredRank = new() { (int)PilotRank.Ensign },
                RequiredTimeInGrade = 100
            },
            [(int)PilotRank.Master] = new()
            {
                RequiredRank = new() { (int)PilotRank.SeniorEnsign },
                RequiredTimeInGrade = 250
            },
            [(int)PilotRank.FlightOfficer] = new()
            {
                RequiredRank = new() { (int)PilotRank.SeniorEnsign },
                RequiredTimeInGrade = 140
            },
            [(int)PilotRank.JuniorLieutenant] = new()
            {
                RequiredRank = new() { (int)PilotRank.FlightOfficer },
                RequiredTimeInGrade = 170
            },
            [(int)PilotRank.SecondLieutenant] = new()
            {
                RequiredRank = new() { (int)PilotRank.JuniorLieutenant },
                RequiredTimeInGrade = 200
            },
            [(int)PilotRank.FirstLieutenant] = new()
            {
                RequiredRank = new() { (int)PilotRank.SecondLieutenant },
                RequiredTimeInGrade = 243
            },
            [(int)PilotRank.Captain] = new()
            {
                RequiredRank = new() { (int)PilotRank.FirstLieutenant },
                RequiredTimeInGrade = 243
            },

            // Save with warden ranks as the pilot ranks.

            [(int)WardenRank.Senior] = new()
            {
                RequiredRank = new() { (int)WardenRank.Warden },
                RequiredTimeInGrade = 90
            },
            [(int)WardenRank.Veteran] = new()
            {
                RequiredRank = new() { (int)WardenRank.Senior },
                RequiredTimeInGrade = 120
            },
            [(int)WardenRank.Chief] = new()
            {
                RequiredBillet = new() { Role.ChiefWarden },
                RequiredTimeInBillet = 60
            },
            [(int)WardenRank.Master] = new()
            {
                RequiredBillet = new() { Role.MasterWarden },
                RequiredTimeInBillet = 60
            },
        };
        #endregion


    }
}
