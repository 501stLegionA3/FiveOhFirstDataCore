using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Util;
public static class BooleanExtensions
{
    public static int ToInt32(this bool val)
        => Convert.ToInt32(val);
}
