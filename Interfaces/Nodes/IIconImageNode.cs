namespace KamiToolKit.Interfaces;

/// <summary>
/// Interface representing an image node that displays a game icon by ID.
/// </summary>
public interface IIconImageNode : ISingleTextureNode, ITextureLoader {

    /// <summary>
    /// Gets or sets the displayed icon by its ID.
    /// </summary>
    uint IconId { get; set; }

    /// <summary>
    /// Gets whether the texture is loaded and ready.
    /// </summary>
    bool IsTextureReady { get; }

    /// <summary>
    /// Gets the currently loaded icon ID, or null if not loaded.
    /// </summary>
    uint? LoadedIconId { get; }
}
