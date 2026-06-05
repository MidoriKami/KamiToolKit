using System.Numerics;
using KamiToolKit.Premade.Node.Simple;

namespace KamiToolKit.Nodes;

/// <summary>
/// Custom implementation of a progress bar node.
/// </summary>
public abstract class ProgressNode : SimpleComponentNode {

    /// <summary>
    /// Gets or sets the current progress representation.
    /// </summary>
    /// <remarks>
    /// Expects values between 0.0f and 1.0f.
    /// </remarks>
    public abstract float Progress { get; set; }

    /// <summary>
    /// Gets or sets the bars color.
    /// </summary>
    /// <remarks>
    /// Expects values between 0.0f and 1.0f.
    /// </remarks>
    public abstract Vector4 BarColor { get; set; }

    /// <summary>
    /// Gets or sets the background textures color.
    /// </summary>
    /// <remarks>
    /// Expects values between 0.0f and 1.0f.
    /// </remarks>
    public abstract Vector4 BackgroundColor { get; set; }
}
