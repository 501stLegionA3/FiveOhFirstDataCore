using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Model.Page;

public class RosterComponentSettingsEditModel
{
    public bool? Scoped { get; set; }

    public bool? AllowUserListing { get; set; }

    public List<DataCoreUserProperty>? UserListDisplayedProperties { get; set; }
    public List<DataCoreUserProperty>? DefaultDisplayedProperties { get; set; }

    public int? LevelFromTop { get; set; }
    public int? Depth { get; set; }

    public List<Guid>? AvalibleRosters { get; set; }
}
