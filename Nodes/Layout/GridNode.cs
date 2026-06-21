using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using KamiToolKit.Classes;

namespace KamiToolKit.Nodes;

/// <summary>
/// A layout node representing a grid.
/// </summary>
public class GridNode : ResNode {

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

    /// <summary>
    /// Gets or sets the grids size.
    /// </summary>
    /// <remarks>
    /// Warning: Changing this value will dispose any existing layout nodes.
    /// </remarks>
    public required GridSize GridSize {
        get;
        set {
            field = value;
            ReallocateArray();
        }
    } = new(0, 0);

    /// <summary>
    /// Recalculates all the nodes sizes and positions.
    /// </summary>
    public void RecalculateLayout() {
        var gridWidth = Width / GridSize.Columns;
        var gridHeight = Height / GridSize.Rows;

        foreach (var row in Enumerable.Range(0, GridSize.Rows)) {
            foreach (var column in Enumerable.Range(0, GridSize.Columns)) {
                this[column, row].Size = new Vector2(gridWidth, gridHeight);
                this[column, row].Position = new Vector2(column * gridWidth, row * gridHeight);
            }
        }
    }

    /// <inheritdoc />
    protected override void OnSizeChanged() {
        base.OnSizeChanged();
        RecalculateLayout();
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
                this[column, row].AttachNode(this);
                this[column, row].IsVisible = true;
            }
        }

        RecalculateLayout();
    }

    private readonly List<ResNode> gridNodes = [];

}
