using System.Drawing;
using System.Numerics;
using Dalamud.Interface;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;

namespace KamiToolKit.Premade.Nodes;

public class ColorPreviewNode : SimpleComponentNode {
    public readonly BackgroundImageNode SelectedColorPreviewNode;
    public readonly ImGuiImageNode AlphaLayerPreviewNode;
    public readonly BackgroundImageNode SelectedColorPreviewBorderNode;

    public ColorPreviewNode() {
        SelectedColorPreviewBorderNode = new BackgroundImageNode {
            Color = KnownColor.White.Vector(),
        };
        SelectedColorPreviewBorderNode.AttachNode(this);

        AlphaLayerPreviewNode = new ImGuiImageNode {
            TexturePath = DalamudInterface.Instance.GetAssetPath("alpha_background.png"),
            WrapMode = WrapMode.Tile,
        };
        AlphaLayerPreviewNode.AttachNode(this);

        SelectedColorPreviewNode = new BackgroundImageNode {
            Color = new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
        };
        SelectedColorPreviewNode.AttachNode(this);
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        SelectedColorPreviewBorderNode.Size = new Vector2(Height - 4.0f, Width - 4.0f);
        SelectedColorPreviewBorderNode.Position = new Vector2(2.0f, 2.0f);
        
        AlphaLayerPreviewNode.Size = new Vector2(Height - 6.0f, Width - 6.0f);
        AlphaLayerPreviewNode.Position = new Vector2(3.0f, 3.0f);
        
        SelectedColorPreviewNode.Size = new Vector2(Height - 6.0f, Width - 6.0f);
        SelectedColorPreviewNode.Position = new Vector2(3.0f, 3.0f);
    }

    public override Vector4 Color {
        get => SelectedColorPreviewNode.Color;
        set => SelectedColorPreviewNode.Color = value;
    }

    public override ColorHelpers.HsvaColor HsvaColor {
        get => SelectedColorPreviewNode.HsvaColor;
        set => SelectedColorPreviewNode.HsvaColor = value;
    }
}
