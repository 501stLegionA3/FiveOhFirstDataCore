using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Roster;

public abstract class RosterObject : DataObject<Guid>
{
    public string Name { get; set; }
}

public static class RosterObjectExtenstions
{
    public static async Task MovePositionAsync(this RosterTree toMove, 
        ApplicationDbContext _dbContext, Guid? parent = null, int newIndex = 0)
    {
        // See if we have a child value...
        var dumpCheck = (await _dbContext.RosterParentLinks
            .Where(x => x.ChildRosertId == toMove.Key)
            .CountAsync()) <= 0;
        // ... then start the query, filtering by a child value
        // if we have one...
        IQueryable<RosterParentLink> linksQuery;
        if (dumpCheck)
            linksQuery = _dbContext.RosterParentLinks;
        else
            linksQuery = _dbContext.RosterParentLinks
                .Where(x => x.ChildRosertId == toMove.Key);
        // ... and then filter by a parent value if
        // we have one ...
        if(parent is not null)
            linksQuery = linksQuery.Where(x => x.ParentRosterId == parent.Value);
        // ... then get the enumerable ...
        var links = linksQuery
            .Include(x => x.ParentRoster)
            .ThenInclude(x => x.ChildRosters)
            .ThenInclude(x => x.RosterParentLinks)
            .AsAsyncEnumerable();
        // ... setup a check so we don't repeat
        // on specifc items ...
        HashSet<Guid> filteredParents = new();
        // ... then for each link in the query ...
        await foreach(var link in links)
        {
            // ... check if the parent has already been computed,
            // skip this link if it has ...
            if (filteredParents.Contains(link.ParentRosterId))
                continue;
            // ... get the ordered child rosters
            // of the parent ...
            var set = link.ParentRoster.ChildRosters
                .OrderBy(x => x.RosterParentLinks.Count)
                .ToList();
            // ... if there is a child object ...
            if(!dumpCheck)
            {
                // ... find the actual item ...
                var item = set.FirstOrDefault(x => x.Key == toMove.Key);

                if (item is null)
                    continue;
                // ... and move it to its new position ...
                set.Remove(item);
                set.Insert(newIndex, item);
            }
            // ... then for every child in the parent ...
            for (int i = 0; i < set.Count; i++)
            {
                // ... get the actual link object ...
                var vals = set[i].RosterParentLinks
                    .Where(x => x.ParentRosterId == link.ParentRosterId);
                // ... and set the order value ...
                foreach (var v in vals)
                    v.Order = i;
            }
            // ... then add the parent key to the repat check.
            filteredParents.Add(link.ParentRosterId);
        }
    }

    public static async Task MovePositionAsync(this RosterSlot toMove,
        ApplicationDbContext _dbContext, int newIndex = 0)
    {
        // See if we have a child value...
        var dumpCheck = (await _dbContext.RosterParentLinks
            .Where(x => x.ChildRosertId == toMove.Key)
            .CountAsync()) <= 0;
        // ... then start the query, filtering by a child value
        // if we have one...
        IQueryable<RosterParentLink> linksQuery;
        if (dumpCheck)
            linksQuery = _dbContext.RosterParentLinks;
        else
            linksQuery = _dbContext.RosterParentLinks
                .Where(x => x.ChildRosertId == toMove.Key);
        // ... then get the enumerable ...
        var links = linksQuery
            .Where(x => x.ParentRosterId == toMove.RosterParentId)
            .Include(x => x.ParentRoster)
            .ThenInclude(x => x.RosterPositions)
            .ThenInclude(x => x.RosterParent)
            .AsAsyncEnumerable();
        // ... setup a check so we don't repeat
        // on specifc items ...
        HashSet<Guid> filteredParents = new();
        // ... then for each link in the query ...
        await foreach (var link in links)
        {
            // ... check if the parent has already been computed,
            // skip this link if it has ...
            if (filteredParents.Contains(link.ParentRosterId))
                continue;
            // ... get the ordered child rosters
            // of the parent ...
            var set = link.ParentRoster.RosterPositions
                .OrderBy(x => x.RosterParent.Order)
                .ToList();
            // ... if there is a child object ...
            if (!dumpCheck)
            {
                // ... find the actual item ...
                var item = set.FirstOrDefault(x => x.Key == toMove.Key);

                if (item is null)
                    continue;
                // ... and move it to its new position ...
                set.Remove(item);
                set.Insert(newIndex, item);
            }
            // ... then for every child in the parent ...
            for (int i = 0; i < set.Count; i++)
            {
                // ... set the order value...
                set[i].RosterParent.Order = i;
            }
            // ... then add the parent key to the repat check.
            filteredParents.Add(link.ParentRosterId);
        }
    }
}
