using ProjectDataCore.Data.Account;
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

    private ConcurrentDictionary<Guid, DynamicAuthorizationPolicy> PolicyCache { get; init; } = new();
    private bool Initalized { get; set; } = false;
    private bool Initalizing { get; set; } = false;

    private Timer? InitalizeTimer { get; set; }

    public PolicyService(IDbContextFactory<ApplicationDbContext> dbContextFactory, IModularRosterService modularRosterService)
        => (_dbContextFactory, _modularRosterService) = (dbContextFactory, modularRosterService);

    public async Task InitalizeAsync()
    {
        try
        {
            Initalizing = true;

            PolicyCache.Clear();

            await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

            var policies = _dbContext.DynamicAuthorizationPolicies
                .AsAsyncEnumerable();

            await foreach (var policy in policies)
            {
                await policy.InitalizePolicyAsync(_modularRosterService, _dbContextFactory);

                _ = PolicyCache.TryAdd(policy.Key, policy);

                // Add the admin policy for use on internal admin pages.
                if (policy.AdminPolicy)
                    _ = PolicyCache.TryAdd(Guid.Empty, policy);
            }

            Initalizing = false;
            Initalized = true;
        }
        finally
        {
            if (InitalizeTimer is not null)
                await InitalizeTimer.DisposeAsync();

            InitalizeTimer = new(async (x) => await InitalizeAsync(), null, TimeSpan.FromMinutes(15), Timeout.InfiniteTimeSpan);
        }
    }

    public async Task<bool> AuthorizeAsync(DataCoreUser user, Guid policyKey, bool forceReload = false)
    {
        if (forceReload || (!Initalized && !Initalizing))
            await InitalizeAsync();

        if(Initalizing)
        {
            await Task.Delay(10);
            return await AuthorizeAsync(user, policyKey, false);
        }

        if (PolicyCache.TryGetValue(policyKey, out var policy))
        {
            return policy.Validate(user);
        }

        return false;
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

        await InitalizeAsync();

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
            List<RosterSlot> toAdd = new();
            foreach(var m in model.AuthorizedSlots)
                if(!policy.AuthorizedSlots.Any(x => x.Key == m.Key))
                    toAdd.Add(m);

            foreach (var item in toAdd)
            {
                var actual = await _dbContext.FindAsync<RosterSlot>(item.Key);
                if (actual is not null)
                    policy.AuthorizedSlots.Add(actual);
            }

            List<RosterSlot> toRemove = new();
            foreach (var p in policy.AuthorizedSlots)
                if (!model.AuthorizedSlots.Any(x => x.Key == p.Key))
                    toRemove.Remove(p);

            foreach(var item in toRemove)
                policy.AuthorizedSlots.Remove(item);
        }

        if (model.AuthorizedTrees is not null)
        {
            List<RosterTree> toAdd = new();
            foreach (var m in model.AuthorizedTrees)
                if (!policy.AuthorizedTrees.Any(x => x.Key == m.Key))
                    toAdd.Add(m);

            foreach (var item in toAdd)
            {
                var actual = await _dbContext.FindAsync<RosterTree>(item.Key);
                if (actual is not null)
                    policy.AuthorizedTrees.Add(actual);
            }

            List<RosterTree> toRemove = new();
            foreach (var p in policy.AuthorizedTrees)
                if (!model.AuthorizedTrees.Any(x => x.Key == p.Key))
                    toRemove.Remove(p);

            foreach(var item in toRemove)
                policy.AuthorizedTrees.Remove(item);
        }

        if (model.AuthorizedDisplays is not null)
        {
            List<RosterDisplaySettings> toAdd = new();
            foreach (var m in model.AuthorizedDisplays)
                if (!policy.AuthorizedDisplays.Any(x => x.Key == m.Key))
                    toAdd.Add(m);

            foreach (var item in toAdd)
            {
                var actual = await _dbContext.FindAsync<RosterDisplaySettings>(item.Key);
                if (actual is not null)
                    policy.AuthorizedDisplays.Add(actual);
            }

            List<RosterDisplaySettings> toRemove = new();
            foreach (var p in policy.AuthorizedDisplays)
                if (!model.AuthorizedDisplays.Any(x => x.Key == p.Key))
                    toRemove.Remove(p);

            foreach(var item in toRemove)
                policy.AuthorizedDisplays.Remove(item);
        }

        if (model.AuthorizedUsers is not null)
        {
            List<DataCoreUser> toAdd = new();
            foreach (var m in model.AuthorizedUsers)
                if (!policy.AuthorizedUsers.Any(x => x.Id == m.Id))
                    toAdd.Add(m);

            foreach (var item in toAdd)
            {
                var actual = await _dbContext.FindAsync<DataCoreUser>(item.Id);
                if (actual is not null)
                    policy.AuthorizedUsers.Add(actual);
            }

            List<DataCoreUser> toRemove = new();
            foreach (var p in policy.AuthorizedUsers)
                if (!model.AuthorizedUsers.Any(x => x.Id == p.Id))
                    toRemove.Add(p);

            foreach(var item in toRemove)
                policy.AuthorizedUsers.Remove(item);
        }

        if (model.Parents is not null)
        {
            List<DynamicAuthorizationPolicy> toAdd = new();
            foreach (var m in model.Parents)
                if (!policy.Parents.Any(x => x.Key == m.Key))
                    toAdd.Add(m);

            foreach (var item in toAdd)
            {
                var actual = await _dbContext.FindAsync<DynamicAuthorizationPolicy>(item.Key);
                if (actual is not null)
                    policy.Parents.Add(actual);
            }

            List<DynamicAuthorizationPolicy> toRemove = new();
            foreach (var p in policy.Parents)
                if (!model.Parents.Any(x => x.Key == p.Key))
                    toRemove.Remove(p);

            foreach(var item in toRemove)
                policy.Parents.Remove(item);
        }

        if(model.AdminPagePolicy is not null)
        {
            if (model.AdminPagePolicy.Value)
            {
                var oldAdminPage = await _dbContext.DynamicAuthorizationPolicies
                    .Where(x => x.AdminPagePolicy)
                    .FirstOrDefaultAsync();

                if (oldAdminPage is not null)
                    oldAdminPage.AdminPagePolicy = false;
            }

            policy.AdminPagePolicy = model.AdminPagePolicy.Value;
        }

        await _dbContext.SaveChangesAsync();

        await InitalizeAsync();

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

        await InitalizeAsync();

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

    public async Task<ActionResult<DynamicAuthorizationPolicy>> GetAdminPagePolicy()
    {
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

        var item = await _dbContext.DynamicAuthorizationPolicies
            .Where(x => x.AdminPagePolicy)
            .FirstOrDefaultAsync();

        if(item is null)
        {
            item = await _dbContext.DynamicAuthorizationPolicies
                .Where(x => x.AdminPolicy)
                .FirstOrDefaultAsync();
        }

        return new(true, null, item);
    }
}
