using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace KamiToolKit.Nodes;

public record GridSize(int Columns, int Rows);

public class GridNode : SimpleComponentNode {

    private List<SimpleComponentNode> gridNodes = [];

    public SimpleComponentNode this[int x, int y] {
        get => gridNodes[x + y * GridSize.Columns];
        set => gridNodes[x + y * GridSize.Columns] = value;
    }

    public SimpleComponentNode this[int index] {
        get => gridNodes[index];
        set => gridNodes[index] = value;
    }

    /// <summary>
    ///     Warning: Changing this value will dispose any existing layout nodes.
    /// </summary>
    public required GridSize GridSize {
        get;
        set {
            field = value;
            ReallocateArray();
        }
    } = new(0, 0);

    private void ReallocateArray() {
        foreach (var node in gridNodes) {
            node.Dispose();
        }
        gridNodes.Clear();

        foreach (var _ in Enumerable.Range(0, GridSize.Rows * GridSize.Columns)) {
            gridNodes.Add(new SimpleComponentNode());
        }

        RecalculateLayout();
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();
        RecalculateLayout();
    }

    public void RecalculateLayout() {
        var gridWidth = Width / GridSize.Columns;
        var gridHeight = Height / GridSize.Rows;

        foreach (var row in Enumerable.Range(0, GridSize.Rows)) {
            foreach (var column in Enumerable.Range(0, GridSize.Columns)) {
                this[column, row].Size = new Vector2(gridWidth, gridHeight);
                this[column, row].Position = new Vector2(column * gridWidth, row * gridHeight);
                this[column, row].IsVisible = true;

                this[column, row].AttachNode(this);
            }
        }
    }
}
