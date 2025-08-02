using System.Numerics;
using KamiToolKit.Extensions;
using Newtonsoft.Json;

namespace KamiToolKit.Nodes;

/// <summary>
///     A simple image node that makes it easy to display a single color.
/// </summary>
[JsonObject(MemberSerialization.OptIn)]
public unsafe class BackgroundImageNode : SimpleImageNode {
    [JsonProperty] public new Vector4 Color {
        get => new(AddColor.X, AddColor.Y, AddColor.Z, InternalResNode->Color.A / 255.0f);
        set {
            InternalResNode->Color = new Vector4(0.0f, 0.0f, 0.0f, value.W).ToByteColor();
            AddColor = value.AsVector3Color();
        }
    }
}
