using System.IO;
using System.Threading.Tasks;
using Dalamud.Interface.Textures.TextureWraps;
using KamiToolKit.Classes;
using KamiToolKit.Internal.Classes;
using KamiToolKit.Nodes.Simplified;

namespace KamiToolKit.Nodes;

/// <summary>
/// A simple image node that allows you to load an IDalamudTextureWrap texture into a native image node.
/// This node creates a single <see cref="Part" />.
/// </summary>
/// <remarks>This node is not intended to be used with multiple <see cref="Part"/>'s.</remarks>
public class ImGuiImageNode : SimpleImageNode {

    /// <summary>
    /// When set loads the texture from either the game or from disk.
    /// </summary>
    public override string TexturePath {
        get => base.TexturePath;
        set {

            // If path represents a file system path, we need to load via ImGui.
            if (Path.IsPathRooted(value)) {

                // Start by hiding the node.
                Alpha = 0.0f;

                // Load the texture as a task
                Task.Run(async () => {
                    var newTexture = await Services.TextureProvider.GetFromFile(value).RentAsync();
                    Services.Log.Verbose($"Loaded texture from file system: {value}");

                    // Once it's ready, load it into the node on the next frame.
                    await Services.Framework.Run(() => {
                        unsafe {
                            if (Node is not null) {
                                LoadTexture(newTexture);
                                TextureSize = newTexture.Size;
                                Alpha = 1.0f;
                                MarkDirty();
                            }
                        }
                    });
                });
            }

            // else, the path is a game file, and the game itself can do its loading magic.
            else if (Services.DataManager.FileExists(value)) {
                unsafe {
                    PartsList[0]->LoadTexture(value);
                }
            }
        }
    }

    /// <summary>
    /// Takes ownership of passed in IDalamudTextureWrap, node automatically disposes texture when node is disposed.
    /// </summary>
    /// <remarks>
    /// Don't try to share this texture across nodes.
    /// If you need to have the same texture for multiple nodes use <see cref="TexturePath"/>,
    /// or load one independent instance of <see cref="IDalamudTextureWrap"/> per node.
    /// </remarks>
    public unsafe void LoadTexture(IDalamudTextureWrap texture) {

        // Load new texture
        PartsList[0]->LoadTexture(texture);

        if (LoadedTexture is not null) {
            Services.Log.Verbose($"Disposing texture: {LoadedTexture} to load {texture}");
        }

        // Dispose any previously used texture
        LoadedTexture?.Dispose();

        // Track currently used texture
        LoadedTexture = texture;
    }

    protected override void Dispose(bool disposing, bool isNativeDestructor) {
        if (disposing) {
            base.Dispose(disposing, isNativeDestructor);

            if (LoadedTexture is not null) {
                Services.Log.Verbose($"Disposing texture: {LoadedTexture}");
            }

            LoadedTexture?.Dispose();
            LoadedTexture = null;
        }
    }

    private IDalamudTextureWrap? LoadedTexture { get; set; }
}
