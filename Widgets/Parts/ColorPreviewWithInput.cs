using System;
using System.Globalization;
using System.Numerics;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Interface;
using KamiToolKit.Nodes;

namespace KamiToolKit.Widgets.Parts;

public class ColorPreviewWithInput : SimpleComponentNode {
    public readonly ColorPreviewNode ColorPreviewNode;
    public readonly TextInputNode ColorInputNode;

    public ColorPreviewWithInput() {
        ColorPreviewNode = new ColorPreviewNode {
            IsVisible = true,
        };
        ColorPreviewNode.AttachNode(this);
        
        ColorInputNode = new TextInputNode {
            IsVisible = true,
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

    private void OnTextInputComplete(SeString obj) {
        if (!obj.ToString().StartsWith('#')) return;

        var hexString = obj.ToString().TrimStart('#');

        var r = byte.Parse(hexString[0..2], NumberStyles.HexNumber);
        var g = byte.Parse(hexString[2..4], NumberStyles.HexNumber);
        var b = byte.Parse(hexString[4..6], NumberStyles.HexNumber);
        var a = byte.Parse(hexString[6..8], NumberStyles.HexNumber);

        var newColor = new Vector4(r / 255.0f, g / 255.0f, b / 255.0f, a / 255.0f);

        Color = newColor;
        OnColorChanged?.Invoke(newColor);
        OnHsvaColorChanged?.Invoke(ColorHelpers.RgbaToHsv(newColor));
    }
    
    private void UpdateColorText()
        => ColorInputNode.String = $"#{(int)(Color.X * 255):X2}{(int)(Color.Y * 255):X2}{(int)(Color.Z * 255):X2}{(int)(Color.W * 255):X2}";
}
