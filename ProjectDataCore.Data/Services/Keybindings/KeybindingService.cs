using Microsoft.AspNetCore.Components.Web;

using ProjectDataCore.Data.Structures.Keybindings;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Services.Keybindings;
public class KeybindingService : IKeybindingService
{
    // Params and methods for this class will be separated by region due to
    // the estimated size of this service in the future.

    public void RegisterKeybindListener(Keybinding binding, Func<Task> listener)
    {
        throw new NotImplementedException();
    }

    public void RemoveKeybindListener(Keybinding binding, Func<Task> listener)
    {
        throw new NotImplementedException();
    }

    public Task ExecuteKeybindingAsync(KeyboardEventArgs args)
    {
        throw new NotImplementedException();
    }

    public Task ExecuteKeybindingAsync(Keybinding keybinding)
    {
        throw new NotImplementedException();
    }

    #region Save
    private HashSet<Func<Task>> saveListeners = new();

    #endregion

    #region Undo

    #endregion

    #region Redo

    #endregion
}
