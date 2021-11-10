namespace ProjectDataCore.Data.Services;

public class ModularRosterService : IModularRosterService
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

    public ModularRosterService(IDbContextFactory<ApplicationDbContext> dbContextFactory)
        => (_dbContextFactory) = (dbContextFactory);

    #region Roster Tree
    public async Task<ActionResult> AddRosterTreeAsync(string name, Guid? parentTree = null, int position = 0)
    {
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

        var tree = new RosterTree()
        {
            Name = name
        };

        if (parentTree is not null)
        {
            var parent = await _dbContext.RosterTrees
                .Where(x => x.Key == parentTree)
                .Include(x => x.ChildRosters)
                .FirstOrDefaultAsync();

            if (parent is null)
                return new(false, new List<string> { "No parent was found by the provided roster ID." });

            tree.ParentRosterId = parentTree.Value;
            tree.Order = parent.ChildRosters.Count;
        }

        await _dbContext.AddAsync(tree);
        await _dbContext.SaveChangesAsync();

        if (parentTree is not null)
        {
            await tree.MovePositionAsync(_dbContext, position);
            await _dbContext.SaveChangesAsync();
        }

        return new(true, null);
    }

    public async Task<ActionResult> UpdateRosterTreeAsync(Guid tree, Action<RosterObjectEditModel> action)
    {
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

        var treeObject = await _dbContext.FindAsync<RosterTree>(tree);

        if (treeObject is null)
            return new(false, new List<string> { "No Roster Tree object was able to be found for the provided ID." });

        RosterObjectEditModel update = new();
        action.Invoke(update);

        if(update.Name is not null)
            treeObject.Name = update.Name;

        // If the parent tree ID has a value...
        if (update.ParentRosterId.HasValue)
        {
            // ... and the value is not null ...
            if (update.ParentRosterId.Value is not null)
            {
                // ... attempt to get the parent tree object,
                // including all roster positions ...
                var parentTree = await _dbContext.RosterTrees
                    .Where(x => x.Key == update.ParentRosterId.Value.Value)
                    .Include(x => x.ChildRosters)
                    .FirstOrDefaultAsync();
                // ... if the parent tree is null, return a failure...
                if (parentTree is null)
                    return new(false, new List<string> { "No parent roster found to swtich to." });
                // ... otherwise update the slot with a new parent...
                treeObject.ParentRosterId = update.ParentRosterId.Value.Value;
                treeObject.Order = parentTree.ChildRosters.Count;
            }
            else
            {
                // There is no parent roster anymore, set
                // relationship to null.
                treeObject.ParentRosterId = null;
                treeObject.Order = 0;
            }
        }

        if (update.Order is not null)
            await treeObject.MovePositionAsync(_dbContext, update.Order.Value);

        await _dbContext.SaveChangesAsync();

        return new(true, null);
    }

    public async Task<ActionResult> AddChildRosterAsync(Guid tree, Guid child, int position)
    {
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

        var childTree = await _dbContext.FindAsync<RosterTree>(child);

        if (childTree is null)
            return new(false, new List<string> { "No child roster tree was found." });

        childTree.ParentRosterId = tree;
        childTree.Order = -1;

        await _dbContext.SaveChangesAsync();

        // Perform a reorder operation to insert at the correct spot.
        await childTree.MovePositionAsync(_dbContext, position);

        await _dbContext.SaveChangesAsync();

        return new(true, null);
    }

    public async Task<ActionResult> RemoveChildRosterAsync(Guid tree, Guid child)
    {
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

        var childTree = await _dbContext.FindAsync<RosterTree>(child);

        if (childTree is null)
            return new(false, new List<string> { "No child roster tree was found." });

        // Probably not going to work
        childTree.ParentRosterId = default;

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

        await rosterTree.MovePositionAsync(_dbContext);

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
            .Include(x => x.ChildRosters)
            .FirstOrDefaultAsync();

        if (rosterTree is null)
            return new(false, new List<string> { "No parent tree was found to add a slot to." });

        var slot = new RosterSlot()
        {
            Name = name,
            ParentRosterId = parentTree,
            Order = rosterTree.RosterPositions.Count
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
        if (update.Name is not null)
        {
            // ... attempt to get the parent tree object,
            // including all roster positions ...
            var parentTree = await _dbContext.RosterTrees
                .Where(x => x.Key == update.ParentRosterId.Value)
                .Include(x => x.RosterPositions)
                .FirstOrDefaultAsync();
            // ... if the parent tree is null, return a failure...
            if (parentTree is null)
                return new(false, new List<string> { "No parent roster found to swtich to." });
            // ... otherwise update the slot with a new parent...
            slotData.ParentRosterId = update.ParentRosterId.Value;
            slotData.Order = parentTree.RosterPositions.Count;
        }

        if (update.Order is not null)
            await slotData.MovePositionAsync(_dbContext, update.Order.Value);

        // ... then save changes.
        await _dbContext.SaveChangesAsync();

        return new(true, null);
    }

    public async Task<ActionResult> RemoveRosterSlotAsync(Guid slot)
    {
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

        var rosterSlot = await _dbContext.FindAsync<RosterSlot>(slot);

        if (rosterSlot is null)
            return new(false, new List<string> { "No roster slot was found." });

        _dbContext.Remove(rosterSlot);
        await _dbContext.SaveChangesAsync();

        await rosterSlot.MovePositionAsync(_dbContext);

        await _dbContext.SaveChangesAsync();

        return new(true, null);
    }
    #endregion

    #region Roster Display Settings
    public async Task<ActionResult> AddRosterDisplaySettingsAsync(string name, bool whitelisted)
    {
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

        var settings = new RosterDisplaySettings()
        {
            Name = name,
            Whitelist = whitelisted
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

        if(update.WhiteList is not null)
            settingsObject.Whitelist = update.WhiteList.Value;

        if(update.TreeKeys is not null)
            settingsObject.TreeKeys = update.TreeKeys;

        await _dbContext.SaveChangesAsync();

        return new(true, null);
    }

    public async Task<ActionResult> AddTreeToDisplaySettingsAsync(Guid settings, Guid tree)
    {
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

        var settingsObject = await _dbContext.FindAsync<RosterDisplaySettings>(settings);

        if (settingsObject is null)
            return new(false, new List<string> { "No settings object found for the provided ID" });

        settingsObject.TreeKeys.Add(tree);

        await _dbContext.SaveChangesAsync();

        return new(true, null);
    }

    public async Task<ActionResult> RemoveTreeFromDisplaySettingsAsync(Guid settings, Guid tree)
    {
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

        var settingsObject = await _dbContext.FindAsync<RosterDisplaySettings>(settings);

        if (settingsObject is null)
            return new(false, new List<string> { "No settings object found for the provided ID" });

        settingsObject.TreeKeys.Remove(tree);

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
    public IAsyncEnumerable<RosterTree> GetFullRosterAsync()
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<RosterTree> GetRosterTreeForSettingsAsync(Guid settings)
    {
        throw new NotImplementedException();
    }

    public async Task<ActionResult<List<RosterDisplaySettings>>> GetAvalibleRosterDisplays()
    {
        throw new NotImplementedException();
    }
    #endregion
}
