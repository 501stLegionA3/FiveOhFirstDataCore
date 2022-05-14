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
    private HashSet<Func<Task>> saveListenersSet = new();

    private LinkedList<Func<Task>> undoListeners = new();
    private HashSet<Func<Task>> undoListenersSet = new();

    private LinkedList<Func<Task>> redoListeners = new();
    private HashSet<Func<Task>> redoListenersSet = new();

    private ConcurrentDictionary<OnPressEventArgs, Keybinding> defaults = new();
    private bool initalized = false;

    public KeybindingService(ILocalUserService userService)
    {
        _localUserService = userService;
    }

    public void RegisterKeybindListener(Keybinding binding, Func<Task> listener)
    {
        (LinkedList<Func<Task>>, HashSet<Func<Task>>)? pair;
        if((pair = GetListenerPair(binding)) is not null)
        {
            if(pair.Value.Item2.Add(listener))
            {
                pair.Value.Item1.AddFirst(listener);
            }
        }
    }

    public void RemoveKeybindListener(Keybinding binding, Func<Task> listener)
    {
        (LinkedList<Func<Task>>, HashSet<Func<Task>>)? pair;
        if ((pair = GetListenerPair(binding)) is not null)
        {
            _ = pair.Value.Item1.Remove(listener);
            _ = pair.Value.Item2.Remove(listener);
        }
    }

    public async Task<Keybinding?> ExecuteKeybindingAsync(OnPressEventArgs args)
    {
        var binding = GetKeybinding(args);

        if(binding is not null)
            await ExecuteKeybindingAsync(binding);

        return binding;
    }

    public async Task ExecuteKeybindingAsync(Keybinding? binding)
    {
        (LinkedList<Func<Task>>, HashSet<Func<Task>>)? pair;
        if ((pair = GetListenerPair(binding)) is not null)
        {
            if (pair.Value.Item1.First is not null)
                await pair.Value.Item1.First.Value.Invoke();
        }
    }

    private (LinkedList<Func<Task>>, HashSet<Func<Task>>)? GetListenerPair(Keybinding? keybinding) 
        => keybinding switch
        {
            Keybinding.Save => (saveListeners, saveListenersSet),
            Keybinding.Undo => (undoListeners, undoListenersSet),
            Keybinding.Redo => (redoListeners, redoListenersSet),
            _ => null,
        };

    private Keybinding? GetKeybinding(OnPressEventArgs args)
    {
        if (!initalized)
            Initalize();

        var userBindings = _localUserService.GetCustomKeybindings();
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
