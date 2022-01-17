using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Util;

public static class DictionaryExtensions
{
    public static Dictionary<TKey, TValue> ShallowCopy<TKey, TValue>(this Dictionary<TKey, TValue> source) where TKey : notnull
    {
        var copy = new Dictionary<TKey, TValue>();
        foreach(var pair in source)
        {
            copy.Add(pair.Key, pair.Value);
        }

        return copy;
    }
}
