using System.Threading.Tasks;
using Dalamud.Interface.Textures.TextureWraps;
using KamiToolKit.Classes;
using KamiToolKit.NodeParts;

namespace KamiToolKit.Nodes.Image;

/// <summary>
/// A simple image node that allows you to load an IDalamudTextureWrap texture into a native image node.
/// This node creates a single <see cref="Part"/>
/// </summary>
/// <remarks>This node is not intended to be used with multiple <see cref="Part"/>'s.</remarks>
public class ImGuiImageNode : SimpleImageNode {
    public void LoadTexture(IDalamudTextureWrap texture) 
        => PartsList[0].LoadTexture(texture);

    private IDalamudTextureWrap? loadedTexture;

    public void LoadTextureFromFile(string fileSystemPath) {
        Task.Run(() => {
            loadedTexture = DalamudInterface.Instance.TextureProvider.GetFromFile(fileSystemPath).RentAsync().Result;
            LoadTexture(loadedTexture);
        });
    }

    protected override void Dispose(bool disposing) {
        if (disposing) {
            loadedTexture?.Dispose();
            
            base.Dispose(disposing);
        }
    }
}