using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Components.Parts;

public class CustomComponentBase : ComponentBase
{
    [CascadingParameter(Name = "CoreRoute")]
    public string? Route { get; set; }
    [CascadingParameter(Name = "PageEdit")]
    public bool Editing { get; set; }
}
