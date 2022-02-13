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

    private ConcurrentDictionary<string, DynamicAuthorizationPolicy> PolicyCache { get; init; } = new();

    public PolicyService(IDbContextFactory<ApplicationDbContext> dbContextFactory)
        => (_dbContextFactory) = (dbContextFactory);

    public async Task InitalizeAsync()
    {
        PolicyCache.Clear();

        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

        var policies = _dbContext.DynamicAuthorizationPolicies
            .AsAsyncEnumerable();

        await foreach(var policy in policies)
            _ = PolicyCache.TryAdd(policy.Key.ToString(), policy);
    }

    public async Task<DynamicAuthroizationPolicyBuilder?> GetPolicyBuilderAsync(string component, bool forceReload)
    {
        if (forceReload)
            await InitalizeAsync();

        if(PolicyCache.TryGetValue(component, out var policy))
        {
            var policyBuilder = new DynamicAuthroizationPolicyBuilder();

            return policyBuilder.WithPolicy(policy);
        }
        
        return null;
    }
}
