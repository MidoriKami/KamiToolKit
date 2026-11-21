using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Enums;

namespace KamiToolKit.Interfaces;

/// <summary>
/// Interface representing a base image node with parts and rendering options.
/// </summary>
public interface IImageNode : IPartsList {

    /// <summary>
    /// Gets or sets the wrap mode for the displayed image.
    /// </summary>
    WrapMode WrapMode { get; set; }

    /// <summary>
    /// Gets or sets the image node flags.
    /// </summary>
    ImageNodeFlags ImageNodeFlags { get; set; }

    /// <summary>
    /// When set to true, causes the loaded texture to fit itself to the size of the node.
    /// </summary>
    bool FitTexture { set; }
}
