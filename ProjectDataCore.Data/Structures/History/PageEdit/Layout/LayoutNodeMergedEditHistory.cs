using ProjectDataCore.Data.Structures.Page.Components.Layout;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.History.PageEdit;
public class LayoutNodeMergedEditHistory : EditHistoryItemBase
{
    private LayoutNodeSplitEditHistory Handler { get; set; }

    public LayoutNodeMergedEditHistory(string name, LayoutNodeModifiedResult result)
        : base(name)
    {
        Handler = new(name, result);
    }

    public override async Task<ActionResult> Undo(IServiceProvider serviceProvider)
        => await Handler.Redo(serviceProvider);

    public override async Task<ActionResult> Redo(IServiceProvider serviceProvider)
        => await Handler.Undo(serviceProvider);
}
