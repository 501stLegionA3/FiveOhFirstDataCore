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
    public MouseEventArgs EventArgs { get; set; }
    [Parameter]
    public RenderFragment Content { get; set; }
}
