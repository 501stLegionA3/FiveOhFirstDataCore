using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Roster;

public class RosterParentLink : DataObject<Guid>
{
    public int Order { get; set; }
    public RosterTree ParentRoster { get; set; }
    public Guid ParentRosterId { get; set; }
    public RosterObject ChildRoster { get; set; }
    public Guid ChildRosertId { get; set; }
    public RosterDisplaySettings ForRosterSettings { get; set; }
    public Guid ForRosterSettingsId { get; set; }
}
