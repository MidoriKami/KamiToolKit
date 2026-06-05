using System.IO;
using System.Threading.Tasks;
using Dalamud.Interface.Textures.TextureWraps;
using KamiToolKit.Classes;
using KamiToolKit.Classes.Internal;
using KamiToolKit.Nodes.Simplified;

namespace KamiToolKit.Nodes;

/// <summary>
/// A simple image node that allows you to load an IDalamudTextureWrap texture into a native image node.
/// This node creates a single <see cref="Part" /> todo: make this class suck less.
/// </summary>
/// <remarks>This node is not intended to be used with multiple <see cref="Part"/>'s.</remarks>
public class ImGuiImageNode : SimpleImageNode {

    /// <summary>
    /// Gets or sets the dalamud texture wrap used for displaying this image.
    /// </summary>
    public unsafe IDalamudTextureWrap? LoadedTexture {
        get;
        set {
            field = value;

            if (value is not null) {
                PartsList[0]->LoadTexture(value);
            }
            // else { todo: this
            //     PartsList[0]->UldAsset->AtkTexture.ReleaseTexture();
            // }
        }
    }

    /// <summary>
    /// When set loads the texture from either the game or from disk.
    /// </summary>
    public override unsafe string TexturePath {
        get => base.TexturePath;
        set {
            if (Path.IsPathRooted(value)) {
                LoadTextureFromFile(value);
            }
            else if (Services.DataManager.FileExists(value)) {
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

    /// <summary>
    /// Loads texture from filesystem.
    /// </summary>
    public void LoadTextureFromFile(string fileSystemPath) {
        Task.Run(async () => {
            Alpha = 0.0f;

            var newTexture = await Services.TextureProvider.GetFromFile(fileSystemPath).RentAsync();

            LoadTexture(newTexture);
            TextureSize = newTexture.Size;

            Alpha = 1.0f;
            MarkDirty();
        });
    }

    protected override void Dispose(bool disposing, bool isNativeDestructor) {
        if (disposing) {
            base.Dispose(disposing, isNativeDestructor);

            LoadedTexture?.Dispose();
            LoadedTexture = null;
        }
    }
}
