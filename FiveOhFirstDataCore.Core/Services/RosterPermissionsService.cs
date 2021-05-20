using FiveOhFirstDataCore.Core.Account;
using FiveOhFirstDataCore.Core.Data;
using FiveOhFirstDataCore.Core.Structures;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Services
{
    public partial class RosterService : IRosterService
    {
        public Task<bool[]> GetC1PermissionsAsync(ClaimsPrincipal claims)
        {
            var perms = new bool[] { false, false, false, false, false };

            if (claims.IsInRole("Admin") || claims.IsInRole("Manager"))
                return Task.FromResult(new bool[] { true, true, true, true, true });

            perms[0] = claims.HasClaim(x => CShopExtensions.ClaimsTree[CShop.RosterStaff].ContainsKey(x.Type));
            perms[1] = claims.HasClaim(x => CShopExtensions.ClaimsTree[CShop.DocMainCom].ContainsKey(x.Type));
            perms[2] = claims.HasClaim(x => CShopExtensions.ClaimsTree[CShop.RecruitingStaff].ContainsKey(x.Type));
            perms[3] = claims.HasClaim(x => CShopExtensions.ClaimsTree[CShop.ReturningMemberStaff].ContainsKey(x.Type));
            perms[4] = claims.HasClaim(x => CShopExtensions.ClaimsTree[CShop.MedalsStaff].ContainsKey(x.Type));
            
            return Task.FromResult(perms);
        }

        public Task<bool[]> GetC3PermissionsAsync(ClaimsPrincipal claims)
        {
            var perms = new bool[] { false, false };

            if (claims.IsInRole("Admin") || claims.IsInRole("Manager"))
                return Task.FromResult(new bool[] { true, true });

            perms[0] = claims.HasClaim(x => CShopExtensions.ClaimsTree[CShop.CampaignManagement].ContainsKey(x.Type));
            perms[1] = claims.HasClaim(x => CShopExtensions.ClaimsTree[CShop.EventManagement].ContainsKey(x.Type));

            return Task.FromResult(perms);
        }
        public Task<bool[]> GetC4PermissionsAsync(ClaimsPrincipal claims)
        {
            var perms = new bool[] { false };

            if (claims.IsInRole("Admin") || claims.IsInRole("Manager"))
                return Task.FromResult(new bool[] { true });

            perms[0] = claims.HasClaim(x => CShopExtensions.ClaimsTree[CShop.Logistics].ContainsKey(x.Type));

            return Task.FromResult(perms);
        }
        public Task<bool[]> GetC5PermissionsAsync(ClaimsPrincipal claims)
        {
            var perms = new bool[] { false, false, false, false };

            if (claims.IsInRole("Admin") || claims.IsInRole("Manager"))
                return Task.FromResult(new bool[] { true, true, true, true });

            perms[0] = claims.HasClaim(x => CShopExtensions.ClaimsTree[CShop.TeamSpeakAdmin].ContainsKey(x.Type));
            perms[1] = claims.HasClaim(x => CShopExtensions.ClaimsTree[CShop.HolositeSupport].ContainsKey(x.Type));
            perms[2] = claims.HasClaim(x => CShopExtensions.ClaimsTree[CShop.DiscordManagement].ContainsKey(x.Type));
            perms[3] = claims.HasClaim(x => CShopExtensions.ClaimsTree[CShop.TechSupport].ContainsKey(x.Type));

            return Task.FromResult(perms);
        }
        public Task<bool[]> GetC6PermissionsAsync(ClaimsPrincipal claims)
        {
            var perms = new bool[] { false, false, false, false };

            if (claims.IsInRole("Admin") || claims.IsInRole("Manager"))
                return Task.FromResult(new bool[] { true, true, true, true });

            perms[0] = claims.HasClaim(x => CShopExtensions.ClaimsTree[CShop.BCTStaff].ContainsKey(x.Type));
            perms[1] = claims.HasClaim(x => CShopExtensions.ClaimsTree[CShop.PrivateTrainingInstructor].ContainsKey(x.Type));
            perms[2] = claims.HasClaim(x => CShopExtensions.ClaimsTree[CShop.UTCStaff].ContainsKey(x.Type));
            perms[3] = claims.HasClaim(x => CShopExtensions.ClaimsTree[CShop.QualTrainingStaff].ContainsKey(x.Type));

            return Task.FromResult(perms);
        }
        public Task<bool[]> GetC7PermissionsAsync(ClaimsPrincipal claims)
        {
            var perms = new bool[] { false, false };

            if (claims.IsInRole("Admin") || claims.IsInRole("Manager"))
                return Task.FromResult(new bool[] { true, true });

            perms[0] = claims.HasClaim(x => CShopExtensions.ClaimsTree[CShop.ServerManagement].ContainsKey(x.Type));
            perms[1] = claims.HasClaim(x => CShopExtensions.ClaimsTree[CShop.AuxModTeam].ContainsKey(x.Type));

            return Task.FromResult(perms);
        }
        public Task<bool[]> GetC8PermissionsAsync(ClaimsPrincipal claims)
        {
            var perms = new bool[] { false, false, false };

            if (claims.IsInRole("Admin") || claims.IsInRole("Manager"))
                return Task.FromResult(new bool[] { true, true, true });

            perms[0] = claims.HasClaim(x => CShopExtensions.ClaimsTree[CShop.PublicAffairs].ContainsKey(x.Type));
            perms[1] = claims.HasClaim(x => CShopExtensions.ClaimsTree[CShop.MediaOutreach].ContainsKey(x.Type));
            perms[2] = claims.HasClaim(x => CShopExtensions.ClaimsTree[CShop.NewsTeam].ContainsKey(x.Type));

            return Task.FromResult(perms);
        }

        public async Task<(bool, bool)> GetAdminAndManagerValuesAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            bool admin = await _userManager.IsInRoleAsync(user, "Admin");
            bool manager = await _userManager.IsInRoleAsync(user, "Manager");

            return (admin, manager);
        }

        public async Task SaveAdminAndManagerValuesAsync(bool admin, bool manager, string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if(admin)
                await _userManager.AddToRoleAsync(user, "Admin");
            if (manager)
                await _userManager.AddToRoleAsync(user, "Manager");
        }

        public async Task<List<Trooper>> GetAllowedNameChangersAsync()
        {
            var data = await _userManager.GetUsersForClaimAsync(new("Change", "Name"));
            return data.ToList();
        }
    }
}
