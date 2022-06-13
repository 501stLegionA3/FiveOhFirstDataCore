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
    public string ShortName { get; set; } = "";
    public bool Form { get; set; } = false;
    public string IconPath { get; set; } = "~/svg/mat-icons/add.svg";
    public Type ComponentSettingsType { get; set; } = typeof(PageComponentSettingsBase);

    public ComponentAttribute(Type componentSettingsType)
        : base() 
    {
        ComponentSettingsType = componentSettingsType;
    }
}

public class LayoutComponentAttribute : ComponentAttribute
{
    public LayoutComponentAttribute(Type componentSettingsType)
        : base(componentSettingsType) { }
}

public class EditableComponentAttribute : ComponentAttribute
{
    public EditableComponentAttribute(Type componentSettingsType)
        : base(componentSettingsType) { }
}

public class DisplayComponentAttribute : ComponentAttribute
{
    public DisplayComponentAttribute(Type componentSettingsType)
        : base(componentSettingsType) { }
}

public class RosterComponentAttribute : ComponentAttribute
{
    public RosterComponentAttribute(Type componentSettingsType)
        : base(componentSettingsType) { }
}

public class ButtonComponentAttribute : ComponentAttribute
{
    public ButtonComponentAttribute(Type componentSettingsType)
        : base(componentSettingsType) { }
}

public class TextDisplayComponentAttribute : ComponentAttribute
{
    public TextDisplayComponentAttribute(Type componentSettingsType)
        : base(componentSettingsType) { }
}