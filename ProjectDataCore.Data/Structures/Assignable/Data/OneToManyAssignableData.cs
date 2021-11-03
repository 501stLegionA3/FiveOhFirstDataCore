using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Assignable;

internal class OneToManyAssignableData : DataObject<Guid>
{
    public string Primary { get; set; }
    public List<string> Secondaries { get; set; }
}
