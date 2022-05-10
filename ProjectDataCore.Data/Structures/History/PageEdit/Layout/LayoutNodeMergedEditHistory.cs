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
    public LayoutNode SecondaryNode { get; set; }
    public bool MergedLeftOrUp { get; set; }
    public bool Row { get; set; }

    public LayoutNodeMergedEditHistory(string name, LayoutNode replacedBy, LayoutNode removedNode, LayoutNode secondaryNode, bool mergedLeftOrUp, bool row)
        : base(name)
    {
        ReplacedBy = replacedBy;
        RemovedNode = removedNode;
        SecondaryNode = secondaryNode;
        MergedLeftOrUp = mergedLeftOrUp;
        Row = row;
    }

    public override Task<ActionResult> Undo(IServiceProvider serviceProvider)
    {
        var res = ReplacedBy.AddNode(Row, !MergedLeftOrUp, new LayoutNode[] { RemovedNode, SecondaryNode });
        
        return Task.FromResult<ActionResult>(res);
    }

    public override Task<ActionResult> Redo(IServiceProvider serviceProvider)
    {
        var res = RemovedNode.DeleteNode();
        
        return Task.FromResult<ActionResult>(res);
    }
}
