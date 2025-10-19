using System.Numerics;
using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;

namespace KamiToolKit.Widgets.Parts;

public class ColorSquareNode : SimpleComponentNode {
    public readonly ImGuiImageNode WhiteGradientNode;
    public readonly ImGuiImageNode ColorGradientNode;
    public readonly ImGuiImageNode BlackGradientNode;
    public readonly ImGuiImageNode ColorDotNode;

    public ColorSquareNode() {
        WhiteGradientNode = new ImGuiImageNode {
            IsVisible = true,
            TexturePath = DalamudInterface.Instance.GetAssetPath("HorizontalGradient_WhiteToAlpha.png"),
            FitTexture = true,
        };
        WhiteGradientNode.AttachNode(this);
        
        ColorGradientNode = new ImGuiImageNode {
            IsVisible = true,
            TexturePath = DalamudInterface.Instance.GetAssetPath("HorizontalGradient_WhiteToAlpha.png"),
            FitTexture = true,
            ImageNodeFlags = ImageNodeFlags.FlipH,
        };
        ColorGradientNode.AttachNode(this);

        BlackGradientNode = new ImGuiImageNode {
            IsVisible = true,
            TexturePath = DalamudInterface.Instance.GetAssetPath("VerticalGradient_AlphaToBlack.png"),
            FitTexture = true,
        };
        BlackGradientNode.AttachNode(this);
        
        ColorDotNode = new ImGuiImageNode {
            TexturePath = DalamudInterface.Instance.GetAssetPath("color_select_dot.png"),
            IsVisible = true,
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

    public override ColorHelpers.HsvaColor HsvaMultiplyColor {
        get => ColorGradientNode.HsvaMultiplyColor;
        set => ColorGradientNode.HsvaMultiplyColor = value;
    }

    public Vector2 ColorDotPosition {
        get => ColorDotNode.Position + ColorDotNode.Origin;
        set => ColorDotNode.Position = value - ColorDotNode.Origin;
    }
}
