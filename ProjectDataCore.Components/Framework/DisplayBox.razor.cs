using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Components.Framework;
public partial class DisplayBox
{
    [Parameter]
    public RenderFragment Contnet { get; set; }
}
