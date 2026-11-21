using KamiToolKit.Classes;

namespace KamiToolKit.Interfaces;

/// <summary>
/// Interface representing a grid layout node.
/// </summary>
public interface IGridNode : ILayoutUpdatable {

    /// <summary>
    /// Gets or sets the grid size.
    /// </summary>
    /// <remarks>
    /// Warning: Changing this value will dispose any existing layout nodes.
    /// </remarks>
    GridSize GridSize { get; set; }
}
