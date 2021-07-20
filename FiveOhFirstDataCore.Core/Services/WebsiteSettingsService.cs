using FiveOhFirstDataCore.Core.Account;
using FiveOhFirstDataCore.Core.Data;
using FiveOhFirstDataCore.Core.Data.Promotions;
using FiveOhFirstDataCore.Core.Database;

using Microsoft.EntityFrameworkCore;

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
                NeededLevel = PromotionBoardLevel.Compnay,
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

        #endregion

        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

        public WebsiteSettingsService(IDbContextFactory<ApplicationDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task SetDefaultSettings()
        {
            var _dbContext = _dbContextFactory.CreateDbContext();
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

        public async Task<PromotionDetails?> GetPromotionRequirementsAsync(int rank)
        {
            var _dbContext = _dbContextFactory.CreateDbContext();
            return await _dbContext.FindAsync<PromotionDetails>(rank);
        }

        public async Task<IReadOnlyList<Promotion>> GetEligiblePromotionsAsync(Trooper forTrooper)
        {
            List<Promotion> promotions = new();

            var _dbContext = _dbContextFactory.CreateDbContext();

            var levels = await GetCshopLevelsAsync(forTrooper.Id);

            if (forTrooper.Rank is not null)
            {
                int rankVal = (int)forTrooper.Rank;
                var tempDetails = _dbContext.PromotionRequirements
                    .Where(x => x.DoesNotRequireLinearProgression 
                        || (x.RequiredRank != null 
                            && x.RequiredRank.Contains(rankVal)))
                    .AsAsyncEnumerable();

                await foreach(var req in tempDetails)
                {
                    if(req.TryGetPromotion(rankVal, forTrooper, levels, out var promo))
                    {
                        promotions.Add(promo);
                    }
                }
            }

            return promotions;
        }

        public async Task<(bool, bool)> GetCshopLevelsAsync(int trooperId)
        {
            throw new NotImplementedException();
        }
    }
}
