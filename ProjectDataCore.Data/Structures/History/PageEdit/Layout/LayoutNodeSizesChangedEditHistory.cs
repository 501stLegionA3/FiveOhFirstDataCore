using ProjectDataCore.Data.Structures.Page.Components.Layout;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.History.PageEdit.Layout;
public class LayoutNodeSizesChangedEditHistory : EditHistoryItemBase
{
    public LayoutNode ParentNode { get; private set; }
    public string OldSizes { get; private set; }
    public string NewSizes { get; private set; }

    public LayoutNodeSizesChangedEditHistory(string name, LayoutNode parentNode, string oldSizes)
        : base(name)
    {
        ParentNode = parentNode;
        OldSizes = oldSizes;
        NewSizes = ParentNode.RawNodeWidths;
    }

    public override Task<ActionResult> Redo(IServiceProvider serviceProvider)
    {
        ParentNode.SetNodeWidths(NewSizes);

        return Task.FromResult<ActionResult>(new(true, null));
    }

    public override Task<ActionResult> Undo(IServiceProvider serviceProvider)
    {
        ParentNode.SetNodeWidths(OldSizes);

        return Task.FromResult<ActionResult>(new(true, null));
    }
}
