using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Page.Components.Layout;
public class LayoutNode : DataObject<Guid>
{
    public PageComponentSettingsBase? Component { get; set; }
    public Guid? ComponentId { get; set; }

    public LayoutNode? ParentNode { get; set; }
    public Guid? ParentNodeId { get; set; }

    public List<LayoutNode> Nodes { get; set; } = new();
    public int Order { get; set; }
    public bool Rows { get; set; } = false;
    public string RawNodeWidths { get; internal set; } = "";
    private string[]? _nodeWidths = null;
    public string[] NodeWidths 
    {
        get
        {
            if (_nodeWidths is null)
            {
                _nodeWidths = RawNodeWidths.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Where(x => x.Contains('%')).ToArray();
            }

            return _nodeWidths;
        }
    }

    public void SetNodeWidths(string raw)
    {
        _nodeWidths = null;
        RawNodeWidths = raw;
    }

    /// <summary>
    /// Adds a new Layout Node to the node tree.
    /// </summary>
    /// <remarks>
    /// A node will be added to the parent node if the direction of the node (row or col) matches
    /// with the parents direction. It will be added as a new child grouping by moving this node
    /// and the newly added node to 
    /// </remarks>
    /// <param name="row">Is the new node a row? true if it is, false if it is column.</param>
    /// <returns>A <see cref="bool"/> that will be true if a node was added to this object,
    /// and false if it was added to the parent.</returns>
    public bool AddNode(bool row)
    {
        if (ParentNode is null)
        {
            // There should never be an instance where the parent node is null
            // and we are attempting to add a new node.
            throw new InvalidOperationException("The parent node can not be null when adding a new node.");
        }

        // ... check to see if there is more than one node in the parent ...
        if(ParentNode.Nodes.Count <= 1)
        {
            // ... if there isnt, we add a new node ...
            ParentNode.Nodes.Add(new());
            // ... and set the direction of the parent grid ...
            ParentNode.Rows = row;
            // ... and let the user know the parent was modified ...
            return false;
        }
        // ... otherwise, we need to check if we can add to the parent ...
        else
        {
            // ... by comparing the node directions ...
            if (ParentNode.Rows == row)
            {
                // ... if we can add, then add and let the user know ...
                ParentNode.Nodes.Add(new());

                return false;
            }
            // ... otherwise, we need to do some fancy work,
            // but only if there are no child nodes yet ...
            else if (Nodes.Count <= 0)
            {
                // ... first, lets make a new child node that contains the information from this node ...
                var node = new LayoutNode()
                {
                    Component = Component,
                    ComponentId = ComponentId,
                    ParentNode = this
                };

                // ... and clear the component data for this node ...
                Component = null;
                ComponentId = null;

                // ... then save the node as a child ...
                Nodes.Add(node);

                // ... finally call add on this new node ...
                node.AddNode(row);

                // ... then tell the user that a new subset of nodes was
                // created ....
                return true;
            }
            // ... we should not get to this point,
            // so something went wrong when adding the node ...
            else
            {
                throw new InvalidOperationException("Node addition failed, node was against the pattern and the" +
                    " this node was not empty to create a child node set.");
            }
        }
    }

    public bool DeleteNode(LayoutNode node)
    {
        throw new NotImplementedException();
    }
}
