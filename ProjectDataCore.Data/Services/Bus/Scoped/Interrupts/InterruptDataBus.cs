using Microsoft.AspNetCore.Components;

using ProjectDataCore.Data.Structures.Events.Parameters;
using ProjectDataCore.Data.Structures.Page.ContextMenu;

namespace ProjectDataCore.Data.Services.Bus.Scoped;
public partial class ScopedDataBus : IScopedDataBus
{
    public event IScopedDataBus.PopupMenuOpenRequestedEventHandler? PopupMenuOpenRequested;
    public event IScopedDataBus.PopupMenuCloseRequestEventHandler? PopupMenuCloseRequested;
    public void OpenPopupMenu<T>(object sender, PopupMenuOpenRequestEventArgs<T> args) where T : PopupMenuBase?
        => OpenPopupMenu(sender, args as PopupMenuOpenRequestEventArgs);

    public void OpenPopupMenu(object sender, PopupMenuOpenRequestEventArgs args)
    {
        if (PopupMenuOpenRequested is not null)
            PopupMenuOpenRequested.Invoke(sender, args);
    }

    public void ClosePopupMenu(object sender)
    {
        if (PopupMenuCloseRequested is not null)
            PopupMenuCloseRequested.Invoke(sender);
    }
}
