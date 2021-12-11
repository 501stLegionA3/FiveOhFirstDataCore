using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Components.Parts.Layout.Basic;

[LayoutComponent(Name = "1x2 Layout")]
public partial class OneByTwoLayoutPart
{
    [CascadingParameter(Name = "CoreRoute")]
    public string? Route { get; set; }

    [Parameter]
    public LayoutComponentSettings? ComponentData { get; set; }
}
