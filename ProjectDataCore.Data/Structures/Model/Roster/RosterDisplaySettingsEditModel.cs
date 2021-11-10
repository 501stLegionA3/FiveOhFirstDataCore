using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Model.Roster;

public class RosterDisplaySettingsEditModel
{
    public string? Name { get; set; }
    public bool? WhiteList { get; set; }
    public List<Guid>? TreeKeys { get; set; }
}
