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
    public Type Configures { get; set; }

    public AssignableConfigurationAttribute(string name, Type configures)
        => (Name, Configures) = (name, configures);
}
