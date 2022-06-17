using ProjectDataCore.Data.Structures.Page.Components.Scope;
using ProjectDataCore.Data.Structures.Policy;
using ProjectDataCore.Data.Structures.Util.Comparers;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

    /// <summary>
    /// If a user is required to be authroized to view this page. Setting this to <i><b>false</b></i> will
    /// make it a public page. Setting this to <i><b>true</b></i> without a <see cref="AuthorizationPolicy"/> assigned
    /// will make it a page that can be seen by anyone who is logged in.
    /// </summary>
    public bool RequireAuth { get; set; } = true;

    /// <summary>
    /// The Authorization Policy for when <see cref="RequireAuth"/> is <i><b>true</b></i>.
    /// </summary>
    public DynamicAuthorizationPolicy? AuthorizationPolicy { get; set; }
    /// <summary>
    /// The key for the <see cref="AuthorizationPolicy"/>
    /// </summary>
    public Guid? AuthorizationPolicyKey { get; set; }

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
    /// <param name="addAboveOrLeft">If the new node should be added above or to the left (depending of if it is a
    /// row or column) or if it should be added below or to the right.</param>
    /// <param name="nodeData">A <see cref="LayoutNode"/> to attach as a child for this object. If this object has
    /// a parent already, it will be removed from that parent.</param>
    /// <param name="widthOverwrite">Overwrites the node width values when adding a node. Can cause unexpected behavior and should not
    /// be used execpt with the values from a <see cref="LayoutNodeModifiedResult"/>.</param>
    /// <returns>A <see cref="ActionResult"/> with a <see cref="LayoutNodeModifiedResult"/> if the operation
    /// was successful.</returns>
    public ActionResult<LayoutNodeModifiedResult> AddNode(bool row, bool addAboveOrLeft, LayoutNode?[]? nodeData = null, string? widthOverwrite = null)
    {
        // ... check to see if this has a parent node ...
        if (ParentNode is null)
        {
            // ... if the parent node is null, then this is the uppermost node handler
            // and we need to add two children ...
            var secondChild = CreateChild(false, 0, nodeData?.ElementAtOrDefault(1));
            Nodes.Add(secondChild);
            // ... insert node will handle both new nodes to
            // set the node widths value, so lets
            // clear any existing values first ...
            SetNodeWidths("");
            var child = CreateChild(true, 0, nodeData?.ElementAtOrDefault(0));
            InsertNode(child, addAboveOrLeft);

            // ... because this is going to be the first node, we also set the
            // row/col indicator ...
            Rows = row;

            // ... then set the node widths if it needs to be overwritten ...
            if (widthOverwrite is not null)
                SetNodeWidths(widthOverwrite);

            return new(true, null, new(this, this, child, secondChild, null, addAboveOrLeft));
        }

        // ... check to see if there is more than one node in the parent ...
        if(ParentNode.Nodes.Count <= 1)
        {
            // ... grab the second child value ...
            var secondChild = ParentNode.Nodes.FirstOrDefault();
            // ... if there isnt, we add a new node ...
            var child = ParentNode.CreateChild(true, Order, nodeData?.ElementAtOrDefault(0));
            ParentNode.InsertNode(child, addAboveOrLeft);
            // ... and set the direction of the parent grid ...
            ParentNode.Rows = row;
            // ... then set the node widths if it needs to be overwritten ...
            if (widthOverwrite is not null)
                ParentNode.SetNodeWidths(widthOverwrite);
            // ... and let the user know the parent was modified ...
            return new(true, null, new(this, ParentNode, child, secondChild, null, addAboveOrLeft));
        }
        // ... otherwise, we need to check if we can add to the parent ...
        else
        {
            // ... by comparing the node directions ...
            if (ParentNode.Rows == row)
            {
                // ... we get a child node ...
                var child = ParentNode.CreateChild(false, Order, nodeData?.ElementAtOrDefault(0));

                // ... if we can add, add it ...
                ParentNode.InsertNode(child, addAboveOrLeft);
                // ... then set the node widths if it needs to be overwritten ...
                if (widthOverwrite is not null)
                    ParentNode.SetNodeWidths(widthOverwrite);
                // ... then let the user know.
                return new(true, null, new(this, ParentNode, child, null, null, addAboveOrLeft));
            }
            // ... otherwise, we need to do some fancy work,
            // but only if there are no child nodes yet ...
            else if (Nodes.Count <= 0)
            {
                // ... first, lets make a new child node that contains the information from this node ...
                var node = CreateChild(false, 0, nodeData?.ElementAtOrDefault(1));

                // ... then save the node as a child ...
                Nodes.Add(node);

                // ... clear this objects node widths values ...
                SetNodeWidths("");

                // ... finally call add on this new node ...
                return node.AddNode(row, addAboveOrLeft, nodeData, widthOverwrite);
            }
            // ... we should not get to this point,
            // so something went wrong when adding the node ...
            else
            {
                throw new InvalidOperationException("Node addition failed, node was against the pattern and" +
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
                var prevSize = pair.Value.Order + (addAboveOrLeft ? 0 : -1);
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
                            sizes.Insert(pair.Value.Order, $"{halfTwo}%");

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
    /// <param name="mergeLeftOrUp">True if the system should replace <paramref name="node"/> with the node to its left or above it, 
    /// false if it should replace <paramref name="node"/> with the node to its right or below it. <br /><br />
    /// Does not have any effect when there is only one node to remove.</param>
    /// <returns>A <see cref="ActionResult"/> with a <see cref="LayoutNodeModifiedResult"/> if the operation
    /// was successful.</returns>
    internal ActionResult<LayoutNodeModifiedResult> DeleteNode(LayoutNode node, bool mergeLeftOrUp = true)
    {
        var res = Nodes.Remove(node);
        if (!res)
            return new(true, new List<string> { "Failed to remove the node." }, null);

        LayoutNode? secondRemoval = null;
        LayoutNode? replacedBy = null;
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
            secondRemoval = child;
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

            // ... then get the node that replaced this one for the result ...
            replacedBy = Nodes.FirstOrDefault(x => x.Order == (mergeLeftOrUp ? prevSize : prevSize - 1));
        }

        // ... then let the caller know this was a success
        return new(true, null, new(this, this, node, secondRemoval, replacedBy, mergeLeftOrUp));
    }

    /// <summary>
    /// Deletes this node.
    /// </summary>
    /// <param name="mergeLeftOrUp">True if the system should replace this node with the node to its left or above it, 
    /// false if it should replace this node with the node to its right or below it. <br /><br />
    /// Does not have any effect when there is only one node to remove.</param>
    /// <remarks>
    /// The top level parent node can not be deleted. If you want
    /// to remove that node, you need to delete the page itself.
    /// </remarks>
    /// <returns>A <see cref="ActionResult"/> with a <see cref="LayoutNodeModifiedResult"/> if the operation
    /// was successful.</returns>
    public ActionResult<LayoutNodeModifiedResult> DeleteNode(bool mergeLeftOrUp = true)
    {
        if(ParentNode is null)
        {
            // The parent node can not be deleted - delete
            // the page instead.
            return new(false, new List<string> { "No parent to delete from." }, null);
        }

        var res = ParentNode.DeleteNode(this, mergeLeftOrUp);
        if (res.GetResult(out var data, out _))
            data.CalledFrom = this;
        return res;
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
    private LayoutNode CreateChild(bool transferComponentData, int order, LayoutNode? nodeData = null)
    {
        // Make a new node ...
        LayoutNode node = new();

        // ... if the node data is not null ...
        if (nodeData is not null)
        {
            // ... then add it here ...
            node = nodeData;
        }

        // ... then set the defaults ...
        node.ParentNode = this;
        node.ParentNodeId = Key;
        node.Order = order;

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

    /// <summary>
    /// Moves upwards on the <see cref="LayoutNode"/> tree, starting from this node, until the <see cref="CustomPageSettings"/> object is found.
    /// </summary>
    /// <remarks>
    /// If a node tree is not fully loaded, specifically in the upwards direction, this method can fail to find the <see cref="CustomPageSettings"/>.
    /// </remarks>
    /// <param name="settings">The <see cref="CustomPageSettings"/> object for this node tree.</param>
    /// <returns>True if the node with a <see cref="CustomPageSettings"/> was found, false if it was not.</returns>
    public bool TryGetPageSettings([NotNullWhen(true)] out CustomPageSettings? settings)
    {
        var cur = this;
        settings = null;

        while (cur.PageSettings is null)
        {
            cur = cur.ParentNode;

            if (cur is null)
                return false;
        }

        settings = cur.PageSettings;
        return true;
    }
}
