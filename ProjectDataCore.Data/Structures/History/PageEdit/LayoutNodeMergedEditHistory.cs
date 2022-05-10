using ProjectDataCore.Data.Structures.Page.Components.Layout;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.History.PageEdit;
public class LayoutNodeMergedEditHistory : EditHistoryItemBase
{
    public LayoutNode ReplacedBy { get; set; }
    public LayoutNode RemovedNode { get; set; }
    public bool MergedLeftOrUp { get; set; }
    public bool Row { get; set; }

    public LayoutNodeMergedEditHistory(string name, LayoutNode replacedBy, LayoutNode removedNode, bool mergedLeftOrUp, bool row)
        : base(name)
    {
        ReplacedBy = replacedBy;
        RemovedNode = removedNode;
        MergedLeftOrUp = mergedLeftOrUp;
        Row = row;
    }

    public override Task<ActionResult> Undo(IServiceProvider serviceProvider)
    {
        _ = ReplacedBy.AddNode(Row, !MergedLeftOrUp, RemovedNode);
        
        return Task.FromResult<ActionResult>(new(true, null));
    }

    public override Task<ActionResult> Redo(IServiceProvider serviceProvider)
    {
        if(RemovedNode.DeleteNode())
            return Task.FromResult<ActionResult>(new(true, null));

        return Task.FromResult<ActionResult>(new(false, new List<string> { "Unable to delete the node." }));
    }
}
