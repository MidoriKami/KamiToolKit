using System;
using System.Globalization;
using System.Numerics;
using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Interfaces;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Nodes;

/// <summary>
/// Node representing a color preview and a hex string representation of the color.
/// </summary>
public class ColorPreviewWithInput : ResNode, IColorPreviewWithInputNode {

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public ColorSquareNode ColorSquareNode { get; }

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public TextInputNode ColorInputNode { get; }

    /// <inheritdoc />
    public Action<ColorHelpers.HsvaColor>? OnHsvaColorChanged { get; set; }

    /// <inheritdoc />
    public Action<Vector4>? OnColorChanged { get; set; }

    /// <inheritdoc />
    public ReadOnlySeString String {
        get => ColorInputNode.String;
        set => ColorInputNode.String = value;
    }

    /// <inheritdoc />
    public override Vector4 Color {
        get => ColorSquareNode.Color;
        set {
            ColorSquareNode.Color = value;
            UpdateColorText();
        }
    }

    /// <inheritdoc />
    public override ColorHelpers.HsvaColor ColorHsva {
        get => ColorSquareNode.ColorHsva;
        set {
            ColorSquareNode.ColorHsva = value;
            UpdateColorText();
        }
    }

    /// <summary>
    /// Constructs a <see cref="ColorPreviewWithInput"/> instance.
    /// </summary>
    public ColorPreviewWithInput() {
        ColorSquareNode = new ColorSquareNode();
        ColorSquareNode.AttachNode(this);

        ColorInputNode = new TextInputNode {
            AutoSelectAll = true,
            OnInputComplete = OnTextInputComplete,
        };
        ColorInputNode.AttachNode(this);
    }

    /// <inheritdoc />
    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        ColorSquareNode.Size = new Vector2(Height, Height);
        ColorSquareNode.Position = Vector2.Zero;

        ColorInputNode.Size = new Vector2(Width - Height - 8.0f, Height - 2.0f);
        ColorInputNode.Position = new Vector2(Height + 8.0f, 1.0f);
    }


    private void OnTextInputComplete(ReadOnlySeString obj) {
        var str = obj.ToString();

        if (string.IsNullOrEmpty(str) || !str.StartsWith('#')) return;

        var hexString = str.TrimStart('#');

        // Allow #RRGGBB and #RRGGBBAA only
        if (hexString.Length != 6 && hexString.Length != 8) return;

        const NumberStyles style = NumberStyles.HexNumber;
        var culture = CultureInfo.InvariantCulture;

        if (!byte.TryParse(hexString[0..2], style, culture, out var r)) return;
        if (!byte.TryParse(hexString[2..4], style, culture, out var g)) return;
        if (!byte.TryParse(hexString[4..6], style, culture, out var b)) return;

        byte a = 255;
        if (hexString.Length == 8 && !byte.TryParse(hexString[6..8], style, culture, out a)) return;

        var newColor = new Vector4(r / 255.0f, g / 255.0f, b / 255.0f, a / 255.0f);

        Color = newColor;
        OnColorChanged?.Invoke(newColor);
        OnHsvaColorChanged?.Invoke(ColorHelpers.RgbaToHsv(newColor));
    }

    private void UpdateColorText()
        => ColorInputNode.String = $"#{(int)(Color.X * 255):X2}{(int)(Color.Y * 255):X2}{(int)(Color.Z * 255):X2}{(int)(Color.W * 255):X2}";
}
