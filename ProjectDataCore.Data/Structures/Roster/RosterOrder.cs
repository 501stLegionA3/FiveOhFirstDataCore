using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Roster;

public class RosterOrder : DataObject<Guid>
{
    /// <summary>
    /// The child object.
    /// </summary>
    public RosterTree? TreeToOrder { get; set; }
    public Guid? TreeToOrderId { get; set; }

    public RosterSlot? SlotToOrder { get; set; }
    public Guid? SlotToOrderId { get; set; }

    /// <summary>
    /// The parent object that controls the order for its children.
    /// </summary>
    public RosterTree ParentObject { get; set; }
    public Guid ParentObjectId { get; set; }

    public int Order { get; set; } = 0;
}
