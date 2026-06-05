using System.Numerics;
using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes.Internal;
using KamiToolKit.Nodes;

namespace KamiToolKit.Premade.Node.Color;

/// <summary>
/// Square texture node representing a triple color gradient for use in <see cref="Premade.Addon.ColorPickerAddon"/>.
/// Not intended for external use.
/// </summary>
public class ColorGradientSquare : ResNode {

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public ImGuiImageNode WhiteGradientNode { get; }

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public ImGuiImageNode ColorGradientNode { get; }

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public ImGuiImageNode BlackGradientNode { get; }

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public ImGuiImageNode ColorDotNode { get; }

    /// <summary>
    /// Gets or sets the color for this node.
    /// </summary>
    public override ColorHelpers.HsvaColor MultiplyColorHsva {
        get => ColorGradientNode.MultiplyColorHsva;
        set => ColorGradientNode.MultiplyColorHsva = value;
    }

    /// <summary>
    /// Gets or sets the position of the circle indicator node.
    /// </summary>
    public Vector2 ColorDotPosition {
        get => ColorDotNode.Position + ColorDotNode.Origin;
        set => ColorDotNode.Position = value - ColorDotNode.Origin;
    }

    public ColorGradientSquare() {
        WhiteGradientNode = new ImGuiImageNode {
            TexturePath = Services.GetAssetPath("HorizontalGradient_WhiteToAlpha.png"),
            FitTexture = true,
        };
        WhiteGradientNode.AttachNode(this);

        ColorGradientNode = new ImGuiImageNode {
            TexturePath = Services.GetAssetPath("HorizontalGradient_WhiteToAlpha.png"),
            FitTexture = true,
            ImageNodeFlags = ImageNodeFlags.FlipH,
        };
        ColorGradientNode.AttachNode(this);

        BlackGradientNode = new ImGuiImageNode {
            TexturePath = Services.GetAssetPath("VerticalGradient_AlphaToBlack.png"),
            FitTexture = true,
        };
        BlackGradientNode.AttachNode(this);

        ColorDotNode = new ImGuiImageNode {
            TexturePath = Services.GetAssetPath("color_select_dot.png"),
            FitTexture = true,
        };
        ColorDotNode.AttachNode(this);
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        WhiteGradientNode.Size = Size;
        ColorGradientNode.Size = Size;
        BlackGradientNode.Size = Size;

        Origin = Size / 2.0f;

        ColorDotNode.Size = new Vector2(16.0f, 16.0f);
        ColorDotNode.Origin = ColorDotNode.Size / 2.0f;
        ColorDotNode.Position = new Vector2(Width, 0.0f) - ColorDotNode.Origin;
    }
}
