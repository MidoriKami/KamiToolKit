using System.Numerics;
using Dalamud.Interface;
using KamiToolKit.Nodes.Simplified;

namespace KamiToolKit.Nodes;

/// <summary>
/// Specialized implementation of an <see cref="SimpleImageNode"/> that represents a single solid color.
/// </summary>
public unsafe class ColorImageNode : SimpleImageNode {

    /// <summary>
    /// The color this node should show.
    /// </summary>
    /// <remarks>
    /// Does some funny business with AddColor and Color to make the node appear the desired color.
    /// </remarks>
    public new Vector4 Color {
        get => new(AddColor.X, AddColor.Y, AddColor.Z, ResNode->Color.A / 255.0f);
        set {
            ResNode->Color = new Vector4(0.0f, 0.0f, 0.0f, value.W).ToByteColor();
            AddColor = value.AsVector3Color();
        }
    }

    /// <summary>
    /// The color this node should show.
    /// </summary>
    /// <remarks>
    /// Does some funny business with AddColor and Color to make the node appear the desired color.
    /// </remarks>
    public new ColorHelpers.HsvaColor ColorHsva {
        get => ColorHelpers.RgbaToHsv(Color);
        set => Color = ColorHelpers.HsvToRgb(value);
    }

    public ColorImageNode() {
        FitTexture = true;
    }
}
