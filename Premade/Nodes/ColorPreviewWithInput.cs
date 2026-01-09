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

    public override ColorHelpers.HsvaColor ColorHsva {
        get => ColorPreviewNode.ColorHsva;
        set {
            ColorPreviewNode.ColorHsva = value;
            UpdateColorText();
        }
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
