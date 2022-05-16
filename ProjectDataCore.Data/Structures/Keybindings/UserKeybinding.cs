using ProjectDataCore.Data.Structures.Events.Parameters;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Keybindings;
public class UserKeybinding : DataObject<Guid>
{
    public Keybinding Keybinding { get; set; }
    public string KeyPressed { get; set; }
    public bool ShiftKey { get; set; }
    public bool CtrlKey { get; set; }
    public bool AltKey { get; set; }
    public bool MetaKey { get; set; }

    public DataCoreUser DataCoreUser { get; set; }
    public Guid DataCoreUserId { get; set; }

    public OnPressEventArgs GetMinimalKeyboardEventArgs()
        => new(KeyPressed, ShiftKey, CtrlKey, AltKey, MetaKey);
}
