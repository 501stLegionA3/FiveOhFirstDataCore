using Microsoft.AspNetCore.Components.Web;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Events.Parameters;
/// <summary>
/// Provides X and Y pos coords relative to the screen and the raw mouse event data.
/// </summary>
public class PageClickedEventArgs : EventArgs
{
    public double XPos { get; set; }
    public double YPos { get; set; }
    public MouseEventArgs Raw { get; set; }

    public PageClickedEventArgs(MouseEventArgs args)
    {
        XPos = args.ScreenX;
        YPos = args.ScreenY;
        Raw = args;
    }
}
