using FiveOhFirstDataCore.Core.Data;
using FiveOhFirstDataCore.Core.Data.Promotions;
using FiveOhFirstDataCore.Core.Database;

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
        private static readonly Dictionary<int, PromotionRequirements> NeededTimeForPromotion = new()
        {
            // recruit and cadet ranks are not handled by promotion boards.

            [(int)TrooperRank.Trooper] = new()
            {
                RequiredRank = new() { (int)TrooperRank.Cadet },
                RequiredTimeInGrade = 30,
                NeededLevel = PromotionBoardLevel.Platoon
            },
            [(int)TrooperRank.SeniorTrooper] = new()
            {
                RequiredRank = new() { (int)TrooperRank.Trooper },
                RequiredTimeInGrade = 122,
                NeededLevel = PromotionBoardLevel.Compnay
            },
            [(int)TrooperRank.VeteranTrooper] = new()
            {
                RequiredRank = new() { (int)TrooperRank.SeniorTrooper },
                RequiredTimeInGrade = 243,
                NeededLevel = PromotionBoardLevel.Battalion
            },
            [(int)TrooperRank.Corporal] = new()
            {
                RequiredRank = new() { (int)TrooperRank.SeniorTrooper, (int)TrooperRank.VeteranTrooper },
                RequiredTimeInGrade = 61,
                RequiredBillet = new() { Role.Lead },
                RequiredTimeInBillet = 61,
                RequiredQualifications = Qualification.RTOQualified | Qualification.ZeusPermit,
                NeededLevel = PromotionBoardLevel.Battalion
            },
            [(int)TrooperRank.SeniorCorporal] = new()
            {
                RequiredRank = new() { (int)TrooperRank.Corporal },
                RequiredTimeInGrade = 152,
                NeededLevel = PromotionBoardLevel.Battalion
            },
            [(int)TrooperRank.Sergeant] = new()
            {
                RequiredRank = new() { (int)TrooperRank.Corporal },
                InherentRankAuth = new() { (int)TrooperRank.SeniorCorporal },
                RequiredTimeInBillet = 91,
                RequiredBillet = new() { Role.Lead },
                TeamMustBeNull = true,
                RequiredTimeInGrade = 61,
                NeededLevel = PromotionBoardLevel.Battalion
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
                NeededLevel = PromotionBoardLevel.Battalion
            },
            [(int)TrooperRank.CompanySergeantMajor] = new()
            {
                RequiredBillet = new() { Role.NCOIC, Role.Adjutant },
                RequiredTimeInBillet = 122,
                NeededLevel = PromotionBoardLevel.Battalion
            },
            [(int)TrooperRank.BattalionSergeantMajor] = new()
            {
                RequiredBillet = new() { Role.NCOIC },
                SlotMin = Slot.Hailstorm,
                SlotMax = Slot.Hailstorm,
                RequiredTimeInBillet = 122,
                NeededLevel = PromotionBoardLevel.Battalion
            },
            [(int)TrooperRank.SecondLieutenant] = new()
            {
                RequiredRank = new() { (int)TrooperRank.Sergeant },
                RequiredTimeInGrade = 182,
                TiGWaivedFor = new() { (int)WarrantRank.Five },
                RequiredBillet = new() { Role.Commander, Role.XO },
                RequiredTimeInBillet = 122,
                NeededLevel = PromotionBoardLevel.Battalion
            },
            [(int)TrooperRank.FirstLieutenant] = new()
            {
                RequiredRank = new() { (int)TrooperRank.SecondLieutenant },
                RequiredTimeInGrade = 243,
                NeededLevel = PromotionBoardLevel.Battalion
            },
            [(int)TrooperRank.Captain] = new()
            {
                RequiredRank = new() { (int)TrooperRank.FirstLieutenant },
                RequiredTimeInGrade = 243,
                RequiredBillet = new() { Role.Commander, Role.XO },
                DivideEqualsZero = 100,
                RequiredTimeInBillet = 122,
                NeededLevel = PromotionBoardLevel.Battalion
            },
            [(int)TrooperRank.Major] = new()
            {
                RequiredRank = new() { (int)TrooperRank.Captain },
                RequiredTimeInGrade = 730,
                NeededLevel = PromotionBoardLevel.Battalion
            },

            // Medical cadet not handled by promotion boards.

            [(int)MedicRank.Medic] = new()
            {
                RequiredRank = new() { (int)MedicRank.Cadet },
                RequiredTimeInGrade = 61,
                NeededLevel = PromotionBoardLevel.Platoon
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
                NeededLevel = PromotionBoardLevel.Battalion
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
                NeededLevel = PromotionBoardLevel.Battalion
            },

            // RTO cadet not handled by promotion boards.

            [(int)RTORank.Intercommunicator] = new()
            {
                RequiredRank = new() { (int)RTORank.Cadet },
                NeededLevel = PromotionBoardLevel.Platoon
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
                NeededLevel = PromotionBoardLevel.Battalion
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
                NeededLevel = PromotionBoardLevel.Battalion
            },

            // CW rank not handled here (or maybe) - leaving it in for now.

            [(int)WarrantRank.Chief] = new()
            {
                RequiresCShop = true,
                RequiredRank = new() { (int)TrooperRank.Trooper },
                RankOrHigher = true,
                RequiredTimeInGrade = 61,
                NeededLevel = PromotionBoardLevel.Battalion
            },
            [(int)WarrantRank.One] = new()
            {
                RequiredRank = new() { (int)WarrantRank.Chief },
                RequiredTimeInGrade = 91,
                NeededLevel = PromotionBoardLevel.Battalion
            },
            [(int)WarrantRank.Two] = new()
            {
                RequiredRank = new() { (int)WarrantRank.One },
                RequiredTimeInGrade = 122,
                NeededLevel = PromotionBoardLevel.Battalion
            },
            [(int)WarrantRank.Three] = new()
            {
                RequiredRank = new() { (int)WarrantRank.Two },
                RequiredTimeInGrade = 122,
                RequiresCShopLeadership = true,
                RequiredTimeInBillet = 91,
                NeededLevel = PromotionBoardLevel.Battalion
            },
            [(int)WarrantRank.Four] = new()
            {
                RequiredRank = new() { (int)WarrantRank.Three },
                RequiredTimeInGrade = 152,
                RequiresCShopLeadership = true,
                RequiredTimeInBillet = 122,
                NeededLevel = PromotionBoardLevel.Battalion
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

            [(int)PilotRank.SeniorCadet] = new()
            {
                RequiredRank = new() { (int)PilotRank.Cadet },
                RequiredTimeInGrade = 60,
                NeededLevel = PromotionBoardLevel.Battalion
            },
            [(int)PilotRank.Ensign] = new()
            {
                RequiredRank = new() { (int)PilotRank.SeniorCadet },
                RequiredTimeInGrade = 80,
                NeededLevel = PromotionBoardLevel.Battalion
            },
            [(int)PilotRank.SeniorEnsign] = new()
            {
                RequiredRank = new() { (int)PilotRank.Ensign },
                RequiredTimeInGrade = 100,
                NeededLevel = PromotionBoardLevel.Battalion
            },
            [(int)PilotRank.Master] = new()
            {
                RequiredRank = new() { (int)PilotRank.SeniorEnsign },
                RequiredTimeInGrade = 250,
                NeededLevel = PromotionBoardLevel.Battalion
            },
            [(int)PilotRank.FlightOfficer] = new()
            {
                RequiredRank = new() { (int)PilotRank.SeniorEnsign },
                RequiredTimeInGrade = 140,
                NeededLevel = PromotionBoardLevel.Battalion
            },
            [(int)PilotRank.JuniorLieutenant] = new()
            {
                RequiredRank = new() { (int)PilotRank.FlightOfficer },
                RequiredTimeInGrade = 170,
                NeededLevel = PromotionBoardLevel.Battalion
            },
            [(int)PilotRank.SecondLieutenant] = new()
            {
                RequiredRank = new() { (int)PilotRank.JuniorLieutenant },
                RequiredTimeInGrade = 200,
                NeededLevel = PromotionBoardLevel.Battalion
            },
            [(int)PilotRank.FirstLieutenant] = new()
            {
                RequiredRank = new() { (int)PilotRank.SecondLieutenant },
                RequiredTimeInGrade = 243,
                NeededLevel = PromotionBoardLevel.Battalion
            },
            [(int)PilotRank.Captain] = new()
            {
                RequiredRank = new() { (int)PilotRank.FirstLieutenant },
                RequiredTimeInGrade = 243,
                NeededLevel = PromotionBoardLevel.Battalion
            },

            // Save with warden ranks as the pilot ranks.

            [(int)WardenRank.Senior] = new()
            {
                RequiredRank = new() { (int)WardenRank.Warden },
                RequiredTimeInGrade = 90,
                NeededLevel = PromotionBoardLevel.Battalion
            },
            [(int)WardenRank.Veteran] = new()
            {
                RequiredRank = new() { (int)WardenRank.Senior },
                RequiredTimeInGrade = 120,
                NeededLevel = PromotionBoardLevel.Battalion
            },
            [(int)WardenRank.Chief] = new()
            {
                RequiredBillet = new() { Role.ChiefWarden },
                RequiredTimeInBillet = 60,
                NeededLevel = PromotionBoardLevel.Battalion
            },
            [(int)WardenRank.Master] = new()
            {
                RequiredBillet = new() { Role.MasterWarden },
                RequiredTimeInBillet = 60,
                NeededLevel = PromotionBoardLevel.Battalion
            },
        };
        #endregion

        private readonly ApplicationDbContext _dbContext;

        public WebsiteSettingsService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task SetDefaultSettings()
        {
            foreach(var pair in NeededTimeForPromotion)
            {
                var old = await _dbContext.PromotionRequirements.FindAsync(pair.Key);
                if (old is not null)
                    _dbContext.Remove(old);

                pair.Value.RequirementsFor = pair.Key;
                _dbContext.PromotionRequirements.Add(pair.Value);
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task<PromotionRequirements?> GetPromotionRequirementsAsync(int rank)
            => await _dbContext.FindAsync<PromotionRequirements>(rank);
    }
}
