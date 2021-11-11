using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Model.Roster;

public class RosterTreeEditModel : RosterObjectEditModel
{
    public (Guid, int)? Order { get; set; }
    public List<Guid>? RemoveParents { get; set; }
    public List<Guid>? AddParents { get; set; }
}
