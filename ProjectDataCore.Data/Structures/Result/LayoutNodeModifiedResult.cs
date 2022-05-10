using ProjectDataCore.Data.Structures.Page.Components.Layout;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Result;

public class LayoutNodeModifiedResult
{
    public LayoutNode ModifiedNode { get; set; }
    public LayoutNode? SecondaryNode { get; set; }
    public LayoutNode ParentNode { get; set; }
    public bool Row { get; set; }
    public bool UpOrLeft { get; set; }

    public LayoutNodeModifiedResult(LayoutNode parentNode, LayoutNode modifiedNode, LayoutNode? secondaryNode, bool upOrLeft)
	{
        ParentNode = parentNode;
        ModifiedNode = modifiedNode;
        SecondaryNode = secondaryNode;
        UpOrLeft = upOrLeft;

        Row = ParentNode.Rows;
	}
}
