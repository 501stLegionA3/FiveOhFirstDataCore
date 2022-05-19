using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Assignable.Render;
public class AssignableValueRenderData
{
    public string Composite 
    {
        get
        {
            if (Order is null)
                // Should return {{Name}}
                return $"{{{{{Name}}}}}";

            // Should return {{Name:#}}
            return $"{{{{{Name}:{Order}}}}}";
        }
    }
    public string Name { get; set; }
    public int? Order { get; set; }
    public string Value { get; set; }

    public AssignableValueRenderData(string name, int? order, string value)
    {
        Name = name;
        Order = order;
        Value = value;
    }
}
