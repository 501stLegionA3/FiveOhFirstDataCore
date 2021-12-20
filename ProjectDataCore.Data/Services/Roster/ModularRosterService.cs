using ProjectDataCore.Data.Account;

using System.Runtime.Serialization;

namespace ProjectDataCore.Data.Services;

public class ModularRosterService : IModularRosterService
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

    public ModularRosterService(IDbContextFactory<ApplicationDbContext> dbContextFactory)
        => (_dbContextFactory) = (dbContextFactory);

    #region Roster Tree
    public async Task<ActionResult<Guid>> AddRosterTreeAsync(string name, Guid? parentTree = null, int position = 0)
    {
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

        var tree = new RosterTree()
        {
            Name = name
        };

        var tracker = await _dbContext.AddAsync(tree);
        await _dbContext.SaveChangesAsync();

        if (parentTree is not null)
        {
            await tracker.ReloadAsync();

            var parent = await _dbContext.RosterTrees
                .Where(x => x.Key == parentTree)
                .Include(x => x.ChildRosters)
                .FirstOrDefaultAsync();

            if (parent is null)
                return new(false, new List<string> { "No parent was found by the provided roster ID." });

            tree.ParentRosters.Add(parent);
            tree.Order.Add(new()
            {
                ParentObjectId = parent.Key,
                TreeToOrderId = tree.Key,
                Order = 0,
            });

            await _dbContext.SaveChangesAsync();
        }

        if (parentTree is not null)
        {
            await tree.MovePositionAsync(_dbContext, parentTree.Value, position);
            await _dbContext.SaveChangesAsync();
        }

        await tracker.ReloadAsync();

        return new(true, null, tree.Key);
    }

    public async Task<ActionResult> UpdateRosterTreeAsync(Guid tree, Action<RosterTreeEditModel> action)
    {
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

        var treeObject = await _dbContext.RosterTrees
            .Where(x => x.Key == tree)
            .Include(x => x.ParentRosters)
            .FirstOrDefaultAsync();

        if (treeObject is null)
            return new(false, new List<string> { "No Roster Tree object was able to be found for the provided ID." });

        RosterTreeEditModel update = new();
        action.Invoke(update);

        if(update.Name is not null)
            treeObject.Name = update.Name;
        // If there are new parents to add ...
        if (update.AddParents is not null)
        {
            // ... for all the new parents to add ...
            foreach(var guid in update.AddParents)
            {
                // ... find and load the parent ...
                var parent = await _dbContext.RosterTrees
                    .Where(x => x.Key == guid)
                    .Include(x => x.ChildRosters)
                    .FirstOrDefaultAsync();

                // ... supress missing parent errors and just move on...
                if (parent is null) continue;
                // ... check to make sure this relation does not already exist ...
                if(!treeObject.ParentRosters.Any(x => x.Key == parent.Key))
                {
                    // ... then add the relation ...
                    treeObject.ParentRosters.Add(parent);
                    treeObject.Order.Add(new()
                    {
                        ParentObjectId = parent.Key,
                        Order = 0
                    });

                    await treeObject.MovePositionAsync(_dbContext, parent.Key, 0);
                }
            }

            // ... save changes to ensure order values propogate for later ...
            await _dbContext.SaveChangesAsync();
        }

        // ... if there are parents to remove ...
        if(update.RemoveParents is not null)
        {
            // ... find guids to remove ...
            var toRemove = treeObject.ParentRosters
                            .Where(x => update.RemoveParents.Contains(x.Key));
            // ... then for each item to remove ...
            foreach(var item in toRemove)
            {
                // ... remove it ...
                treeObject.ParentRosters.Remove(item);
                // ... and remove the order object for it ...
                var orderObject = treeObject.Order.FirstOrDefault(x =>
                                    x.ParentObjectId == item.Key);
                if(orderObject is not null)
                    treeObject.Order.Remove(orderObject);
            }
        }

        await _dbContext.SaveChangesAsync();

        if (update.Order is not null)
        {
            await treeObject.MovePositionAsync(_dbContext,
                update.Order.Value.Item1, update.Order.Value.Item2);

            await _dbContext.SaveChangesAsync();
        }

        return new(true, null);
    }

    public async Task<ActionResult> AddChildRosterAsync(Guid tree, Guid child, int position)
    {
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

        var childTree = await _dbContext.FindAsync<RosterTree>(child);

        if (childTree is null)
            return new(false, new List<string> { "No child roster tree was found." });

        var parentTree = await _dbContext.FindAsync<RosterTree>(tree);

        if (parentTree is null)
            return new(false, new List<string> { "No parent roster tree was found." });

        childTree.ParentRosters.Add(parentTree);
        childTree.Order.Add(new()
        {
            ParentObjectId = parentTree.Key,
            Order = position
        });

        await _dbContext.SaveChangesAsync();

        // Perform a reorder operation to insert at the correct spot.
        await childTree.MovePositionAsync(_dbContext, tree, position);

        await _dbContext.SaveChangesAsync();

        return new(true, null);
    }

    public async Task<ActionResult> RemoveChildRosterAsync(Guid tree, Guid child)
    {
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();
        // Find the parent ...
        var parentTree = await _dbContext.RosterTrees
            .Where(x => x.Key == tree)
            .Include(x => x.ChildRosters)
            .FirstOrDefaultAsync();

        if (parentTree is null)
            return new(false, new List<string> { "No parent roster tree was found." });
        // ... then the child based on this parent ...
        var childData = parentTree.ChildRosters.FirstOrDefault(x => x.Key == child);

        if(childData is null)
            return new(false, new List<string> { "No child roster tree was found." });
        // ... then detach the child ...
        parentTree.ChildRosters.Remove(childData);
        var childOrder = childData.Order.FirstOrDefault(x => x.ParentObjectId == tree);
        // ... and delete the order object if it exiists ...
        if (childOrder is not null)
        {
            _dbContext.Remove(childOrder);
        }

        await _dbContext.SaveChangesAsync();

        return new(true, null);
    }

    public async Task<ActionResult> RemoveRosterTreeAsync(Guid tree)
    {
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

        var rosterTree = await _dbContext.FindAsync<RosterTree>(tree);

        if (rosterTree is null)
            return new(false, new List<string> { "No roster tree was found." });

        _dbContext.Remove(rosterTree);

        await _dbContext.SaveChangesAsync();

        return new(true, null);
    }
    #endregion

    #region Roster Position
    public async Task<ActionResult> AddRosterSlotAsync(string name, Guid parentTree, int position)
    {
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

        var rosterTree = await _dbContext.RosterTrees
            .Where(x => x.Key == parentTree)
            .Include(x => x.RosterPositions)
            .FirstOrDefaultAsync();

        if (rosterTree is null)
            return new(false, new List<string> { "No parent tree was found to add a slot to." });

        var slot = new RosterSlot()
        {
            Name = name,
            Order = new()
            {
                Order = rosterTree.RosterPositions.Count,
                ParentObjectId = rosterTree.Key
            },
            ParentRosterId = rosterTree.Key,
        };

        await _dbContext.AddAsync(slot);
        await _dbContext.SaveChangesAsync();

        await slot.MovePositionAsync(_dbContext, position);

        await _dbContext.SaveChangesAsync();

        return new(true, null);
    }

    public async Task<ActionResult> UpdateRosterSlotAsync(Guid slot, Action<RosterSlotEditModel> action)
    {
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

        var slotData = await _dbContext.FindAsync<RosterSlot>(slot);

        if (slotData is null)
            return new(false, new List<string> { "No slot found for the provided ID." });
        // Get an edit model
        RosterSlotEditModel update = new();
        action.Invoke(update);
        // If the user ID has a value...
        if(update.UserId.HasValue)
            // ...update the occupide ID.
            slotData.OccupiedById = update.UserId.Value;
        // If the position name has a value...
        if (update.Name is not null)
            // ...update the position name.
            slotData.Name = update.Name;
        // If the parent tree ID has a value...
        if (update.RosterParentId is not null)
        {
            // ... attempt to get the parent tree object,
            // including all roster positions ...
            var parentTree = await _dbContext.RosterTrees
                .Where(x => x.Key == update.RosterParentId.Value)
                .Include(x => x.RosterPositions)
                .FirstOrDefaultAsync();

            // ... if the parent tree is null, return a failure...
            if (parentTree is null)
                return new(false, new List<string> { "No parent roster found to swtich to." });

            // ... otherwise update the slot with a new parent...
            slotData.Order = new()
            {
                ParentObjectId = parentTree.Key,
                Order = parentTree.RosterPositions.Count
            };
            slotData.ParentRosterId = parentTree.Key;
        }

        await _dbContext.SaveChangesAsync();

        if (update.Order is not null)
        {
            await slotData.MovePositionAsync(_dbContext, update.Order.Value);

            // ... then save changes.
            await _dbContext.SaveChangesAsync();
        }

        return new(true, null);
    }

    public async Task<ActionResult> RemoveRosterSlotAsync(Guid slot)
    {
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

        var rosterSlot = await _dbContext.FindAsync<RosterSlot>(slot);

        if (rosterSlot is null)
            return new(false, new List<string> { "No roster slot was found." });

        _dbContext.Remove(rosterSlot);

        await rosterSlot.MovePositionAsync(_dbContext, -1);

        await _dbContext.SaveChangesAsync();

        return new(true, null);
    }
    #endregion

    #region Roster Display Settings
    public async Task<ActionResult> AddRosterDisplaySettingsAsync(string name, Guid host)
    {
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

        var settings = new RosterDisplaySettings()
        {
            Name = name,
            HostRosterId = host
        };

        await _dbContext.AddAsync(settings);
        await _dbContext.SaveChangesAsync();

        return new(true, null);
    }

    public async Task<ActionResult> UpdateRosterDisplaySettingsAsync(Guid settings, Action<RosterDisplaySettingsEditModel> action)
    {
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

        var settingsObject = await _dbContext.FindAsync<RosterDisplaySettings>(settings);

        if (settingsObject is null)
            return new(false, new List<string> { "No settings object found for the provided ID" });

        RosterDisplaySettingsEditModel update = new();
        action.Invoke(update);

        if(update.Name is not null)
            settingsObject.Name = update.Name;

        if(update.HostRosterId is not null)
            settingsObject.HostRosterId = update.HostRosterId.Value;

        await _dbContext.SaveChangesAsync();

        return new(true, null);
    }

    public async Task<ActionResult> RemoveRosterDisplaySettingsAsync(Guid settings)
    {
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

        var settingsObject = await _dbContext.FindAsync<RosterDisplaySettings>(settings);

        if (settingsObject is null)
            return new(false, new List<string> { "No settings object found for the provided ID" });

        _dbContext.Remove(settingsObject);
        await _dbContext.SaveChangesAsync();

        return new(true, null);
    }
    #endregion

    #region Get Roster Display
    public async IAsyncEnumerable<bool> LoadFullRosterTreeAsync(RosterTree tree, List<DataCoreUser>? userList = null)
    {
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

        // attach the tree object...
        var obj = _dbContext.Attach(tree);

        if (obj is null)
            // ... a roster tree was not found,
            throw new MissingRosterTreeException("The base roster tree can not be loaded.");

        // ... then load the base roster positions ...
        await obj.Collection(e => e.RosterPositions)
            .Query()
            .OrderBy(e => e.Order.Order)
            .LoadAsync();

        // ... if it is not null ...
        if(userList is not null)
        {
            // ... then add the loaded trooperes to the user list ...
            // (defined select statement to prevet null warnings)
            userList.AddRange(tree.RosterPositions
                .Where(x => x.OccupiedBy is not null)
                .Select<RosterSlot, DataCoreUser>(x => x.OccupiedBy));
        }

        // ... and return true now that some data is loaded ...
        yield return true;
        // ... then load the child rosters ...
        await obj.Collection(e => e.ChildRosters).LoadAsync();
        // ... and place the child rosters into a queue ...
        Queue<RosterTree> rosters = new();
        foreach (var t in tree.ChildRosters)
            rosters.Enqueue(t);
        // ... then for each child roster ...
        while(rosters.TryDequeue(out RosterTree? roster))
        {
            obj = _dbContext.Entry(roster);

            // ... then load the roster positions ...
            await obj.Collection(e => e.RosterPositions)
                .Query()
                .OrderBy(e => e.Order.Order)
                .LoadAsync();

            if(userList is not null)
            {
                // ... then add the loaded trooperes to the user list ...
                // (defined select statement to prevet null warnings)
                userList.AddRange(roster.RosterPositions
                    .Where(x => x.OccupiedBy is not null)
                    .Select<RosterSlot, DataCoreUser>(x => x.OccupiedBy));
            }

            // ... let the page know a new roster has been loaded ...
            yield return true;

            // ... and load + enque any child further child rosters ...
            await obj.Collection(e => e.ChildRosters).LoadAsync();
            foreach (var t in roster.ChildRosters)
                rosters.Enqueue(t);
        }

        // ... then let the page know to refresh one more time ...
        yield return true;
    }

    public async Task<ActionResult<RosterTree>> GetRosterTreeForSettingsAsync(Guid settings)
    {
        // TODO rework this to load from a roster tree object insated of requiring the component
        // to sort the values. (see the loading of a component tree in the RoutingService).

        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

        var settingsObject = await _dbContext.RosterDisplaySettings
            .Where(x => x.Key == settings)
            .Include(x => x.HostRoster)
            .FirstOrDefaultAsync();

        if (settingsObject is null)
            return new(false, new List<string>() { "No settings object was found for the provided ID." });

        return new(true, null, settingsObject.HostRoster);
    }

    public async Task<ActionResult<List<RosterDisplaySettings>>> GetAvalibleRosterDisplaysAsync()
    {
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

        var rosterDisplays = await _dbContext
            .RosterDisplaySettings
            .ToListAsync();

        if(rosterDisplays is null)
            return new(false, new List<string> { "Unable to get a roster display settings list."}, null);

        return new(true, null, rosterDisplays);
    }

	public async Task<ActionResult<List<RosterTree>>> GetOrphanedRosterTreesAsync()
	{
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

        var trees = await _dbContext
            .RosterTrees
            .Include(x => x.ParentRosters)
            .Include(x => x.DisplaySettings)
            .Where(x => x.ParentRosters.Count <= 0)
            .Where(x => x.DisplaySettings.Count <= 0)
            .ToListAsync();

        if (trees is null)
            return new(false, new List<string> { "Unable to get a roster tree list." });

        return new(true, null, trees);
	}

	public async Task<ActionResult<List<RosterTree>>> GetTopLevelRosterTreesAsync()
	{
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

        var trees = await _dbContext
            .RosterTrees
            .Include(x => x.ParentRosters)
            .Where(x => x.ParentRosters.Count <= 0)
            .ToListAsync();

        if (trees is null)
            return new(false, new List<string> { "Unable to get a roster tree list." });

        return new(true, null, trees);
    }

	public async Task<ActionResult<List<RosterTree>>> GetAllRosterTreesAsync()
	{
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

        var trees = await _dbContext.RosterTrees.ToListAsync();

        if (trees is null)
            return new(false, new List<string> { "Unable to get a roster tree list." });

        return new(true, null, trees);
    }

    public async Task<ActionResult<RosterTree>> GetRosterTreeByIdAsync(Guid tree)
    {
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

        var treeObject = await _dbContext.RosterTrees
            .Where(x => x.Key == tree)
            .FirstOrDefaultAsync();

        if (treeObject is null)
            return new(false, new List<string>() { "No roster tree object was found for the provided ID." });

        return new(true, null, treeObject);
    }
    #endregion
}

/// <summary>
/// Exception for when a roster tree can not be loaded.
/// </summary>
public class MissingRosterTreeException : Exception
{
    public MissingRosterTreeException() { }
    public MissingRosterTreeException(string? message) : base(message) { }
    public MissingRosterTreeException(string? message, Exception? innerException) : base(message, innerException) { }
    protected MissingRosterTreeException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}