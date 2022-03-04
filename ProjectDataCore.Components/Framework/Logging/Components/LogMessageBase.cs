using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Components.Framework.Logging.Components;

public class LogMessageBase : ComponentBase
{
    [Parameter]
    public string Message { get; set; }
    [Parameter]
    public DateTime Date { get; set; }
    [Parameter]
    public int Depth { get; set; } = 0;
}
