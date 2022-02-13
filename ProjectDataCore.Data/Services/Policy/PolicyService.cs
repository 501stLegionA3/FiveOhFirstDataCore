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

    public PolicyService(IDbContextFactory<ApplicationDbContext> dbContextFactory, IModularRosterService modularRosterService)
        => (_dbContextFactory, _modularRosterService) = (dbContextFactory, modularRosterService);

    public async Task InitalizeAsync()
    {
        PolicyCache.Clear();

        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

        var policies = _dbContext.DynamicAuthorizationPolicies
            .AsAsyncEnumerable();

        await foreach (var policy in policies)
        {
            await policy.InitalizePolicyAsync(_modularRosterService, _dbContextFactory);

            _ = PolicyCache.TryAdd(policy.Key.ToString(), policy);
        }
    }

    public async Task<DynamicAuthroizationPolicyBuilder?> GetPolicyBuilderAsync(string component, bool forceReload)
    {
        if (forceReload)
            await InitalizeAsync();

        if(PolicyCache.TryGetValue(component, out var policy))
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
            policy.AuthorizedSlots = model.AuthorizedSlots;

        if (model.AuthorizedTrees is not null)
            policy.AuthorizedTrees = model.AuthorizedTrees;

        if (model.AuthorizedDisplays is not null)
            policy.AuthorizedDisplays = model.AuthorizedDisplays;

        if (model.AuthorizedUsers is not null)
            policy.AuthorizedUsers = model.AuthorizedUsers;

        if (model.Parents is not null)
            policy.Parents = model.Parents;

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
}
