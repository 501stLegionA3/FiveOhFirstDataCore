using ProjectDataCore.Data.Structures.Page.Components.Layout;
using ProjectDataCore.Data.Structures.Page.Components.Parameters;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Components.Framework.Page.Components.Display;
public class DisplayBase : CustomComponentBase
{
    [Parameter]
    public DisplayComponentSettings? ComponentData { get; set; }
    [Parameter]
    public LayoutNode? Node { get; set; }
}
