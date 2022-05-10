using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.History;
public abstract class EditHistoryItemBase
{
    public string Name { get; init; }

    public EditHistoryItemBase(string name)
    {
        Name = name;
    }

    public abstract Task<ActionResult> Undo(IServiceProvider serviceProvider);

    public abstract Task<ActionResult> Redo(IServiceProvider serviceProvider);
}
