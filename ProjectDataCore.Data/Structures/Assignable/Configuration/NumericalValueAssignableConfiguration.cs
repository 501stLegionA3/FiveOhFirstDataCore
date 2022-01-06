using ProjectDataCore.Data.Structures.Assignable.Value;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Assignable.Configuration;

[AssignableConfiguration("Integer", typeof(IntegerAssignableValue))]
public class IntegerValueAssignableConfiguration : ValueBaseAssignableConfiguration<int>
{

}

[AssignableConfiguration("Decimal", typeof(DoubleAssignableValue))]
public class DoubleValueAssignableConfiguration : ValueBaseAssignableConfiguration<double>
{

}
