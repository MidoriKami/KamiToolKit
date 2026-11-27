namespace KamiToolKit.Core.Interfaces.Nodes;

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

    /// <summary>
    /// Loads a texture part via Uld part-set and part-id.
    /// </summary>
    /// <param name="uldPath">The path to the uld to load, should end in .uld</param>
    /// <param name="partId">The specific part id you want.</param>
    /// <param name="partSet">Which parts page you want to index into.</param>
    /// <param name="resolveTheme">If the loaded part should be themed.</param>
    void LoadUldPart(string uldPath, uint partId, uint partSet, bool resolveTheme = true);
}
