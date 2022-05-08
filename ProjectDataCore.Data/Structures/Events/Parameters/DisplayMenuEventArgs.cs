using Microsoft.AspNetCore.Components;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Events.Parameters;
public class DisplayMenuEventArgs : EventArgs
{
    public RenderFragment Menu { get; set; }

    public DisplayMenuEventArgs(RenderFragment menu)
    {
        Menu = menu;
    }
}
