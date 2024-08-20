using Dalamud.Interface.Textures.TextureWraps;
using KamiToolKit.Nodes.Parts;

namespace KamiToolKit.Nodes;

/// <summary>
/// A simple image node that allows you to load an IDalamudTextureWrap texture into a native image node.
/// This node creates a single <see cref="Part"/>
/// </summary>
/// <remarks>This node is not intended to be used with multiple <see cref="Part"/>'s.</remarks>
public class ImGuiImageNode : SimpleImageNode {
    
    /// <summary>
    /// Loads a IDalamudTextureWrap into the native node.
    /// It is assumed that the lifespan of the <see cref="IDalamudTextureWrap"/> will exceed that of this ImGuiImageNode.
    /// </summary>
    /// <remarks>Only allows loading the texture once, and does not allow changing the texture.</remarks>
    public void LoadImGuiTexture(IDalamudTextureWrap textureWrap) {
        PartsList[0].LoadImGuiTexture(textureWrap);
    }
}