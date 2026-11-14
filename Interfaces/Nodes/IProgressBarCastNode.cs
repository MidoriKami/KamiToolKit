using System.Numerics;

namespace KamiToolKit.Interfaces;

/// <summary>
/// Interface representing a progress bar cast node with border customization.
/// </summary>
public interface IProgressBarCastNode : IProgressNode {

    /// <summary>
    /// Gets or sets the border texture color.
    /// </summary>
    /// <remarks>
    /// Expects values between 0.0f and 1.0f.
    /// </remarks>
    Vector4 BorderColor { get; set; }

    /// <summary>
    /// Gets or sets whether the border should be visible.
    /// </summary>
    bool BorderVisible { get; set; }
}
