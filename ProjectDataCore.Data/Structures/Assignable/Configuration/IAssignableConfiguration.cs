using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Assignable.Configuration;

public interface IAssignableConfiguration<T> : IAssignableConfiguration
{
    public List<T> AllowedValues { get; set; }
}

public interface IAssignableConfiguration
{
    public void MoveElement(int currentIndex, int newIndex);
    public void AddElement(object o);
    public void RemoveElement(int index);
}