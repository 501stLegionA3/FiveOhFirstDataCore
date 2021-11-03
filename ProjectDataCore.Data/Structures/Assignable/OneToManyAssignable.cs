using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Assignable;

public class OneToManyAssignable : DataObject<Guid>
{
    public List<OneToManyAssignable> Assignables { get; set; } = new();
    public string Name { get; set; }
    public bool AreSiteClaims { get; set; }
}
