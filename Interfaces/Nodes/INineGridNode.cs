using System.Numerics;

namespace KamiToolKit.Interfaces;

/// <summary>
/// Interface representing a texture node that stretches its parts to fit its content area.
/// </summary>
public interface INineGridNode : IPartsList {

    /// <summary>
    /// Gets or sets the parts render type.
    /// </summary>
    byte PartsRenderType { get; set; }

    /// <summary>
    /// Gets or sets the texture offsets used for fitting the ninegrid texture.
    /// </summary>
    Vector4 Offsets { get; set; }

    /// <summary>
    /// Gets or sets the top offset for the texture.
    /// </summary>
    float TopOffset { get; set; }

    /// <summary>
    /// Gets or sets the bottom offset for the texture.
    /// </summary>
    float BottomOffset { get; set; }

    /// <summary>
    /// Gets or sets the left offset for the texture.
    /// </summary>
    float LeftOffset { get; set; }

    /// <summary>
    /// Gets or sets the right offset for the texture.
    /// </summary>
    float RightOffset { get; set; }

    /// <summary>
    /// Gets or sets the blend mode.
    /// </summary>
    uint BlendMode { get; set; }
}
