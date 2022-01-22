using ProjectDataCore.Data.Structures.Nav;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Components.Nav
{
    public partial class EditModuleDisplay
    {
        [Parameter] public NavModule Module { get; set; }
        [Parameter] public Action<NavModule> OnDblClick { get; set; }
        [Parameter] public Action<NavModule> OnLeftClick { get; set; }
    }
}
