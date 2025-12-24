using System;
using System.Globalization;
using System.Numerics;
using Dalamud.Interface;
using KamiToolKit.Nodes;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Premade.Nodes;

public class ColorPreviewWithInput : SimpleComponentNode {
    public readonly ColorPreviewNode ColorPreviewNode;
    public readonly TextInputNode ColorInputNode;

    public ColorPreviewWithInput() {
        ColorPreviewNode = new ColorPreviewNode();
        ColorPreviewNode.AttachNode(this);

        ColorInputNode = new TextInputNode {
            AutoSelectAll = true,
            OnInputComplete = OnTextInputComplete,
        };
        ColorInputNode.AttachNode(this);
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        ColorPreviewNode.Size = new Vector2(Height, Height);
        ColorPreviewNode.Position = Vector2.Zero;

        ColorInputNode.Size = new Vector2(Width - Height - 8.0f, Height - 2.0f);
        ColorInputNode.Position = new Vector2(Height + 8.0f, 1.0f);
    }

    public Action<ColorHelpers.HsvaColor>? OnHsvaColorChanged { get; set; }
    public Action<Vector4>? OnColorChanged { get; set; }

    public string String {
        get => ColorInputNode.String;
        set => ColorInputNode.String = value;
    }

    public override Vector4 Color {
        get => ColorPreviewNode.Color;
        set {
            ColorPreviewNode.Color = value;
            UpdateColorText();
        }
    }

    public override ColorHelpers.HsvaColor HsvaColor {
        get => ColorPreviewNode.HsvaColor;
        set {
            ColorPreviewNode.HsvaColor = value;
            UpdateColorText();
        }
    }

    private void OnTextInputComplete(ReadOnlySeString obj) {
        ReadOnlySpan<char> s = obj.ToString().AsSpan();

        if (s.Length == 0)
            return;

        if (s[0] == '#')
            s = s[1..];

        if (s.Length != 8)
            return;

        const NumberStyles style = NumberStyles.AllowHexSpecifier;

        if (!byte.TryParse(s[..2], style, CultureInfo.InvariantCulture, out byte r) ||
            !byte.TryParse(s.Slice(2, 2), style, CultureInfo.InvariantCulture, out byte g) ||
            !byte.TryParse(s.Slice(4, 2), style, CultureInfo.InvariantCulture, out byte b) ||
            !byte.TryParse(s.Slice(6, 2), style, CultureInfo.InvariantCulture, out byte a)) {
            return;
        }

        const float inv255 = 1f / 255f;
        var newColor = new Vector4(r * inv255, g * inv255, b * inv255, a * inv255);

        Color = newColor;
        OnColorChanged?.Invoke(newColor);
        OnHsvaColorChanged?.Invoke(ColorHelpers.RgbaToHsv(newColor));
    }

    private void UpdateColorText()
        => ColorInputNode.String = $"#{(int)(Color.X * 255):X2}{(int)(Color.Y * 255):X2}{(int)(Color.Z * 255):X2}{(int)(Color.W * 255):X2}";
}
