using ProjectDataCore.Data.Structures.Page.Components.Layout;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Result;

public class LayoutNodeModifiedResult
{
    /// <summary>
    /// The <see cref="LayoutNode"/> that called this action.
    /// </summary>
    public LayoutNode CalledFrom { get; set; }
    /// <summary>
    /// The parent <see cref="LayoutNode"/> for the <see cref="ModifiedNode"/>
    /// </summary>
    public LayoutNode ParentNode { get; set; }
    /// <summary>
    /// The <see cref="LayoutNode"/> that was modified.
    /// </summary>
    public LayoutNode ModifiedNode { get; set; }
    /// <summary>
    /// The other <see cref="LayoutNode"/> that was also effected by this action, if any.
    /// </summary>
    public LayoutNode? SecondaryNode { get; set; }
    /// <summary>
    /// The node that replaced the <see cref="ModifiedNode"/> during a delete operation, if any.
    /// </summary>
    public LayoutNode? ReplacedBy { get; set; }
    /// <summary>
    /// True if this operation was done on a row of nodes.
    /// </summary>
    public bool Row { get; set; }
    /// <summary>
    /// True if this operation was done upwards or leftwards in the grid.
    /// </summary>
    public bool UpOrLeft { get; set; }
    /// <summary>
    /// The node width values of the parent of <see cref="ModifiedNode"/>.
    /// </summary>
    public string ParentWidths { get; set; }

    public LayoutNodeModifiedResult(LayoutNode calledFrom, LayoutNode parentNode, LayoutNode modifiedNode, 
        LayoutNode? secondaryNode, LayoutNode? replacedBy, bool upOrLeft)
    {
        CalledFrom = calledFrom;
        ParentNode = parentNode;
        ModifiedNode = modifiedNode;
        SecondaryNode = secondaryNode;
        ReplacedBy = replacedBy;
        UpOrLeft = upOrLeft;

        Row = ParentNode.Rows;
        ParentWidths = ParentNode.RawNodeWidths;
    }
}
