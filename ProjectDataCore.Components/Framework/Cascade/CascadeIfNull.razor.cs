using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Components.Framework.Cascade;
public partial class CascadeIfNull<TValue>
{
    [Parameter]
    public TValue Value { get; set; }

    [CascadingParameter]
    public TValue? Cascaded { get; set; }

    [Parameter]
    public RenderFragment Content { get; set; }
}
