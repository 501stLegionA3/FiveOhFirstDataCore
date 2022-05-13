using Microsoft.AspNetCore.Components.Web;

using ProjectDataCore.Data.Services.User;
using ProjectDataCore.Data.Structures.Events.Parameters;
using ProjectDataCore.Data.Structures.Keybindings;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Services.Keybindings;
public class KeybindingService : IKeybindingService
{
    private readonly ILocalUserService _localUserService;

    // For all listener lists, the first node is the active listener.
    // Additions should be done to the front of the linked list, not the rear.
    private LinkedList<Func<Task>> saveListeners = new();
    private LinkedList<Func<Task>> undoListeners = new();
    private LinkedList<Func<Task>> redoListeners = new();

    private ConcurrentDictionary<OnPressEventArgs, Keybinding> defaults = new();
    private bool initalized = false;

    public KeybindingService(ILocalUserService userService)
    {
        _localUserService = userService;
    }

    public void RegisterKeybindListener(Keybinding binding, Func<Task> listener)
    {
        LinkedList<Func<Task>>? list;
        if((list = GetListeners(binding)) is not null)
        {
            list.AddFirst(listener);
        }
    }

    public void RemoveKeybindListener(Keybinding binding, Func<Task> listener)
    {
        LinkedList<Func<Task>>? list;
        if ((list = GetListeners(binding)) is not null)
        {
            list.Remove(listener);
        }
    }

    public async Task<Keybinding?> ExecuteKeybindingAsync(OnPressEventArgs args)
    {
        var binding = await GetKeybindingAsync(args);

        await ExecuteKeybindingAsync(binding);

        return binding;
    }

    public async Task ExecuteKeybindingAsync(Keybinding? binding)
    {
        LinkedList<Func<Task>>? list;
        if ((list = GetListeners(binding)) is not null)
        {
            if (list.First is not null)
                await list.First.Value.Invoke();
        }
    }

    private LinkedList<Func<Task>>? GetListeners(Keybinding? keybinding) 
        => keybinding switch
        {
            Keybinding.Save => saveListeners,
            Keybinding.Undo => undoListeners,
            Keybinding.Redo => redoListeners,
            _ => null,
        };

    private async Task<Keybinding?> GetKeybindingAsync(OnPressEventArgs args)
    {
        if (!initalized)
            Initalize();

        var userBindings = await _localUserService.GetCustomKeybindings();
        if (userBindings.TryGetValue(args, out var binding))
            return binding;

        // Remove all custom keybindings from
        // the list of defaults.
        var filteredDefaults = defaults.Where((x) =>
        {
            foreach (var b in userBindings)
                if (x.Value == b.Value) 
                    return false;

            return true;
        }).ToDictionary(x => x.Key, x => x.Value);
        
        if (filteredDefaults.TryGetValue(args, out binding))
            return binding;

        return null;
    }

    private void Initalize()
    {
        initalized = true;
        defaults.Clear();

        _ = defaults.TryAdd(new()
        {
            CtrlKey = true,
            Key = "S"
        }, Keybinding.Save);

        _ = defaults.TryAdd(new()
        {
            CtrlKey = true,
            Key = "Z"
        }, Keybinding.Undo);

        _ = defaults.TryAdd(new()
        {
            CtrlKey = true,
            Key = "Y"
        }, Keybinding.Redo);
    }
}
