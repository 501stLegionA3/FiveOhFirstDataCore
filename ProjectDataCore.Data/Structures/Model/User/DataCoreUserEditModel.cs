using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Model.User;

public class DataCoreUserEditModel
{
    public ConcurrentDictionary<string, object?> StaticValues { get; set; } = new();
    public ConcurrentDictionary<string, object?> AssignableValues { get; set; } = new();

    public void ApplyStaticValues<T>(T obj) where T : class
    {
        var typ = typeof(T);
        foreach(var pair in StaticValues)
        {
            typ.GetProperty(pair.Key)?.SetValue(obj, pair.Value);
        }
    }
}
