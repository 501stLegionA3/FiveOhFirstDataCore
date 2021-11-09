namespace ProjectDataCore.Data.Services;

public class ModularRosterService : IModularRosterService
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

    public ModularRosterService(IDbContextFactory<ApplicationDbContext> dbContextFactory)
        => (_dbContextFactory) = (dbContextFactory);

    #region Roster Tree
    public async Task<ActionResult> AddRosterTreeAsync(string name, Guid? parentTree = null)
    {
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

        var tree = new RosterTree()
        {
            RosterName = name
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

        return new(true, null);
    }

    public async Task<ActionResult> UpdateRosterTreeAsync(Guid tree, Action<RosterTreeEditModel> action)
    {
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

        var treeObject = await _dbContext.FindAsync<RosterTree>(tree);

        if (treeObject is null)
            return new(false, new List<string> { "No Roster Tree object was able to be found for the provided ID." });
        
        // TODO: Implement Roster tree edit model action for updates.
        //if(newName is not null)
        //    treeObject.RosterName = newName;

        //if(newParent is not null)
        //    treeObject.ParentRosterId = newParent.Value;

        await _dbContext.SaveChangesAsync();

        return new(true, null);
    }

    public async Task<ActionResult> AddChildRosterAsync(Guid tree, Guid child, int position)
    {
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

        var parentTree = await _dbContext.RosterTrees
            .Where(x => x.Key == tree)
            .Include(x => x.ChildRosters)
            .FirstOrDefaultAsync();

        if (parentTree is null)
            return new(false, new List<string> { "No parent roster tree was found." });

        var childTree = await _dbContext.FindAsync<RosterTree>(child);

        if (childTree is null)
            return new(false, new List<string> { "No child roster tree was found." });

        childTree.ParentRosterId = tree;
        parentTree.ChildRosters.ForEach(x =>
        {
            if (x.Order > position)
                x.Order++;
        });
        childTree.Order = position;

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

    public async Task<ActionResult> UpdateChildRosterPositionAsync(Guid tree, Guid child, int newPosition)
    {
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

        var parentTree = await _dbContext.RosterTrees
            .Where(x => x.Key == tree)
            .Include(x => x.ChildRosters)
            .FirstOrDefaultAsync();

        if (parentTree is null)
            return new(false, new List<string> { "No parent roster tree was found." });

        // Get an ordered lists of the child values.

        var order = parentTree.ChildRosters
            .OrderBy(x => x.Order)
            .ToList();

        // Find the object we are editing.
        var childTree = order.Find(x => x.Key == child);

        if(childTree is null)
            return new(false, new List<string> { "No child roster tree was found." });

        // Remove it and re-insert it into the list at
        // its new position.
        order.Remove(childTree);
        order.Insert(newPosition, childTree);

        // Reassign order values.
        for(int i = 0; i < order.Count; i++)
        {
            order[i].Order = i;
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
    public async Task<ActionResult> AddRosterSlotAsync(string name, Guid parentTree)
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
            PositionName = name,
            ParentTreeId = parentTree,
            Order = rosterTree.RosterPositions.Count
        };

        await _dbContext.AddAsync(slot);
        await _dbContext.SaveChangesAsync();

        return new(true, null);
    }

    public async Task<ActionResult> UpdateRosterSlotAsync(Guid slot, Action<RosterSlotEditModel> action)
    {
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

        var slotData = await _dbContext.FindAsync<RosterSlot>(slot);

        if (slotData is null)
            return new(false, new List<string> { "No slot found for the provided ID." });

        RosterSlotEditModel update = new();
        action.Invoke(update);

        if(update.UserId.HasValue)
            slotData.OccupiedById = update.UserId.Value;

        if (update.PositionName is not null)
            slotData.PositionName = update.PositionName;

        if (update.ParentTreeId is not null)
        {
            // TOOD: Implement parent change for roster slot.
        }

        await _dbContext.SaveChangesAsync();

        return new(true, null);
    }

    public async Task<ActionResult> UpdateRosterSlotPositionAsync(Guid parentTree, Guid slot, int newPosition)
    {
        throw new NotImplementedException();
    }

    public async Task<ActionResult> RemoveRosterSlotAsync(Guid slot)
    {
        throw new NotImplementedException();
    }
    #endregion

    #region Roster Display Settings
    public async Task<ActionResult> AddRosterDisplaySettingsAsync(string name, bool whitelisted)
    {
        throw new NotImplementedException();
    }

    public async Task<ActionResult> UpdateRosterDisplaySettingsAsync(Guid settings, bool? whitelisted = null)
    {
        throw new NotImplementedException();
    }

    public async Task<ActionResult> AddTreeToDisplaySettingsAsync(Guid settings, Guid tree)
    {
        throw new NotImplementedException();
    }

    public async Task<ActionResult> RemoveTreeFromDisplaySettingsAsync(Guid settings, Guid tree)
    {
        throw new NotImplementedException();
    }

    public async Task<ActionResult> RemoveRosterDisplaySettingsAsync(Guid settings)
    {
        throw new NotImplementedException();
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
