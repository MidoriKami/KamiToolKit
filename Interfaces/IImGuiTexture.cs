using Dalamud.Interface.Textures.TextureWraps;

namespace KamiToolKit.Interfaces;

/// <summary>
/// Interface representing properties of a ImGui Texture.
/// </summary>
public interface IImGuiTexture {

    /// <summary>
    /// Gets or sets the texture path for the image to display.
    /// Setting this will trigger it to be loaded from disk/game.
    /// </summary>
    string TexturePath { get; set; }

    /// <summary>
    /// Use a texture that has already been created.
    /// </summary>
    /// <remarks>
    /// This texture must continue to exist during the life of this node.
    /// </remarks>
    void LoadTexture(IDalamudTextureWrap texture);

    /// <summary>
    /// Loads a texture from the provided path, if the path is a game path will load from game files.
    /// Else the texture will be loaded from disk.
    /// </summary>
    void LoadTexture(string path);
}
