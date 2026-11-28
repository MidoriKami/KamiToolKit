using KamiToolKit.Core.Layout;
using KamiToolKit.UI.Nodes.Layout;

namespace KamiToolKit.Core.Interfaces.Nodes;

/// <summary>
/// Interface representing a grid layout node.
/// </summary>
public interface IGridNode : ILayoutNode {

    /// <summary>
    /// Gets or sets the grid size.
    /// </summary>
    GridSize GridSize { get; init; }
}
