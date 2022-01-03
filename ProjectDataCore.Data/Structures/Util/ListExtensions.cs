using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Util;

public static class ListExtensions
{
    public static List<Out> ToList<T, Out>(this List<T> list, Func<T, Out> converter)
    {
        var res = new List<Out>();
        foreach (var item in list)
            res.Add(converter.Invoke(item));
        return res;
    }
}
