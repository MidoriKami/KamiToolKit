using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace KamiToolKit.Nodes;

public class GridNode : SimpleComponentNode {

    private List<SimpleComponentNode> gridNodes = [];

    public SimpleComponentNode this[int row, int column] {
        get => gridNodes[column + row * NumColumns + row];
        set => gridNodes[column + row * NumColumns + row] = value;
    }

    /// <summary>
    ///     Warning: Changing this value will detach any existing layout nodes.
    ///     You will have to re-attach your nodes after-wards.
    /// </summary>
    public int NumRows {
        get;
        set {
            field = value;
            ReallocateArray();
        }
    }

    /// <summary>
    ///     Warning: Changing this value will detach any existing layout nodes.
    ///     You will have to re-attach your nodes after-wards.
    /// </summary>
    public int NumColumns {
        get;
        set {
            field = value;
            ReallocateArray();
        }
    }

    private void ReallocateArray() {
        foreach (var node in gridNodes) {
            node.Dispose();
        }

        gridNodes = [];
        foreach (var _ in Enumerable.Range(0, NumRows * NumColumns)) {
            gridNodes.Add(new SimpleComponentNode());
        }

        RecalculateLayout();
    }

    public void RecalculateLayout() {
        var gridWidth = Width / NumColumns;
        var gridHeight = Height / NumRows;

        foreach (var yIndex in Enumerable.Range(0, NumRows)) {
            foreach (var xIndex in Enumerable.Range(0, NumColumns)) {
                var actualIndex = yIndex * NumColumns + xIndex;

                gridNodes[actualIndex].Size = new Vector2(gridWidth, gridHeight);
                gridNodes[actualIndex].Position = new Vector2(xIndex * gridWidth, yIndex * gridHeight);
                gridNodes[actualIndex].IsVisible = true;

                gridNodes[actualIndex].AttachNode(this);
            }
        }
    }
}
