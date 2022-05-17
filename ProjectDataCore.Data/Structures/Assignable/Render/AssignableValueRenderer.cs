using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Assignable.Render;
public class AssignableValueRenderer : DataObject<Guid>
{
    public string PropertyName { get; set; }
    public bool Static { get; set; }

    
}
