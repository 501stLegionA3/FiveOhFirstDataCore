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
    public bool Form { get; set; } = false;

    public ComponentAttribute()
        : base() { }
}

public class LayoutComponentAttribute : ComponentAttribute { }
public class EditableComponentAttribute : ComponentAttribute { }
public class DisplayComponentAttribute : ComponentAttribute { }
public class RosterComponentAttribute : ComponentAttribute { }
public class ButtonComponentAttribute : ComponentAttribute { }
public class TextDisplayComponentAttribute : ComponentAttribute { }