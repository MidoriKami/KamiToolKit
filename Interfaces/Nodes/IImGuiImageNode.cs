using Dalamud.Interface.Textures.TextureWraps;

namespace KamiToolKit.Interfaces;

/// <summary>
/// Interface representing an image node that can load IDalamudTextureWrap textures.
/// </summary>
public interface IImGuiImageNode : ISingleTextureNode, ITextureLoader {

    /// <summary>
    /// Takes ownership of the passed IDalamudTextureWrap; the node disposes the texture when disposed.
    /// </summary>
    /// <param name="texture">The texture to load.</param>
    void LoadTexture(IDalamudTextureWrap texture);
}
