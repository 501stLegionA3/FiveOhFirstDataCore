using ProjectDataCore.Data.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Roster;

/// <summary>
/// A position on the roster.
/// </summary>
public class RosterSlot : DataObject<Guid>
{
    public string PositionName { get; set; }
    public DataCoreUser OccupiedBy { get; set; }
    public RosterTree ParentTree { get; set; }
    public Guid RosterTreeId { get; set; }
}
