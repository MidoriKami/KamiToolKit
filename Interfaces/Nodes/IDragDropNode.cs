namespace KamiToolKit.Interfaces;

/// <summary>
/// Interface representing a DragDropNode.
/// </summary>
public interface IDragDropNode : IIcon, IDragDroppable, IQuantityDisplay {

    /// <summary>
    /// Gets or sets the visibility of the background frame.
    /// </summary>
    bool IsBackgroundShown { get; set; }
}
