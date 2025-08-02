using System.IO;
using Dalamud.Interface.Textures.TextureWraps;
using KamiToolKit.Classes;
using KamiToolKit.NodeParts;

namespace KamiToolKit.Nodes;

/// <summary>
///     A simple image node that allows you to load an IDalamudTextureWrap texture into a native image node.
///     This node creates a single <see cref="Part" />
/// </summary>
/// <remarks>This node is not intended to be used with multiple <see cref="Part" />'s.</remarks>
public class ImGuiImageNode : SimpleImageNode {

    public IDalamudTextureWrap? LoadedTexture;

    public override string TexturePath {
        get => base.TexturePath;
        set {
            if (Path.IsPathRooted(value)) {
                LoadTextureFromFile(value);
            }
            else if (DalamudInterface.Instance.DataManager.FileExists(value)) {
                PartsList[0].LoadTexture(value);
            }
        }
    }

    public void LoadTexture(IDalamudTextureWrap texture)
        => PartsList[0].LoadTexture(texture);

    public void LoadTextureFromFile(string fileSystemPath) {
        DalamudInterface.Instance.Framework.RunOnTick(async () => {
            Alpha = 0.0f;

            LoadedTexture = await DalamudInterface.Instance.TextureProvider.GetFromFile(fileSystemPath).RentAsync();

            LoadTexture(LoadedTexture);
            TextureSize = LoadedTexture.Size;
            Alpha = 1.0f;
            MarkDirty();
        });
    }

    protected override void Dispose(bool disposing) {
        if (disposing) {
            LoadedTexture?.Dispose();

            base.Dispose(disposing);
        }
    }
}
