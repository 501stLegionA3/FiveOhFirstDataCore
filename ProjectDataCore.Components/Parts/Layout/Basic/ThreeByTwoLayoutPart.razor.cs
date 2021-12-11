using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Components.Parts.Layout.Basic;
public partial class ThreeByTwoLayoutPart
{
    [CascadingParameter(Name = "CoreRoute")]
    public string? Route { get; set; }
}
