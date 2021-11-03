using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Assignable;

internal class SingleValueAssignable
{
    public List<SingleValueAssignableData> Assignables { get; set; } = new();
    public string Name { get; set; }
    public bool AreSiteRoles { get; set; }
}
