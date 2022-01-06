using ProjectDataCore.Data.Structures.Assignable.Value;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Assignable.Configuration;

[AssignableConfiguration("String", typeof(StringAssignableValue))]
public class StringValueAssignableConfiguration : ValueBaseAssignableConfiguration<string>
{

}
