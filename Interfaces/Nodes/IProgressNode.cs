using System.Numerics;

namespace KamiToolKit.Interfaces;

/// <summary>
/// Interface representing a progress bar node.
/// </summary>
public interface IProgressNode {

    /// <summary>
    /// Gets or sets the current progress representation.
    /// </summary>
    /// <remarks>
    /// Expects values between 0.0f and 1.0f.
    /// </remarks>
    float Progress { get; set; }

    /// <summary>
    /// Gets or sets the bar's color.
    /// </summary>
    /// <remarks>
    /// Expects values between 0.0f and 1.0f.
    /// </remarks>
    Vector4 BarColor { get; set; }

    /// <summary>
    /// Gets or sets the background texture's color.
    /// </summary>
    /// <remarks>
    /// Expects values between 0.0f and 1.0f.
    /// </remarks>
    Vector4 BackgroundColor { get; set; }
}
