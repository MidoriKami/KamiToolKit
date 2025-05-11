using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Plugin.Services;
using KamiToolKit.Classes;

namespace KamiToolKit.Nodes;

/// <summary>
/// A simple image node that allows you to load an IDalamudTextureWrap texture into a native image node.
/// This node creates a single <see cref="Part"/>
/// </summary>
/// <remarks>This node is not intended to be used with multiple <see cref="Part"/>'s.</remarks>
public class ImGuiImageNode : SimpleImageNode {
    public void LoadTexture(ITextureProvider textureProvider, IDalamudTextureWrap texture) 
        => PartsList[0].LoadTexture(textureProvider, texture);
}