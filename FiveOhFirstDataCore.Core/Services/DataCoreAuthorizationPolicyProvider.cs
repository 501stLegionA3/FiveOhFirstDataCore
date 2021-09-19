using FiveOhFirstDataCore.Data.Structures;
using FiveOhFirstDataCore.Data.Structures.Auth;

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

using System.Diagnostics.CodeAnalysis;

namespace FiveOhFirstDataCore.Data.Services
{
    public class DataCoreAuthorizationPolicyProvider : IAuthorizationPolicyProvider
    {
        private readonly DefaultAuthorizationPolicyProvider _default;
        private readonly IWebsiteSettingsService _settings;

        public DataCoreAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options,
            IWebsiteSettingsService settings)
        {
            _default = new DefaultAuthorizationPolicyProvider(options);
            _settings = settings;
        }

        public async Task<AuthorizationPolicy> GetDefaultPolicyAsync()
            => await _default.GetDefaultPolicyAsync();

        public async Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
            => await _default.GetFallbackPolicyAsync();

        public async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            var dynamicBuilder = await _settings.GetPolicyBuilderAsync(policyName, false);

            if (dynamicBuilder is not null)
                return dynamicBuilder.Build();

            if (TryGetPolicyBuilder(policyName, out var builder))
            {
                return builder.Build();
            }
            else
            {
                return await _default.GetPolicyAsync(policyName);
            }
        }

        private bool TryGetPolicyBuilder(string policyName,
            [NotNullWhen(true)] out WebsiteSettingsAuthorizationPolicyBuilder? builder)
        {
            builder = new WebsiteSettingsAuthorizationPolicyBuilder(_settings);

            switch (policyName)
            {
                case "RequireC1":
                    builder.RequireAssertion(async (ctx, _s) =>
                    {
                        if (ctx.User.IsInRole("Admin")
                            || ctx.User.IsInRole("Manager")) return true;

                        if (ctx.User.IsInRole("Archived")) return false;

                        var ClaimsTree = await _s.GetCachedCShopClaimTreeAsync();
                        return ctx.User.HasClaim("Department Lead", "C1")
                            || ctx.User.HasClaim(x => ClaimsTree.TryGetValue(CShop.RosterStaff, out var dat) && dat.ClaimData.ContainsKey(x.Type)
                                || ClaimsTree.TryGetValue(CShop.DocMainCom, out dat) && dat.ClaimData.ContainsKey(x.Type)
                                || ClaimsTree.TryGetValue(CShop.RecruitingStaff, out dat) && dat.ClaimData.ContainsKey(x.Type)
                                || ClaimsTree.TryGetValue(CShop.ReturningMemberStaff, out dat) && dat.ClaimData.ContainsKey(x.Type)
                                || ClaimsTree.TryGetValue(CShop.MedalsStaff, out dat) && dat.ClaimData.ContainsKey(x.Type));
                    });
                    return true;
                case "RequireRosterClerk":
                    builder.RequireAssertion(async (ctx, _s) =>
                    {
                        if (ctx.User.IsInRole("Admin")
                            || ctx.User.IsInRole("Manager")) return true;

                        if (ctx.User.IsInRole("Archived")) return false;

                        var ClaimsTree = await _s.GetCachedCShopClaimTreeAsync();
                        return ctx.User.HasClaim("Department Lead", "C1")
                            || ctx.User.HasClaim(x => ClaimsTree.TryGetValue(CShop.RosterStaff, out var dat) && dat.ClaimData.ContainsKey(x.Type));
                    });
                    return true;
                case "RequireRosterClerkOrReturningMemberStaff":
                    builder.RequireAssertion(async (ctx, _s) =>
                    {
                        if (ctx.User.IsInRole("Admin")
                            || ctx.User.IsInRole("Manager")) return true;

                        if (ctx.User.IsInRole("Archived")) return false;

                        var ClaimsTree = await _s.GetCachedCShopClaimTreeAsync();
                        return ctx.User.HasClaim("Department Lead", "C1")
                            || ctx.User.HasClaim(x => ClaimsTree.TryGetValue(CShop.RosterStaff, out var dat) && dat.ClaimData.ContainsKey(x.Type))
                            || ctx.User.HasClaim(x => ClaimsTree.TryGetValue(CShop.ReturningMemberStaff, out var dat) && dat.ClaimData.ContainsKey(x.Type));
                    });
                    return true;
                case "RequireRecruiter":
                    builder.RequireAssertion(async (ctx, _s) =>
                    {
                        if (ctx.User.IsInRole("Admin")
                               || ctx.User.IsInRole("Manager")) return true;

                        if (ctx.User.IsInRole("Archived")) return false;

                        var ClaimsTree = await _s.GetCachedCShopClaimTreeAsync();
                        return ctx.User.HasClaim("Department Lead", "C1")
                            || ctx.User.HasClaim(x => ClaimsTree.TryGetValue(CShop.RecruitingStaff, out var dat) && dat.ClaimData.ContainsKey(x.Type));
                    });
                    return true;
                case "RequireRecruiterOrReturningMemberStaff":
                    builder.RequireAssertion(async (ctx, _s) =>
                    {
                        if (ctx.User.IsInRole("Admin")
                            || ctx.User.IsInRole("Manager")) return true;

                        if (ctx.User.IsInRole("Archived")) return false;

                        var ClaimsTree = await _s.GetCachedCShopClaimTreeAsync();
                        return ctx.User.HasClaim("Department Lead", "C1")
                            || ctx.User.HasClaim(x => ClaimsTree.TryGetValue(CShop.RecruitingStaff, out var dat) && dat.ClaimData.ContainsKey(x.Type))
                            || ctx.User.HasClaim(x => ClaimsTree.TryGetValue(CShop.ReturningMemberStaff, out var dat) && dat.ClaimData.ContainsKey(x.Type));
                    });
                    return true;
                case "RequireReturningMemberStaff":
                    builder.RequireAssertion(async (ctx, _s) =>
                    {
                        if (ctx.User.IsInRole("Admin")
                            || ctx.User.IsInRole("Manager")) return true;

                        if (ctx.User.IsInRole("Archived")) return false;

                        var ClaimsTree = await _s.GetCachedCShopClaimTreeAsync();
                        return ctx.User.HasClaim("Department Lead", "C1")
                            || ctx.User.HasClaim(x => ClaimsTree.TryGetValue(CShop.ReturningMemberStaff, out var dat) && dat.ClaimData.ContainsKey(x.Type));
                    });
                    return true;
                case "RequireC3":
                    builder.RequireAssertion(async (ctx, _s) =>
                    {
                        if (ctx.User.IsInRole("Admin")
                            || ctx.User.IsInRole("Manager")) return true;

                        if (ctx.User.IsInRole("Archived")) return false;

                        var ClaimsTree = await _s.GetCachedCShopClaimTreeAsync();
                        return ctx.User.HasClaim("Department Lead", "C3")
                            || ctx.User.HasClaim(x => ClaimsTree.TryGetValue(CShop.CampaignManagement, out var dat) && dat.ClaimData.ContainsKey(x.Type)
                                || ClaimsTree.TryGetValue(CShop.EventManagement, out dat) && dat.ClaimData.ContainsKey(x.Type));
                    });
                    return true;
                case "RequireC4":
                    builder.RequireAssertion(async (ctx, _s) =>
                    {
                        if (ctx.User.IsInRole("Admin")
                            || ctx.User.IsInRole("Manager")) return true;

                        if (ctx.User.IsInRole("Archived")) return false;

                        var ClaimsTree = await _s.GetCachedCShopClaimTreeAsync();
                        return ctx.User.HasClaim("Department Lead", "C4")
                            || ctx.User.HasClaim(x => ClaimsTree.TryGetValue(CShop.Logistics, out var dat) && dat.ClaimData.ContainsKey(x.Type));
                    });
                    return true;
                case "RequireC5":
                    builder.RequireAssertion(async (ctx, _s) =>
                    {
                        if (ctx.User.IsInRole("Admin")
                            || ctx.User.IsInRole("Manager")) return true;

                        if (ctx.User.IsInRole("Archived")) return false;

                        var ClaimsTree = await _s.GetCachedCShopClaimTreeAsync();
                        return ctx.User.HasClaim("Department Lead", "C5")
                            || ctx.User.HasClaim(x => ClaimsTree.TryGetValue(CShop.TeamSpeakAdmin, out var dat) && dat.ClaimData.ContainsKey(x.Type)
                                || ClaimsTree.TryGetValue(CShop.HolositeSupport, out dat) && dat.ClaimData.ContainsKey(x.Type)
                                || ClaimsTree.TryGetValue(CShop.DiscordManagement, out dat) && dat.ClaimData.ContainsKey(x.Type)
                                || ClaimsTree.TryGetValue(CShop.TechSupport, out dat) && dat.ClaimData.ContainsKey(x.Type));
                    });
                    return true;
                case "RequireC6":
                    builder.RequireAssertion(async (ctx, _s) =>
                    {
                        if (ctx.User.IsInRole("Admin")
                            || ctx.User.IsInRole("Manager")) return true;

                        if (ctx.User.IsInRole("Archived")) return false;

                        var ClaimsTree = await _s.GetCachedCShopClaimTreeAsync();
                        return ctx.User.HasClaim("Department Lead", "C6")
                            || ctx.User.HasClaim(x => ClaimsTree.TryGetValue(CShop.BCTStaff, out var dat) && dat.ClaimData.ContainsKey(x.Type)
                                || ClaimsTree.TryGetValue(CShop.PrivateTrainingInstructor, out dat) && dat.ClaimData.ContainsKey(x.Type)
                                || ClaimsTree.TryGetValue(CShop.UTCStaff, out dat) && dat.ClaimData.ContainsKey(x.Type)
                                || ClaimsTree.TryGetValue(CShop.QualTrainingStaff, out dat) && dat.ClaimData.ContainsKey(x.Type));
                    });
                    return true;
                case "RequireQualificationInstructor":
                    builder.RequireAssertion(async (ctx, _s) =>
                    {
                        if (ctx.User.IsInRole("Admin")
                            || ctx.User.IsInRole("Manager")) return true;

                        if (ctx.User.IsInRole("Archived")) return false;

                        var ClaimsTree = await _s.GetCachedCShopClaimTreeAsync();
                        return ctx.User.HasClaim("Department Lead", "C6")
                            || ctx.User.HasClaim(x => ClaimsTree.TryGetValue(CShop.QualTrainingStaff, out var dat) && dat.ClaimData.ContainsKey(x.Type));
                    });
                    return true;
                case "RequireC7":
                    builder.RequireAssertion(async (ctx, _s) =>
                    {
                        if (ctx.User.IsInRole("Admin")
                            || ctx.User.IsInRole("Manager")) return true;

                        if (ctx.User.IsInRole("Archived")) return false;

                        var ClaimsTree = await _s.GetCachedCShopClaimTreeAsync();
                        return ctx.User.HasClaim("Department Lead", "C7")
                            || ctx.User.HasClaim(x => ClaimsTree.TryGetValue(CShop.ServerManagement, out var dat) && dat.ClaimData.ContainsKey(x.Type)
                                || ClaimsTree.TryGetValue(CShop.AuxModTeam, out dat) && dat.ClaimData.ContainsKey(x.Type));
                    });
                    return true;
                case "RequireC8":
                    builder.RequireAssertion(async (ctx, _s) =>
                    {
                        if (ctx.User.IsInRole("Admin")
                            || ctx.User.IsInRole("Manager")) return true;

                        if (ctx.User.IsInRole("Archived")) return false;

                        var ClaimsTree = await _s.GetCachedCShopClaimTreeAsync();
                        return ctx.User.HasClaim("Department Lead", "C8")
                            || ctx.User.HasClaim(x => ClaimsTree.TryGetValue(CShop.PublicAffairs, out var dat) && dat.ClaimData.ContainsKey(x.Type)
                                || ClaimsTree.TryGetValue(CShop.MediaOutreach, out dat) && dat.ClaimData.ContainsKey(x.Type)
                                || ClaimsTree.TryGetValue(CShop.NewsTeam, out dat) && dat.ClaimData.ContainsKey(x.Type));
                    });
                    return true;
                default:
                    builder = null;
                    return false;
            }
        }
    }
}
