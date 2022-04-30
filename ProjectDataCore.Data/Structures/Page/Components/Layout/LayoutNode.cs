using ProjectDataCore.Data.Structures.Util.Comparers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Page.Components.Layout;
public class LayoutNode : DataObject<Guid>
{
    internal const string GUTTER_SIZE = "10px";

    private string? _editorKey;
    public string EditorKey
    {
        get
        {
            if (_editorKey is null)
                _editorKey = Guid.NewGuid().ToString();

            return _editorKey;
        }
    }

    public PageComponentSettingsBase? Component { get; set; }
    public Guid? ComponentId { get; set; }

    public LayoutNode? ParentNode { get; set; }
    public Guid? ParentNodeId { get; set; }

    public CustomPageSettings? PageSettings { get; set; }
    public Guid? PageSettingsId { get; set; }

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

    // TODO handle width setup on add node

    /// <summary>
    /// Adds a new Layout Node to the node tree.
    /// </summary>
    /// <remarks>
    /// A node will be added to the parent node if the direction of the node (row or col) matches
    /// with the parents direction. It will be added as a new child grouping by moving this node
    /// and the newly added node to 
    /// </remarks>
    /// <param name="row">Is the new node a row? true if it is, false if it is column.</param>
    /// <param name="addAboveOrLeft">If the new node should be added above or to the left (depending of if it is a
    /// row or column) or if it should be added below or to the right.</param>
    /// <returns>A <see cref="bool"/> that will be true if a node was added to this object,
    /// and false if it was added to the parent.</returns>
    public bool AddNode(bool row, bool addAboveOrLeft)
    {
        // ... check to see if this has a parent node ...
        if (ParentNode is null)
        {
            // ... if the parent node is null, then this is the uppermost node handler
            // and we need to add two children ...
            Nodes.Add(CreateChild(!addAboveOrLeft, 0));
            // ... insert node will handle both new nodes to
            // set the node widths value, so lets
            // clear any existing values first ...
            SetNodeWidths("");
            InsertNode(CreateChild(!addAboveOrLeft, 0), addAboveOrLeft);

            // ... because this is going to be the first node, we also set the
            // row/col indicator ...
            Rows = row;

            return true;
        }

        // ... check to see if there is more than one node in the parent ...
        if(ParentNode.Nodes.Count <= 1)
        {
            // ... if there isnt, we add a new node ...
            ParentNode.InsertNode(ParentNode.CreateChild(false, Order), addAboveOrLeft);
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
                ParentNode.InsertNode(ParentNode.CreateChild(false, Order), addAboveOrLeft);

                return false;
            }
            // ... otherwise, we need to do some fancy work,
            // but only if there are no child nodes yet ...
            else if (Nodes.Count <= 0)
            {
                // ... first, lets make a new child node that contains the information from this node ...
                var node = CreateChild(true, 0);

                // ... then save the node as a child ...
                Nodes.Add(node);

                // ... clear this objects node widths values ...
                SetNodeWidths("");

                // ... finally call add on this new node ...
                node.AddNode(row, addAboveOrLeft);

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

    internal void InsertNode(LayoutNode node, bool addAboveOrLeft)
    {
        // add the new node ...
        Nodes.Add(node);
        // ... then order those nodes ...
        var sorted = OrderNodes(addAboveOrLeft);
        // ... and for each item in the sort ...
        foreach(var pair in sorted)
        {
            // ... if the node is the node we just added ...
            if(pair.Value == node)
            {
                // ... get the width values ...
                var widths = RawNodeWidths.Split(' ');
                // ... then the values without the gutters ...
                var sizes = widths.Where(x => x != GUTTER_SIZE).ToList();
                // ... then if there is a previous value we can get ...
                var prevSize = pair.Value.Order + (addAboveOrLeft ? -1 : 0);
                // ... and set an indicator for generating new size values ...
                bool generateSizes = true;
                if(sizes.Count > prevSize && prevSize >= 0)
                {
                    // ... pull the size string ...
                    var sizeStr = sizes[prevSize];

                    // ... if there is a size value ...
                    if (!string.IsNullOrWhiteSpace(sizeStr))
                    {
                        // ... then trim that to remove the % sign ...
                        var sizeTrim = sizeStr[0..(sizeStr.Length - 1)];
                        // ... and turn it into an integer ...
                        if (double.TryParse(sizeTrim, out double size))
                        {
                            // ... find the first half of the split ...
                            double halfOne = size / 2;
                            // ... and the second ...
                            double halfTwo = size - halfOne;

                            // ... then set the existing size to half one ...
                            sizes[prevSize] = $"{halfOne}%";
                            // ... and the new value to half two ...
                            sizes.Insert(pair.Value.Order + (addAboveOrLeft ? -1 : 1), $"{halfTwo}%");

                            // ... then make a new node widths value by combining all the
                            // percentages with gutters ...
                            SetNodeWidths(string.Join($" {GUTTER_SIZE} ", sizes));

                            // ... because all this worked, set the generate sizes value
                            // to false ...
                            generateSizes = false;
                        }
                    }
                }

                // ... if we need to generate new sizes ...
                if (generateSizes)
                {
                    // ... generate them ...
                    GenerateNodeSizes(sorted);
                }

                // ... then break out of the loop.
                break;
            }
        }
    }

    /// <summary>
    /// Deletes a child node from it's parent.
    /// </summary>
    /// <param name="node">The node to remove.</param>
    /// <param name="mergeLeftOrUp">True if the system should merge left, false if it should merege right.
    /// Does not have any effect when there is only one node to remove.</param>
    /// <returns>True if the node was removed, false if not.</returns>
    internal bool DeleteNode(LayoutNode node, bool mergeLeftOrUp = true)
    {
        var res = Nodes.Remove(node);
        if (!res)
            return false;

        // ... if there is a single child node left ...
        if(Nodes.Count == 1)
        {
            // ... get the child ...
            var child = Nodes[0];
            // ... take the child information and save it to this node ...
            Component = child.Component;
            ComponentId = child.ComponentId;

            if(Component is not null)
                Component.ParentNodeId = Key;

            // ... then remove the child ...
            Nodes.Remove(child);
        }
        // ... if there is more than one node ...
        else if (Nodes.Count > 1)
        {
            // ... then sort the remaning nodes ...
            var sorted = OrderNodes(mergeLeftOrUp);

            // ... then get the width values ...
            var widths = RawNodeWidths.Split(' ');
            // ... then the values without the gutters ...
            var sizes = widths.Where(x => x != GUTTER_SIZE).ToList();
            // ... then if there is a previous value we can get (either left or right) ...
            var prevSize = node.Order + (mergeLeftOrUp ? -1 : 1);
            // ... and set an indicator for generating new size values ...
            bool generateSizes = true;
            if (sizes.Count > prevSize && sizes.Count > node.Order
                && prevSize >= 0 && node.Order >= 0)
            {
                // ... pull the size string ...
                var sizeStr = sizes[prevSize];
                // ... and trim that to remove the % sign ...
                var sizeTrimLeft = sizeStr[0..(sizeStr.Length - 1)];

                // ... then do it again for the other side of the removed node ...
                sizeStr = sizes[node.Order];
                var sizeTrimRight = sizeStr[0..(sizeStr.Length - 1)];

                // ... and turn it into an integer ...
                if (int.TryParse(sizeTrimLeft, out int sizeLeft)
                    && int.TryParse(sizeTrimRight, out int sizeRight))
                {
                    // ... combine the sizes ...
                    int size = sizeLeft + sizeRight;

                    // ... then set the existing size to the new size ...
                    sizes[prevSize] = $"{size}%";
                    // ... then remove the value for the node we got rid of ...
                    sizes.RemoveAt(node.Order);

                    // ... then make a new node widths value by combining all the
                    // percentages with gutters ...
                    SetNodeWidths(string.Join($" {GUTTER_SIZE} ", sizes));

                    // ... because all this worked, set the generate sizes value
                    // to false ...
                    generateSizes = false;
                }
            }

            // ... if we need to generate new sizes ...
            if (generateSizes)
            {
                // ... generate them ...
                GenerateNodeSizes(sorted);
            }
        }

        // ... then let the caller know this was a success
        return true;
    }

    /// <summary>
    /// Deletes this node.
    /// </summary>
    /// <param name="mergeLeft">True if the system should merge left, false if it should merege right.
    /// Does not have any effect when there is only one node to remove.</param>
    /// <remarks>
    /// The top level parent node can not be deleted. If you want
    /// to remove that node, you need to delete the page itself.
    /// </remarks>
    /// <returns>A <see cref="bool"/> value that is true if the
    /// node was deleted and false if it was not.</returns>
    public bool DeleteNode(bool mergeLeft = true)
    {
        if(ParentNode is null)
        {
            // The parent node can not be deleted - delete
            // the page instead.
            return false;
        }

        return ParentNode.DeleteNode(this, mergeLeft);
    }

    private SortedList<int, LayoutNode> OrderNodes(bool sameIsLower)
    {
        // ... recalculate the order of the nodes ...
        SortedList<int, LayoutNode> children = new(new DuplicateComparer<int>(sameIsLower));
        // ... by placing them all in an sorted list ...
        foreach (var child in Nodes)
            children.Add(child.Order, child);
        // ... then assigning the new order values ...
        int i = 0;
        foreach (var child in children.Values)
            child.Order = i++;

        return children;
    }

    private void GenerateNodeSizes(SortedList<int, LayoutNode> sorted)
    {
        // ... then make a new sizes list ...
        List<string> newSizes = new();
        // ... and calculate the percent that each item will take up ...
        int percent = (int)(1d / sorted.Count * 100);
        for (int i = 0; i < sorted.Count; i++)
        {
            // ... then add the percent value for each item in the split ...
            newSizes.Add($"{percent}%");
        }

        // ... then make a new node widths value by combining all the
        // percentages with gutters ...
        SetNodeWidths(string.Join($" {GUTTER_SIZE} ", newSizes));
    }

    /// <summary>
    /// Creates a new node that is child of this node.
    /// </summary>
    /// <remarks>
    /// This method does not add the child node to <see cref="Nodes"/>
    /// </remarks>
    /// <param name="transferComponentData">If the component data for this node should
    /// be transfered to the new child.</param>
    /// <returns>A new <see cref="LayoutNode"/> that is a child of this node.</returns>
    private LayoutNode CreateChild(bool transferComponentData, int order)
    {
        // ... create a new child object ...
        var node = new LayoutNode()
        {
            ParentNode = this,
            ParentNodeId = this.Key,
            Order = order,
        };

        // ... if we need to transfer component data ...
        if (transferComponentData)
        {
            // ... then move the data over ...
            node.Component = Component;
            node.ComponentId = ComponentId;

            // ... and configure the component if its exists ...
            if (node.Component is not null)
            {
                node.Component.ParentNode = node;
                node.Component.ParentNodeId = node.Key;
            }

            // ... and clear this nodes data ...
            Component = null;
            ComponentId = null;
        }
        // ... then treturn the node.
        return node;
    }
}
