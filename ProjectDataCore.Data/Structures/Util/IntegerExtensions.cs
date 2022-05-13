using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Util;
public static class IntegerExtensions
{

    // Thanks MSDN https://docs.microsoft.com/en-us/dotnet/api/system.object.gethashcode?view=net-6.0
    public static int ShiftAndWrap(this int value, int positions)
    {
        positions &= 0x1F;

        // Save the existing bit pattern, but interpret it as an unsigned integer.
        uint number = BitConverter.ToUInt32(BitConverter.GetBytes(value), 0);
        // Preserve the bits to be discarded.
        uint wrapped = number >> (32 - positions);
        // Shift and wrap the discarded bits.
        return BitConverter.ToInt32(BitConverter.GetBytes((number << positions) | wrapped), 0);
    }

    public static int ShiftAndWrap(this char value, int positions)
        => ShiftAndWrap((int)value, positions);
}
