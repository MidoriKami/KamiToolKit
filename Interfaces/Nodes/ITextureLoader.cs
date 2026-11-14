namespace KamiToolKit.Interfaces;

/// <summary>
/// Interface representing a node that can load textures and icons from game resources.
/// </summary>
public interface ITextureLoader {

    /// <summary>
    /// Loads a texture from the given path, optionally with theme resolution disabled.
    /// </summary>
    /// <param name="path">The texture path to load.</param>
    /// <param name="resolveTheme">Whether to resolve theme-based paths. Defaults to true.</param>
    void LoadTexture(string path, bool resolveTheme = true);

    /// <summary>
    /// Loads an icon by its ID directly.
    /// </summary>
    /// <param name="iconId">The icon ID to load.</param>
    void LoadIcon(uint iconId);
}
