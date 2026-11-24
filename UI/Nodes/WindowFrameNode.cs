using System;
using System.Numerics;
using KamiToolKit.Core.Enums;
using KamiToolKit.Core.Interfaces;
using KamiToolKit.UI.Nodes.Simplified;

namespace KamiToolKit.UI.Nodes;

/// <summary>
/// This node implements the nodes needed to visually represent a window frame.
/// </summary>
/// <remarks>
/// This node does not implement things like title, subtitle, or control buttons.
/// </remarks>
public class WindowFrameNode : ResNode, ILayoutFiller, IFocusable {

    /// <inheritdoc />
    public LayoutFill LayoutFill { get; set; } = LayoutFill.Both;

    /// <inheritdoc />
    public bool IsFocused {
        get => foregroundFrame.IsVisible;
        set => foregroundFrame.IsVisible = value;
    }

    /// <inheritdoc />
    public Action? OnFocused { get; set; }

    /// <inheritdoc />
    public Action? OnUnfocused { get; set; }

    /// <summary>
    /// Constructor for a <see cref="WindowFrameNode"/>.
    /// </summary>
    public WindowFrameNode() {
        backgroundFrame = new WindowBackgroundTextureNode(false) {
            Position = Vector2.Zero,
            Offsets = new Vector4(64.0f, 32.0f, 32.0f, 32.0f),
            PartsRenderType = 19,
        };
        backgroundFrame.AttachNode(this);

        backgroundGradient = new SimpleImageNode {
            WrapMode = WrapMode.Stretch,
            TexturePath = "ui/uld/WindowA_Gradation.tex",
            TextureCoordinates = new Vector2(6.0f, 2.0f),
            TextureSize = new Vector2(24.0f, 24.0f),
        };
        backgroundGradient.AttachNode(this);

        foregroundFrame = new WindowBackgroundTextureNode(true) {
            Position = Vector2.Zero,
            Offsets = new Vector4(64.0f, 32.0f, 32.0f, 32.0f),
            PartsRenderType = 19,
        };
        foregroundFrame.AttachNode(this);
    }

    /// <inheritdoc />
    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        backgroundFrame.Size = Size;
        foregroundFrame.Size = Size;

        backgroundGradient.Size = new Vector2(Width - 8.0f, Height - 16.0f);
        backgroundGradient.Position = new Vector2(4.0f, 4.0f);
    }

    private readonly WindowBackgroundTextureNode backgroundFrame;
    private readonly SimpleImageNode backgroundGradient;
    private readonly WindowBackgroundTextureNode foregroundFrame;
}
