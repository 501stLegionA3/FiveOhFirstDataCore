using Microsoft.Extensions.Configuration;

using ProjectDataCore.Data.Structures.History;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Services.History;
public class EditHistoryService : IEditHistoryService
{
    private readonly IServiceProvider _serviceProvider;

    private LinkedList<EditHistoryItemBase> UndoList { get; init; } = new();
    private Stack<EditHistoryItemBase> RedoStack { get; init; } = new();
    private int UndoLimit { get; init; }

    public EditHistoryService(IServiceProvider serviceProvider, IConfiguration config)
    {
        _serviceProvider = serviceProvider;
        UndoLimit = Convert.ToInt32(config["Config:EditState:UndoLimit"]);
    }

    public void Push(EditHistoryItemBase item)
    {
        RedoStack.Clear();

        UndoList.AddFirst(item);

        if (UndoList.Count > UndoLimit)
            UndoList.RemoveLast();
    }

    public async Task<ActionResult> RedoAsync()
    {
        if(RedoStack.TryPop(out var item))
        {
            var res = await item.Redo(_serviceProvider);

            if (res.GetResult(out _))
                UndoList.AddFirst(item);

            return res;
        }

        return new(false, new List<string> { "No items to redo." });
    }

    public async Task<ActionResult> UndoAsync()
    {
        if (UndoList.First is not null)
        {
            var item = UndoList.First.Value;
            UndoList.RemoveFirst();

            var res = await item.Undo(_serviceProvider);

            if (res.GetResult(out _))
                RedoStack.Push(item);

            return res;
        }

        return new(false, new List<string> { "No items to undo." });
    }

	public void Reset()
	{
        UndoList.Clear();
        RedoStack.Clear();
	}
}
