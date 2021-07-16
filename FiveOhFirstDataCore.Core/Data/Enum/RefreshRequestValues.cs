using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Data
{
    [Flags]
    public enum RefreshRequestValues : long
    {
        // User Details
        UserDetails_NickName = 0x0000001,

        UserDetails = UserDetails_NickName,

        
    }
}
