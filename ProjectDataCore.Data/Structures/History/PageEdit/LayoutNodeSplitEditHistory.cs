using ProjectDataCore.Data.Structures.Page.Components.Layout;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.History.PageEdit;
public class LayoutNodeSplitEditHistory : EditHistoryItemBase
{
    public LayoutNode NewNode { get; init; }
    public LayoutNode ParentNode { get; set; }
    public bool Row { get; set; }
    public bool AddedLeftOrUp { get; set; }

    public LayoutNodeSplitEditHistory(string name, LayoutNode newNode, LayoutNode parentNode, bool row, bool addedLeftOrUp)
        : base (name)
    {
        NewNode = newNode;
        ParentNode = parentNode;
        Row = row;
        AddedLeftOrUp = addedLeftOrUp;
    }

    public override Task<ActionResult> Undo(IServiceProvider serviceProvider)
    {
        if (NewNode.DeleteNode())
            return Task.FromResult<ActionResult>(new(true, null));

        return Task.FromResult<ActionResult>(new(false, new List<string> { "Failed to remove the node." }));
    }

    public override Task<ActionResult> Redo(IServiceProvider serviceProvider)
    {
        _ = ParentNode.AddNode(Row, AddedLeftOrUp, NewNode);
        
        return Task.FromResult<ActionResult>(new(true, null));
    }
}
