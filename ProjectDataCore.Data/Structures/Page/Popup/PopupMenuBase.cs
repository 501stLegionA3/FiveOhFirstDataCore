using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Page.ContextMenu;
public class PopupMenuBase : ComponentBase
{
    [Parameter]
    public RenderFragment Popup { get; set; }
    [Parameter]
    public RenderFragment Element { get; set; }
    [Parameter]
    public string Class { get; set; }
}
