using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Roster;

public abstract class RosterObject : DataObject<Guid>
{
    public int Order { get; set; }
    public string Name { get; set; }
    public RosterTree? ParentRoster { get; set; }
    public Guid? ParentRosterId { get; set; }
}

public static class RosterObjectExtenstions
{
    public static async Task MovePositionAsync(this RosterObject toMove, 
        ApplicationDbContext _dbContext, int newIndex = 0)
    {
        if (toMove.ParentRosterId is null)
            throw new ArgumentNullException(nameof(toMove), 
                $"The {toMove.GetType()} can not have the value null " +
                $"for the {nameof(toMove.ParentRosterId)} parameter.");

        var parent = await _dbContext.RosterTrees
            .Where(x => toMove.ParentRosterId.Value == x.Key)
            .Include(x => x.ChildRosters)
            .FirstOrDefaultAsync();

        if (parent is null)
            throw new ArgumentException($"The {toMove.GetType()} can not find " +
                $"a value in the database for the {nameof(toMove.ParentRosterId)} " +
                $"parameter.", nameof(toMove));

        // Get an ordered lists of the child values.

        var order = parent.ChildRosters
            .OrderBy(x => x.Order)
            .ToList();

        // Find the object we are editing.
        var childTree = order.Find(x => x.Key == toMove.Key);

        // If the child tree is not null...
        if (childTree is not null)
        {
            // ...Remove it and re-insert it into the list at
            // its new position
            order.Remove(childTree);
            order.Insert(newIndex, childTree);
        }

        // ... otherwise we are just consolidating
        // a gap in the order.

        // Reassign order values.
        for (int i = 0; i < order.Count; i++)
        {
            order[i].Order = i;
        }
    }
}
