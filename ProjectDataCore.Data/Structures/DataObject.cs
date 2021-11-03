using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures;

public abstract class DataObject<T> where T : unmanaged
{
    public T Key { get; set; }
    public DateTime LastEdit { get; set; }
}
