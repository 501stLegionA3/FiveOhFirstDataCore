using ProjectDataCore.Data.Account;
using ProjectDataCore.Data.Structures.Policy;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Model.Policy;
public class DynamicAuthorizationPolicyEditModel
{
    public string? PolicyName { get; set; } = null;

    public List<RosterSlot>? AuthorizedSlots { get; set; } = null;
    public List<RosterTree>? AuthorizedTrees { get; set; } = null;
    public List<RosterDisplaySettings>? AuthorizedDisplays { get; set; } = null;
    public List<DataCoreUser>? AuthorizedUsers { get; set; } = null;

    public List<DynamicAuthorizationPolicy>? Parents { get; set; } = null;
}
