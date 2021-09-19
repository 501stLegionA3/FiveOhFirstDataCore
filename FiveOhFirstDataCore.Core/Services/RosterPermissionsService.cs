using FiveOhFirstDataCore.Data.Account;
using FiveOhFirstDataCore.Data.Structures;

using Microsoft.Extensions.Logging;

using System.Security.Claims;

namespace FiveOhFirstDataCore.Data.Services
{
    public partial class RosterService : IRosterService
    {
        public async Task<bool[]> GetC1PermissionsAsync(ClaimsPrincipal claims)
        {
            var ClaimsTree = await _settings.GetFullClaimsTreeAsync();

            var perms = new bool[] { false, false, false, false, false };

            if (claims.IsInRole("Admin") || claims.IsInRole("Manager"))
                return new bool[] { true, true, true, true, true };

            perms[0] = claims.HasClaim(x => ClaimsTree[CShop.RosterStaff].ClaimData.ContainsKey(x.Type));
            perms[1] = claims.HasClaim(x => ClaimsTree[CShop.DocMainCom].ClaimData.ContainsKey(x.Type));
            perms[2] = claims.HasClaim(x => ClaimsTree[CShop.RecruitingStaff].ClaimData.ContainsKey(x.Type));
            perms[3] = claims.HasClaim(x => ClaimsTree[CShop.ReturningMemberStaff].ClaimData.ContainsKey(x.Type));
            perms[4] = claims.HasClaim(x => ClaimsTree[CShop.MedalsStaff].ClaimData.ContainsKey(x.Type));

            return perms;
        }

        public async Task<bool[]> GetC3PermissionsAsync(ClaimsPrincipal claims)
        {
            var ClaimsTree = await _settings.GetFullClaimsTreeAsync();

            var perms = new bool[] { false, false };

            if (claims.IsInRole("Admin") || claims.IsInRole("Manager"))
                return new bool[] { true, true };

            perms[0] = claims.HasClaim(x => ClaimsTree[CShop.CampaignManagement].ClaimData.ContainsKey(x.Type));
            perms[1] = claims.HasClaim(x => ClaimsTree[CShop.EventManagement].ClaimData.ContainsKey(x.Type));

            return perms;
        }
        public async Task<bool[]> GetC4PermissionsAsync(ClaimsPrincipal claims)
        {
            var ClaimsTree = await _settings.GetFullClaimsTreeAsync();

            var perms = new bool[] { false };

            if (claims.IsInRole("Admin") || claims.IsInRole("Manager"))
                return new bool[] { true };

            perms[0] = claims.HasClaim(x => ClaimsTree[CShop.Logistics].ClaimData.ContainsKey(x.Type));

            return perms;
        }
        public async Task<bool[]> GetC5PermissionsAsync(ClaimsPrincipal claims)
        {
            var ClaimsTree = await _settings.GetFullClaimsTreeAsync();

            var perms = new bool[] { false, false, false, false };

            if (claims.IsInRole("Admin") || claims.IsInRole("Manager"))
                return new bool[] { true, true, true, true };

            perms[0] = claims.HasClaim(x => ClaimsTree[CShop.TeamSpeakAdmin].ClaimData.ContainsKey(x.Type));
            perms[1] = claims.HasClaim(x => ClaimsTree[CShop.HolositeSupport].ClaimData.ContainsKey(x.Type));
            perms[2] = claims.HasClaim(x => ClaimsTree[CShop.DiscordManagement].ClaimData.ContainsKey(x.Type));
            perms[3] = claims.HasClaim(x => ClaimsTree[CShop.TechSupport].ClaimData.ContainsKey(x.Type));

            return perms;
        }
        public async Task<bool[]> GetC6PermissionsAsync(ClaimsPrincipal claims)
        {
            var ClaimsTree = await _settings.GetFullClaimsTreeAsync();

            var perms = new bool[] { false, false, false, false };

            if (claims.IsInRole("Admin") || claims.IsInRole("Manager"))
                return new bool[] { true, true, true, true };

            perms[0] = claims.HasClaim(x => ClaimsTree[CShop.BCTStaff].ClaimData.ContainsKey(x.Type));
            perms[1] = claims.HasClaim(x => ClaimsTree[CShop.PrivateTrainingInstructor].ClaimData.ContainsKey(x.Type));
            perms[2] = claims.HasClaim(x => ClaimsTree[CShop.UTCStaff].ClaimData.ContainsKey(x.Type));
            perms[3] = claims.HasClaim(x => ClaimsTree[CShop.QualTrainingStaff].ClaimData.ContainsKey(x.Type));

            return perms;
        }
        public async Task<bool[]> GetC7PermissionsAsync(ClaimsPrincipal claims)
        {
            var ClaimsTree = await _settings.GetFullClaimsTreeAsync();

            var perms = new bool[] { false, false };

            if (claims.IsInRole("Admin") || claims.IsInRole("Manager"))
                return new bool[] { true, true };

            perms[0] = claims.HasClaim(x => ClaimsTree[CShop.ServerManagement].ClaimData.ContainsKey(x.Type));
            perms[1] = claims.HasClaim(x => ClaimsTree[CShop.AuxModTeam].ClaimData.ContainsKey(x.Type));

            return perms;
        }
        public async Task<bool[]> GetC8PermissionsAsync(ClaimsPrincipal claims)
        {
            var ClaimsTree = await _settings.GetFullClaimsTreeAsync();

            var perms = new bool[] { false, false, false };

            if (claims.IsInRole("Admin") || claims.IsInRole("Manager"))
                return new bool[] { true, true, true };

            perms[0] = claims.HasClaim(x => ClaimsTree[CShop.PublicAffairs].ClaimData.ContainsKey(x.Type));
            perms[1] = claims.HasClaim(x => ClaimsTree[CShop.MediaOutreach].ClaimData.ContainsKey(x.Type));
            perms[2] = claims.HasClaim(x => ClaimsTree[CShop.NewsTeam].ClaimData.ContainsKey(x.Type));

            return perms;
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

            try
            {
                if (admin)
                    await _userManager.AddToRoleAsync(user, "Admin");
                else
                    await _userManager.RemoveFromRoleAsync(user, "Admin");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to modify trooper role.");
            }

            try
            {
                if (manager)
                    await _userManager.AddToRoleAsync(user, "Manager");
                else
                    await _userManager.RemoveFromRoleAsync(user, "Manager");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to modify trooper role.");
            }
        }

        public async Task<List<Trooper>> GetAllowedNameChangersAsync()
        {
            var data = await _userManager.GetUsersForClaimAsync(new("Change", "Name"));
            return data.ToList();
        }
    }
}
