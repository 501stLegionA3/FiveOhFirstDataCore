using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Model.Roster;

public class RosterTreeEditModel
{
    public string? RosterName { get; set; }
    public Optional<Guid?> ParentRosterId { get; set; }
}
