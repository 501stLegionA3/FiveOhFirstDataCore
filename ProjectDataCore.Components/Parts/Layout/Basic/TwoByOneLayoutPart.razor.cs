using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Components.Parts.Layout.Standard;
public partial class TwoByOneLayoutPart
{
    [CascadingParameter(Name = "CoreRoute")]
    public string? Route { get; set; }
}
