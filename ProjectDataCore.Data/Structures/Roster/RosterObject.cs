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
    public RosterTree ParentRoster { get; set; }
    public Guid? ParentRosterId { get; set; }
}
