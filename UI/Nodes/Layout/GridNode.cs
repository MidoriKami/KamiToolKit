using System;
using System.Numerics;
using KamiToolKit.Core.Interfaces.Nodes;
using KamiToolKit.Core.Layout;
using KamiToolKit.Core.Primitives;

namespace KamiToolKit.UI.Nodes.Layout;

/// <summary>
/// A layout node representing a grid.
/// </summary>
public class GridNode : LayoutNode, IGridNode {

    /// <summary>
    /// Gets the child at the specified column and row.
    /// </summary>
    public NodeBase this[int x, int y]
        => this[x + y * GridSize.Columns];

    /// <summary>
    /// Gets the child at the specified row-major index.
    /// </summary>
    public NodeBase this[int index]
        => NodeList[index];

    /// <inheritdoc />
    public required GridSize GridSize {
        get;
        init {
            if (value.Columns is 0 || value.Rows is 0) throw new IndexOutOfRangeException();

            field = value;
            UpdateLayout();
        }
    }

    /// <inheritdoc />
    protected override void OnUpdateLayout() {
        var columns = GridSize.Columns;
        var rows = GridSize.Rows;
        if (columns is 0 || rows is 0) return;

        var gridWidth = Width / columns;
        var gridHeight = Height / rows;

        for (var index = 0; index < NodeList.Count; index++) {
            var cell = this[index];
            var column = index % columns;
            var row = index / columns;
            cell.Size = new Vector2(gridWidth, gridHeight);
            cell.Position = new Vector2(column * gridWidth, row * gridHeight);
        }
    }

    /// <inheritdoc />
    protected override void OnNavigationUpdate(NavigationContext context) {
        // TODO: DO DIS YOU DOER
    }
}
