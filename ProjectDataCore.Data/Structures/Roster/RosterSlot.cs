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
public class RosterSlot : RosterObject
{
    public DataCoreUser? OccupiedBy { get; set; }
    public int? OccupiedById { get; set; }
    public RosterParentLink RosterParent { get; set; }
    public Guid RosterParentId { get; set; }
}
