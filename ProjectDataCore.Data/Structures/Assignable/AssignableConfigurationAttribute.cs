using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Assignable;

[AttributeUsage(AttributeTargets.Class)]
public class AssignableConfigurationAttribute : Attribute
{
    public string Name { get; set; }

    public AssignableConfigurationAttribute(string name)
        => Name = name;
}
