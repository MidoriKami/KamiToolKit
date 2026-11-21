using KamiToolKit.Enums;

namespace KamiToolKit.Interfaces;

/// <summary>
/// Interface representing a circle button node.
/// </summary>
public interface ICircleButtonNode : IButtonNode {

    /// <summary>
    /// Gets or sets the icon displayed for this node.
    /// </summary>
    CircleButtonIcon Icon { get; set; }
}
