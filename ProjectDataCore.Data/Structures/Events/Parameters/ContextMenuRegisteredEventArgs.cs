using Microsoft.AspNetCore.Components;

using ProjectDataCore.Data.Structures.Page.ContextMenu;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Events.Parameters;
public class ContextMenuRegisteredEventArgs : EventArgs
{
    public Type MenuType { get; set; }
    public RenderFragment ContextMenu { get; init; }

    public ContextMenuRegisteredEventArgs(Type menuType, RenderFragment fragment)
    {
        MenuType = menuType;
        ContextMenu = fragment;
    }
}

public class ContextMenuRegisteredEventArgs<T> : ContextMenuRegisteredEventArgs 
    where T : PopupMenuBase?
{
    public ContextMenuRegisteredEventArgs(RenderFragment fragment)
        : base (typeof(T), fragment)
    {

    }
}
