using ProjectDataCore.Data.Structures.Model.Policy;
using ProjectDataCore.Data.Structures.Policy;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Services.Policy;
public class PolicyService : IPolicyService
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
    private readonly IModularRosterService _modularRosterService;

    private ConcurrentDictionary<string, DynamicAuthorizationPolicy> PolicyCache { get; init; } = new();
    private bool Initalized { get; set; } = false;
    private bool Initalizing { get; set; } = false;

    public PolicyService(IDbContextFactory<ApplicationDbContext> dbContextFactory, IModularRosterService modularRosterService)
        => (_dbContextFactory, _modularRosterService) = (dbContextFactory, modularRosterService);

    public async Task InitalizeAsync()
    {
        Initalizing = true;

        PolicyCache.Clear();

        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

        var policies = _dbContext.DynamicAuthorizationPolicies
            .AsAsyncEnumerable();

        await foreach (var policy in policies)
        {
            await policy.InitalizePolicyAsync(_modularRosterService, _dbContextFactory);

            _ = PolicyCache.TryAdd(policy.Key.ToString(), policy);

            // Add the admin policy for use on internal admin pages.
            if (policy.AdminPolicy)
                _ = PolicyCache.TryAdd("internal-admin-policy", policy);
        }

        Initalizing = false;
        Initalized = true;
    }

    public async Task<DynamicAuthroizationPolicyBuilder?> GetPolicyBuilderAsync(string component, bool forceReload)
    {
        if (forceReload || (!Initalized && !Initalizing))
            await InitalizeAsync();

        if (Initalizing)
        {
            var builder = new DynamicAuthroizationPolicyBuilder();
            builder.RequireAssertion((ctx, p, bus) => false);
            return builder;
        }

        if (PolicyCache.TryGetValue(component, out var policy))
        {
            var policyBuilder = new DynamicAuthroizationPolicyBuilder();

            policyBuilder.RequireAssertion((ctx, p, bus) =>
            {
                var user = bus.GetLoaclUserServiceFromClaimsPrincipal(ctx.User);

                if (user is not null)
                    return user.ValidateWithPolicy(p);
                else return false;
            });

            return policyBuilder.WithPolicy(policy);
        }
        
        return null;
    }

    public async Task<ActionResult> CreatePolicyAsync(DynamicAuthorizationPolicy policy)
    {
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

        if(!policy.AdminPolicy)
        {
            var admin = await _dbContext.DynamicAuthorizationPolicies.Where(x => x.AdminPolicy)
                .FirstOrDefaultAsync();

            if (admin is null)
                return new(false, new List<string>() { "Admin policy not found." });

            policy.AdministratorPolicyKey = admin.Key;
        }

        await _dbContext.AddAsync(policy);
        await _dbContext.SaveChangesAsync();

        return new(true, null);
    }

    public async Task<ActionResult> UpdatePolicyAsync(Guid key, Action<DynamicAuthorizationPolicyEditModel> action)
    {
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

        var policy = await _dbContext.DynamicAuthorizationPolicies
            .Where(x => x.Key == key)
            .FirstOrDefaultAsync();

        if (policy is null)
            return new(false, new List<string>() { "No policy was found to edit." });

        DynamicAuthorizationPolicyEditModel model = new();
        action.Invoke(model);

        if (model.PolicyName is not null)
            policy.PolicyName = model.PolicyName;

        if (model.AuthorizedSlots is not null)
        {
            foreach(var m in model.AuthorizedSlots)
                if(!policy.AuthorizedSlots.Any(x => x.Key == m.Key))
                    policy.AuthorizedSlots.Add(m);

            foreach (var p in policy.AuthorizedSlots)
                if (!model.AuthorizedSlots.Any(x => x.Key == p.Key))
                    policy.AuthorizedSlots.Remove(p);
        }

        if (model.AuthorizedTrees is not null)
        {
            foreach (var m in model.AuthorizedTrees)
                if (!policy.AuthorizedTrees.Any(x => x.Key == m.Key))
                    policy.AuthorizedTrees.Add(m);

            foreach (var p in policy.AuthorizedTrees)
                if (!model.AuthorizedTrees.Any(x => x.Key == p.Key))
                    policy.AuthorizedTrees.Remove(p);
        }

        if (model.AuthorizedDisplays is not null)
        {
            foreach (var m in model.AuthorizedDisplays)
                if (!policy.AuthorizedDisplays.Any(x => x.Key == m.Key))
                    policy.AuthorizedDisplays.Add(m);

            foreach (var p in policy.AuthorizedDisplays)
                if (!model.AuthorizedDisplays.Any(x => x.Key == p.Key))
                    policy.AuthorizedDisplays.Remove(p);
        }

        if (model.AuthorizedUsers is not null)
        {
            foreach (var m in model.AuthorizedUsers)
                if (!policy.AuthorizedUsers.Any(x => x.Id == m.Id))
                    policy.AuthorizedUsers.Add(m);

            foreach (var p in policy.AuthorizedUsers)
                if (!model.AuthorizedUsers.Any(x => x.Id == p.Id))
                    policy.AuthorizedUsers.Remove(p);
        }

        if (model.Parents is not null)
        {
            foreach (var m in model.Parents)
                if (!policy.Parents.Any(x => x.Key == m.Key))
                    policy.Parents.Add(m);

            foreach (var p in policy.Parents)
                if (!model.Parents.Any(x => x.Key == p.Key))
                    policy.Parents.Remove(p);
        }

        await _dbContext.SaveChangesAsync();

        return new(true, null);
    }

    public async Task<ActionResult> DeletePolicyAsync(Guid key)
    {
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

        var policy = await _dbContext.DynamicAuthorizationPolicies
            .Where(x => x.Key == key)
            .FirstOrDefaultAsync();

        if (policy is null)
            return new(false, new List<string>() { "No policy was found to delete." });

        _dbContext.Remove(policy);
        await _dbContext.SaveChangesAsync();

        return new(true, null);
    }

    public async Task<ActionResult<List<DynamicAuthorizationPolicy>>> GetAllPoliciesAsync()
    {
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

        var policy = await _dbContext.DynamicAuthorizationPolicies
            .ToListAsync();

        return new(true, null, policy);
    }

    public async Task LoadParentsAsync(DynamicAuthorizationPolicy policy)
    {
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

        await _dbContext.Attach(policy).Collection(e => e.Parents).LoadAsync();
    }
}
