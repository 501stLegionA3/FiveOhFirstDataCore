using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

using ProjectDataCore.Data.Structures.Page.ContextMenu;

namespace ProjectDataCore.Data.Structures.Events.Parameters;

public class PopupMenuOpenRequestEventArgs : EventArgs
{
    public Type MenuType { get; init; }
    public MouseEventArgs EventArgs { get; init; }
    public RenderFragment Fragment { get; init; }

    public PopupMenuOpenRequestEventArgs(Type menuType, MouseEventArgs eventArgs, RenderFragment fragment)
    {
        MenuType = menuType;
        EventArgs = eventArgs;
        Fragment = fragment;
    }
}

public class PopupMenuOpenRequestEventArgs<T> : PopupMenuOpenRequestEventArgs
    where T : PopupMenuBase?
{
    public PopupMenuOpenRequestEventArgs(MouseEventArgs eventArgs, RenderFragment fragment)
        : base (typeof(T), eventArgs, fragment)
    {

    }
}
