using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Extensions
{
    public static class ObjectArrayExtensions
    {
        public static T? GetArgument<T>(this object[] args, int pos)
            => (T)Convert.ChangeType(args[pos], typeof(T));
    }
}
