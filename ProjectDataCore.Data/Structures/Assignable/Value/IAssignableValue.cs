using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Assignable.Value;

public interface IAssignableValue<T>
{
    public List<T> SetValue { get; set; }
}
