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
        ApplicationDbContext _dbContext, Guid parent, int newIndex = 0)
    {
        // Load the parent to move ...
        var tracker = _dbContext.Entry(toMove);

        // We can't do anything if this object is not in the database yet.
        if (tracker is null)
            return;

        // ... load the parent value ...
        await tracker.Collection(x => x.ParentRosters)
            .Query()
            .Where(x => x.Key == parent)
            .Include(x => x.ChildRosters)
            .ThenInclude(x => x.Order)
            .LoadAsync();

        // ... then get the parent object ...
        var parentData = toMove.ParentRosters.FirstOrDefault();

        // ... no parent means no movement possible ...
        if (parentData is null)
            return;

        // ... otherwise, shift through the order values and
        // set the new indexies ...

        // ... first, order the child objects ...
        var orderedChildren = parentData.ChildRosters.OrderBy(x => 
            x.Order.FirstOrDefault(x => 
                x.ParentObjectId == parentData.Key)?.Order ?? 0);

        // ... then change the indexies as needed ...
        int index = 0;
        foreach(var child in orderedChildren)
        {
            // ... skip over the new index value ...
            if (index == newIndex)
                index++;
            // ... get the order object for this child/parent pair ...
            var orderObject = child.Order.FirstOrDefault(x => x.ParentObjectId == parentData.Key);
            if (orderObject is null)
            {
                // ... or create a new order object if needed ...
                orderObject = new()
                {
                    ParentObjectId = parentData.Key,
                    TreeToOrderId = child.Key
                };
                child.Order.Add(orderObject);
            }
            // ... then set the new order key ...
            if (child.Key == toMove.Key)
            {
                orderObject.Order = newIndex;
            }
            else
            {
                orderObject.Order = index++;
            }
        }
    }

    public static async Task MovePositionAsync(this RosterSlot toMove,
        ApplicationDbContext _dbContext, int newIndex = 0)
    {
        // Load the parent to move ...
        var tracker = _dbContext.Entry(toMove);

        // We can't do anything if this object is not in the database yet.
        if (tracker is null)
            return;

        // ... load the parent value ...
        await tracker.Reference(x => x.ParentRoster)
            .Query()
            .Include(x => x.RosterPositions)
            .LoadAsync();

        // ... then get the parent object ...
        var parentData = toMove.ParentRoster;

        // ... no parent means no movement possible ...
        if (parentData is null)
            return;

        // ... otherwise, shift through the order values and
        // set the new indexies ...

        // ... first, order the child objects ...
        var orderedChildren = parentData.RosterPositions.OrderBy(x =>
            x.Order.Order);

        // ... then change the indexies as needed ...
        int index = 0;
        foreach (var child in orderedChildren)
        {
            // ... skip over the new index value ...
            if (index == newIndex)
                index++;
            // ... get the order object for this child/parent pair ...
            var orderObject = child.Order;
            if (orderObject is null)
            {
                // ... or create a new order object if needed ...
                orderObject = new()
                {
                    ParentObjectId = parentData.Key,
                    SlotToOrderId = child.Key
                };
                child.Order = orderObject;
            }
            // ... then set the new order key ...
            if (child.Key == toMove.Key)
            {
                orderObject.Order = newIndex;
            }
            else
            {
                orderObject.Order = index++;
            }
        }
    }
}
