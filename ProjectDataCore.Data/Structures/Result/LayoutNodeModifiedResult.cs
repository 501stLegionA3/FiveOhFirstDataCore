using ProjectDataCore.Data.Structures.Page.Components.Layout;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Result;

public class LayoutNodeModifiedResult
{
    public LayoutNode CalledFrom { get; set; }
    public LayoutNode ParentNode { get; set; }
    public LayoutNode ModifiedNode { get; set; }
    public LayoutNode? SecondaryNode { get; set; }
    public LayoutNode? ReplacedBy { get; set; }
    public bool Row { get; set; }
    public bool UpOrLeft { get; set; }

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
	}
}
