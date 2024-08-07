using System.Numerics;
using Dalamud.Utility.Numerics;
using KamiToolKit.Extensions;

namespace KamiToolKit.Nodes;

/// <summary>
/// A simple image node that makes it easy to display a single color.
/// </summary>
public unsafe class BackgroundImageNode : SimpleImageNode {
    public new Vector4 Color {
        get => new(AddColor.X, AddColor.Y, AddColor.Z, InternalResNode->Color.A);
        set {
            InternalResNode->Color = new Vector4(0.0f, 0.0f, 0.0f, value.W).ToByteColor();
            AddColor = value.AsVector3Color();
        }
    }
}