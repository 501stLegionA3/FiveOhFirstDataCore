using ProjectDataCore.Data.Structures.Page.Components.Layout;

namespace ProjectDataCore.Data.Structures.History.PageEdit;
public class LayoutNodeSplitEditHistory : EditHistoryItemBase
{
    public LayoutNode Node { get; private set; }
    public LayoutNode? SiblingNode { get; private set; }
    public LayoutNode AddCaller { get; private set; }
    public bool Row { get; private set; }
    public bool UpOrLeft { get; private set; }

    public LayoutNodeSplitEditHistory(string name, LayoutNodeModifiedResult result)
        : base(name) 
    {
        Node = result.ModifiedNode;
        AddCaller = result.CalledFrom;

        AssignValues(result);
    }

    private void AssignValues(LayoutNodeModifiedResult result)
    {
        Node = result.ModifiedNode;
        SiblingNode = result.SecondaryNode;

        AddCaller = result.ParentNode;

        Row = result.Row;
        UpOrLeft = result.UpOrLeft;
    }

    public override Task<ActionResult> Undo(IServiceProvider serviceProvider)
    {
        var res = Node.DeleteNode(!UpOrLeft);

        if (res.GetResult(out var data, out _))
            AssignValues(data);

        return Task.FromResult<ActionResult>(res);
    }

    public override Task<ActionResult> Redo(IServiceProvider serviceProvider)
    {
        var res = AddCaller.AddNode(Row, !UpOrLeft, new LayoutNode?[] { Node, SiblingNode });

        if (res.GetResult(out var data, out _))
            AssignValues(data);

        return Task.FromResult<ActionResult>(res);
    }
}
