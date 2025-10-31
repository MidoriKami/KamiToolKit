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

    public override unsafe string TexturePath {
        get => base.TexturePath;
        set {
            if (Path.IsPathRooted(value)) {
                LoadTextureFromFile(value);
            }
            else if (DalamudInterface.Instance.DataManager.FileExists(value)) {
                PartsList[0]->LoadTexture(value);
            }
        }
    }

    /// <summary>
    /// Takes ownership of passed in IDalamudTextureWrap, disposes texture when node is disposed.
    /// </summary>
    public unsafe void LoadTexture(IDalamudTextureWrap texture) {
        var previouslyLoadedTexture = LoadedTexture;

        PartsList[0]->LoadTexture(texture);

        // Delay unloading texture until new texture is loaded.
        previouslyLoadedTexture?.Dispose();
        LoadedTexture = texture;
    }

    public void LoadTextureFromFile(string fileSystemPath) {
        DalamudInterface.Instance.Framework.RunOnTick(async () => {
            Alpha = 0.0f;

            var newTexture = await DalamudInterface.Instance.TextureProvider.GetFromFile(fileSystemPath).RentAsync();

            LoadTexture(newTexture);
            TextureSize = newTexture.Size;

            Alpha = 1.0f;
            MarkDirty();
        });
    }

    // Note, disposes loaded IDalamudTextureWrap if either native or managed code frees this node.
    protected override void Dispose(bool disposing, bool isNativeDestructor) {
        if (disposing) {
            base.Dispose(disposing, isNativeDestructor);

            LoadedTexture?.Dispose();
            LoadedTexture = null;
        }
    }
}
