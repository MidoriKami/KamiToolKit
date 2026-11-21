using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using KamiToolKit.Classes;
using KamiToolKit.Interfaces;

namespace KamiToolKit.Nodes;

/// <summary>
/// A layout node representing a grid.
/// </summary>
public class GridNode : LayoutNodeBase, IGridNode {

    /// <summary>
    /// Gets the grid section at the specified X,Y coordinate.
    /// </summary>
    public ResNode this[int x, int y]
        => gridNodes[x + y * GridSize.Columns];

    /// <summary>
    /// Gets the grid section at the specified index.
    /// </summary>
    public ResNode this[int index]
        => gridNodes[index];

    /// <inheritdoc />
    public required GridSize GridSize {
        get;
        set {
            field = value;
            ReallocateArray();
        }
    } = new(0, 0);

    /// <inheritdoc />
    protected override void DoUpdateLayout() {
        var gridWidth = Width / GridSize.Columns;
        var gridHeight = Height / GridSize.Rows;

        foreach (var row in Enumerable.Range(0, GridSize.Rows)) {
            foreach (var column in Enumerable.Range(0, GridSize.Columns)) {
                this[column, row].Size = new Vector2(gridWidth, gridHeight);
                this[column, row].Position = new Vector2(column * gridWidth, row * gridHeight);
            }
        }
    }

    private void ReallocateArray() {
        foreach (var node in gridNodes) {
            node.Dispose();
        }
        gridNodes.Clear();

        foreach (var _ in Enumerable.Range(0, GridSize.Rows * GridSize.Columns)) {
            gridNodes.Add(new ResNode());
        }

        foreach (var row in Enumerable.Range(0, GridSize.Rows)) {
            foreach (var column in Enumerable.Range(0, GridSize.Columns)) {
                var node = this[column, row];

                node.AttachNode(this);
                node.IsVisible = true;
            }
        }

        UpdateLayout();
    }

    private readonly List<ResNode> gridNodes = [];

}
