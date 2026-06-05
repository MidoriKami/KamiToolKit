using System.Drawing;
using System.Numerics;
using Dalamud.Interface;
using KamiToolKit.Classes.Internal;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;

namespace KamiToolKit.Premade.Node.Color;

/// <summary>
/// Square Image Node with Alpha background texture to represent a color.
/// </summary>
public class ColorSquareNode : ResNode {

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public ColorImageNode SelectedColorPreviewNode { get; }

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public ImGuiImageNode AlphaLayerPreviewNode { get; }

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public ColorImageNode SelectedColorPreviewBorderNode { get; }

    /// <summary>
    /// Gets or sets the displayed color as RGBA.
    /// </summary>
    /// <remarks>
    /// Expects values between 0.0f and 1.0f.
    /// </remarks>
    public override Vector4 Color {
        get => SelectedColorPreviewNode.Color;
        set => SelectedColorPreviewNode.Color = value;
    }

    /// <summary>
    /// Gets or sets the displayed color as HSVA.
    /// </summary>
    public override ColorHelpers.HsvaColor ColorHsva {
        get => SelectedColorPreviewNode.ColorHsva;
        set => SelectedColorPreviewNode.ColorHsva = value;
    }

    public ColorSquareNode() {
        SelectedColorPreviewBorderNode = new ColorImageNode {
            Color = KnownColor.White.Vector(),
        };
        SelectedColorPreviewBorderNode.AttachNode(this);

        AlphaLayerPreviewNode = new ImGuiImageNode {
            TexturePath = Services.GetAssetPath("alpha_background.png"),
            WrapMode = WrapMode.Tile,
        };
        AlphaLayerPreviewNode.AttachNode(this);

        SelectedColorPreviewNode = new ColorImageNode {
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
}
