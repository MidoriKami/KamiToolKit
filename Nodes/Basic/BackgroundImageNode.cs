using System.Numerics;
using Dalamud.Interface;

namespace KamiToolKit.Nodes;

/// <summary>
///     A simple image node that makes it easy to display a single color.
/// </summary>
public unsafe class BackgroundImageNode : SimpleImageNode {
    public BackgroundImageNode() {
        FitTexture = true;
    }

    public new Vector4 Color {
        get => new(AddColor.X, AddColor.Y, AddColor.Z, ResNode->Color.A / 255.0f);
        set {
            ResNode->Color = new Vector4(0.0f, 0.0f, 0.0f, value.W).ToByteColor();
            AddColor = value.AsVector3Color();
        }
    }

    public new ColorHelpers.HsvaColor HsvaColor {
        get => ColorHelpers.RgbaToHsv(Color);
        set => Color = ColorHelpers.HsvToRgb(value);
    }
}
