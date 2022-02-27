using ProjectDataCore.Data.Account;
using ProjectDataCore.Data.Services;
using ProjectDataCore.Data.Structures.Page.Components;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Policy;
public class DynamicAuthorizationPolicy : DataObject<Guid>
{
    public bool AdminPolicy { get; set; } = false;
    public bool AdminPagePolicy { get; set; } = false;
    public string PolicyName { get; set; }

    public List<RosterSlot> AuthorizedSlots { get; set; } = new();
    public List<RosterTree> AuthorizedTrees { get; set; } = new();
    public List<RosterDisplaySettings> AuthorizedDisplays { get; set; } = new();
    public List<DataCoreUser> AuthorizedUsers { get; set; } = new();

    #region Administrator Policy
    public DynamicAuthorizationPolicy AdministratorPolicy { get; set; }
    public Guid AdministratorPolicyKey { get; set; }

    public List<DynamicAuthorizationPolicy> WebsitePolciies { get; set; } = new();
    #endregion

    #region Parents
    public List<DynamicAuthorizationPolicy> Parents { get; set; } = new();
    public List<DynamicAuthorizationPolicy> Children { get; set; } = new();
    #endregion

    #region Authentication Mapping
    public List<PageComponentSettingsBase> PageComponenetSettings { get; set; }
    public List<TextDisplayComponentSettings> TextDisplayComponentSettings { get; set; }
    #endregion

    #region Non-Database Fields
    public HashSet<Guid> ValidRosterSlots { get; set; } = new();
    public HashSet<Guid> ValidUsers { get; set; } = new();
    #endregion

    public async Task InitalizePolicyAsync(IModularRosterService rosterService, 
        IDbContextFactory<ApplicationDbContext> dbContextFactory, bool inital = true, HashSet<Guid> parents = null, bool test = false)
    {
        if (!test)
        {
            foreach (var slot in AuthorizedSlots)
                ValidRosterSlots.Add(slot.Key);

            foreach (var tree in AuthorizedTrees)
                foreach (var slot in tree.RosterPositions)
                    ValidRosterSlots.Add(slot.Key);

            foreach (var display in AuthorizedDisplays)
            {
                var res = rosterService.LoadFullRosterTreeAsync(display.HostRoster);
                await foreach (bool _ in res)
                    continue;

                Stack<RosterTree> stack = new();
                stack.Push(display.HostRoster);

                while (stack.TryPop(out var tree))
                {
                    foreach (var slot in tree.RosterPositions)
                        ValidRosterSlots.Add(slot.Key);

                    foreach (var t in tree.ChildRosters)
                        stack.Push(t);
                }
            }

            foreach (var user in AuthorizedUsers)
                ValidUsers.Add(user.Id);
        }

        // The admin policy does not have any parents.
        if (!AdminPolicy)
        {
            if (!test)
            {
                if (AdministratorPolicy is not null && inital)
                {
                    await AdministratorPolicy.InitalizePolicyAsync(rosterService, dbContextFactory, false);

                    ValidRosterSlots.UnionWith(AdministratorPolicy.ValidRosterSlots);
                    ValidUsers.UnionWith(AdministratorPolicy.ValidUsers);
                }

                // We want to dispose of this scope as soon as we are done using it.
                await using (var _dbContext = await dbContextFactory.CreateDbContextAsync())
                {
                    await _dbContext.Attach(this).Collection(e => e.Parents).LoadAsync();
                }
            }

            if (inital)
            {
                parents = new();
            }

            foreach (var parent in Parents)
            {
                if (parents.Contains(parent.Key))
                    throw new CircularPolicyException("A circular inheritance was detected.");

                parents.Add(parent.Key);

                await parent.InitalizePolicyAsync(rosterService, dbContextFactory, false, parents, test);

                ValidRosterSlots.UnionWith(parent.ValidRosterSlots);
                ValidUsers.UnionWith(parent.ValidUsers);
            }
        }
    }

    public bool Validate(DataCoreUser user)
    {
        if (ValidUsers.Contains(user.Id))
            return true;

        foreach(var slot in user.RosterSlots)
        {
            if (ValidRosterSlots.Contains(slot.Key))
                return true;
        }

        return false;
    }
}

public class CircularPolicyException : Exception
{
    public CircularPolicyException() : base() { }
    public CircularPolicyException(string? message) : base(message) { }
    public CircularPolicyException(string? message, Exception? innerException) : base(message, innerException) { }
    public CircularPolicyException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}