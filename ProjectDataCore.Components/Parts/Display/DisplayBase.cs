using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Components.Parts.Display;

public class DisplayBase : CustomComponentBase
{
    [Parameter]
    public DisplayComponentSettings? ComponentData { get; set; }
}
