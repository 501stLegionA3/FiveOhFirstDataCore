using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Page.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class ComponentAttribute : Attribute
{
    public string Name { get; set; } = "";

    public ComponentAttribute()
        : base() { }
}

public class LayoutComponentAttribute : ComponentAttribute { }
public class EditableComponentAttribute : ComponentAttribute { }
public class DisplayComponentAttribute : ComponentAttribute { }