using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Keybindings;
public enum Keybinding
{
    [Description("Save")]
    Save,
    [Description("Undo")]
    Undo,
    [Description("Redo")]
    Redo
}
