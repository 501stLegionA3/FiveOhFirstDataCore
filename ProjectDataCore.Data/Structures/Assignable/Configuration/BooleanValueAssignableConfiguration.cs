using ProjectDataCore.Data.Structures.Assignable.Value;

namespace ProjectDataCore.Data.Structures.Assignable.Configuration;

[AssignableConfiguration("Boolean", typeof(BooleanAssignableValue))]
public class BooleanValueAssignableConfiguration : ValueBaseAssignableConfiguration<bool>
{
    
}