using ProjectDataCore.Data.Structures.Page.Components.Layout;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.History.PageEdit;
public class LayoutNodeSplitEditHistory : EditHistoryItemBase
{
    public LayoutNode NewNode { get; set; }
    public LayoutNode? SiblingNode { get; set; }
    public LayoutNode ParentNode { get; set; }
    public bool Row { get; set; }
    public bool AddedLeftOrUp { get; set; }

    public LayoutNodeSplitEditHistory(string name, LayoutNodeModifiedResult result)
        : this(name, result.ModifiedNode, result.SecondaryNode, result.ParentNode, result.Row, result.UpOrLeft) { }

    public LayoutNodeSplitEditHistory(string name, LayoutNode newNode, LayoutNode? siblingNode, LayoutNode parentNode, bool row, bool addedLeftOrUp)
        : base (name)
    {
        NewNode = newNode;
        SiblingNode = siblingNode;
        ParentNode = parentNode;
        Row = row;
        AddedLeftOrUp = addedLeftOrUp;
    }

    public override Task<ActionResult> Undo(IServiceProvider serviceProvider)
    {
        var res = NewNode.DeleteNode();

        return Task.FromResult<ActionResult>(res);
    }

    public override Task<ActionResult> Redo(IServiceProvider serviceProvider)
    {
        var res = ParentNode.AddNode(Row, AddedLeftOrUp, new LayoutNode?[] { SiblingNode, NewNode });

        return Task.FromResult<ActionResult>(res);
    }
}
