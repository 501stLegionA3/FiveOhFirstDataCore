using ProjectDataCore.Data.Account;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Policy;
public class DynamicAuthorizationPolicy : DataObject<Guid>
{
    public List<RosterSlot> AuthorizedSlots { get; set; } = new();
    public List<RosterTree> AuthorizedTrees { get; set; } = new();
    public List<RosterDisplaySettings> AuthorizedDisplays { get; set; } = new();
    public List<DataCoreUser> AuthorizedUsers { get; set; } = new();
}
