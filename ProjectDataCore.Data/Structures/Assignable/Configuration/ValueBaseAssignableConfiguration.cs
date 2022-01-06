using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Assignable.Configuration;

public class ValueBaseAssignableConfiguration<T> : BaseAssignableConfiguration, IAssignableConfiguration<T>
{
    public List<T> AllowedValues { get; set; } = new();

    public void AddElement(object o)
    {
        AllowedValues.Add((T)o);
    }

    public void MoveElement(int currentIndex, int newIndex)
    {
        var cur = AllowedValues[currentIndex];
        AllowedValues.RemoveAt(currentIndex);
        AllowedValues.Insert(newIndex, cur);
    }

    public void RemoveElement(int index)
    {
        AllowedValues.RemoveAt(index);
    }
}
