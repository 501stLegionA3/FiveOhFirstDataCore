using Microsoft.AspNetCore.Components;

using ProjectDataCore.Data.Structures.Events.Parameters;
using ProjectDataCore.Data.Structures.Page.ContextMenu;

namespace ProjectDataCore.Data.Services.Bus.Scoped;
public partial interface IScopedDataBus
{
    public delegate void PopupMenuOpenRequestedEventHandler(object sender, PopupMenuOpenRequestEventArgs args);
    public event PopupMenuOpenRequestedEventHandler PopupMenuOpenRequested;
    public void OpenPopupMenu<T>(object sender, PopupMenuOpenRequestEventArgs<T> args) where T : PopupMenuBase?;
    public void OpenPopupMenu(object sender, PopupMenuOpenRequestEventArgs args);

    public delegate void PopupMenuCloseRequestEventHandler(object sender);
    public event PopupMenuCloseRequestEventHandler PopupMenuCloseRequested;
    public void ClosePopupMenu(object sender);
}
