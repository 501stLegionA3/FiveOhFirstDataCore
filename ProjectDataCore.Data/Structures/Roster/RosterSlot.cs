using ProjectDataCore.Data.Account;
using ProjectDataCore.Data.Structures.Policy;

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
    public Guid? OccupiedById { get; set; }
    public RosterOrder Order { get; set; }
    public RosterTree ParentRoster { get; set; }
    public Guid ParentRosterId { get; set; }

    public List<DynamicAuthorizationPolicy> DynamicPolicies { get; set; } = new();
}
