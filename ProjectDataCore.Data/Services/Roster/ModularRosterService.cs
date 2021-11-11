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

            tree.RosterParentLinks.Add(new()
            {
                ParentRosterId = parent.Key,
                Order = parent.ChildRosters.Count
            });
        }

        await _dbContext.AddAsync(tree);
        await _dbContext.SaveChangesAsync();

        if (parentTree is not null)
        {
            await tree.MovePositionAsync(_dbContext, parentTree.Value, position);
            await _dbContext.SaveChangesAsync();
        }

        return new(true, null);
    }

    public async Task<ActionResult> UpdateRosterTreeAsync(Guid tree, Action<RosterTreeEditModel> action)
    {
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

        var treeObject = await _dbContext.FindAsync<RosterTree>(tree);

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
                if(!await _dbContext.RosterParentLinks
                    .Where(x => x.ChildRosertId == treeObject.Key)
                    .Where(x => x.ParentRosterId == parent.Key).AnyAsync())
                {
                    // ... and that it doesn't exist in the inverse ...
                     if(!await _dbContext.RosterParentLinks
                        .Where(x => x.ChildRosertId == parent.Key)
                        .Where(x => x.ParentRosterId == treeObject.Key).AnyAsync())
                    {
                        // ... then add the new link ...
                        treeObject.RosterParentLinks.Add(new()
                        {
                            ParentRosterId = parent.Key,
                            Order = parent.ChildRosters.Count
                        });
                    }
                }
            }
        }
        // ... if there are parents to remove ...
        if(update.RemoveParents is not null)
        {
            // ... for each guid to remove ...
            foreach(var guid in update.RemoveParents)
            {
                // ... find the link ...
                var link = await _dbContext.RosterParentLinks
                    .Where(x => x.ChildRosertId == treeObject.Key)
                    .Where(x => x.ParentRosterId == guid)
                    .FirstOrDefaultAsync();
                // ... skip and invalid links ...
                if(link is null) continue;
                // ... and remove the link ...
                _dbContext.Remove(link);
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

        childTree.RosterParentLinks.Add(new()
        {
            ParentRosterId = tree,
            Order = -1
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

        var link = _dbContext.RosterParentLinks
            .Where(x => x.ParentRosterId == tree)
            .Where(x => x.ChildRosertId == child)
            .FirstOrDefaultAsync();

        if (link is null)
            return new(false, new List<string> { "No link was found for the provided IDs" });

        _dbContext.Remove(link);
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
            .Include(x => x.RosterPositions)
            .FirstOrDefaultAsync();

        if (rosterTree is null)
            return new(false, new List<string> { "No parent tree was found to add a slot to." });

        var slot = new RosterSlot()
        {
            Name = name,
            RosterParent = new()
            {
                Order = rosterTree.RosterPositions.Count,
                ParentRosterId = rosterTree.Key
            }
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
            slotData.RosterParent = new()
            {
                ParentRosterId = parentTree.Key,
                Order = parentTree.RosterPositions.Count
            };
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
        await _dbContext.SaveChangesAsync();

        await rosterSlot.MovePositionAsync(_dbContext);

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
    public async IAsyncEnumerable<RosterTree> GetRosterTreeForSettingsAsync(Guid settings)
    {
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

        var settingsObject = await _dbContext.RosterDisplaySettings
            .Where(x => x.Key == settings)
            .Include(x => x.HostRoster)
            .FirstOrDefaultAsync();

        if (settingsObject is null)
            throw new ArgumentNullException(nameof(settings), 
                $"ID does not yield a valid settings object.");

        await foreach (var res in LoadRosterTreeAsync(settingsObject.HostRoster, _dbContext))
            yield return res;
    }

    private async IAsyncEnumerable<RosterTree> LoadRosterTreeAsync(RosterTree parent, 
        ApplicationDbContext _dbContext)
    {
        await _dbContext.Entry(parent).Collection(x => x.RosterPositions).LoadAsync();

        yield return parent;

        await _dbContext.Entry(parent).Collection(x => x.ChildRosters).LoadAsync();

        foreach (var child in parent.ChildRosters)
            await foreach (var x in LoadRosterTreeAsync(child, _dbContext))
                yield return x;
    }

    public async Task<ActionResult<List<RosterDisplaySettings>>> GetAvalibleRosterDisplays()
    {
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

        var rosterDisplays = await _dbContext
            .RosterDisplaySettings
            .ToListAsync();

        if(rosterDisplays is null)
            return new(false, new List<string> { "Unable to get a roster display settings list."}, null);

        return new(true, null, rosterDisplays);
    }
    #endregion
}
