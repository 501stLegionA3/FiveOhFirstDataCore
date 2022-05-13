using Microsoft.AspNetCore.Components.Web;

using ProjectDataCore.Data.Structures.Events.Parameters;
using ProjectDataCore.Data.Structures.Keybindings;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Services.Keybindings;
public interface IKeybindingService
{
    /// <summary>
    /// Registers a keybinding listener. Only the most recently registered will be executed.
    /// </summary>
    /// <param name="binding">The binding to register the <paramref name="listener"/> for.</param>
    /// <param name="listener">The function to execute when this keybining is used.</param>
    public void RegisterKeybindListener(Keybinding binding, Func<Task> listener);
    /// <summary>
    /// Removes a keybinding listener. If a listener was registered before this one, that listener
    /// become active again.
    /// </summary>
    /// <param name="binding">The binding to remove the <paramref name="listener"/> from.</param>
    /// <param name="listener">The function to remove from the execution list.</param>
    public void RemoveKeybindListener(Keybinding binding, Func<Task> listener);

    /// <summary>
    /// Execute a keybinding.
    /// </summary>
    /// <param name="args">The event args of a keyboard action on the website.</param>
    /// <returns>A task for this action.</returns>
    public Task<Keybinding?> ExecuteKeybindingAsync(OnPressEventArgs args);
    /// <summary>
    /// Execute a keybinding.
    /// </summary>
    /// <param name="keybinding">The keybinding to execute.</param>
    /// <returns>A task for this action.</returns>
    public Task ExecuteKeybindingAsync(Keybinding? keybinding);
}
