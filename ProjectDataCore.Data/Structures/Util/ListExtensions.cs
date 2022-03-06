using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Util;

public static class ListExtensions
{
    public static List<TOut> ToList<T, TOut>(this IEnumerable<T> list, Func<T, TOut> converter)
    {
        var res = new List<TOut>();
        foreach (var item in list)
            res.Add(converter.Invoke(item));
        return res;
    }

    public static TOut[] ToArray<T, TOut> (this IEnumerable<T> list, Func<T, TOut> converter)
    {
        var res = (TOut[])Array.CreateInstance(typeof(TOut), list.Count());
        for(int i = 0; i < res.Length; i++)
            res[i] = converter.Invoke(list.ElementAt(i));
        return res;
    }
}
