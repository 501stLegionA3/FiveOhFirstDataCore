using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Components.Parts.Layout.Basic;

[LayoutComponent(Name = "3x3 Layout")]
public partial class ThreeByThreeLayoutPart
{
    [CascadingParameter(Name = "CoreRoute")]
    public string? Route { get; set; }

    [Parameter]
    public LayoutComponentSettings? ComponentData { get; set; }
}
