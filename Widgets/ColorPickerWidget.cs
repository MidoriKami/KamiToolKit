using System.Numerics;
using Dalamud.Interface;
using KamiToolKit.Nodes;
using KamiToolKit.Widgets.Parts;

namespace KamiToolKit.Widgets;

public class ColorPickerWidget : SimpleComponentNode {
    public readonly ColorRingWithSquareNode ColorPickerNode;
    public readonly AlphaBarNode AlphaBarNode;
    public readonly ColorPreviewWithInput ColorPreviewWithInput;

    public ColorHelpers.HsvaColor CurrentColor { get; set; }
    
    public ColorPickerWidget() {
        ColorPickerNode = new ColorRingWithSquareNode {
            IsVisible = true,
            OnHueChanged = SetHue,
            OnSaturationChanged = SetSaturation,
            OnValueChanged = SetValue,
        };
        ColorPickerNode.AttachNode(this);

        AlphaBarNode = new AlphaBarNode {
            IsVisible = true,
            OnAlphaChanged = SetAlpha,
        };
        AlphaBarNode.AttachNode(this);
        
        ColorPreviewWithInput = new ColorPreviewWithInput {
            IsVisible = true,
            OnHsvaColorChanged = newColor => {
                SetHue(newColor.H);
                SetSaturation(newColor.S);
                SetValue(newColor.V);
                SetAlpha(newColor.A);
            },
        };
        ColorPreviewWithInput.AttachNode(this);

        CurrentColor = ColorHelpers.RgbaToHsv(new Vector4(1.0f, 0.0f, 0.0f, 1.0f));
        SetHue(CurrentColor.H);
        SetSaturation(CurrentColor.S);
        SetValue(CurrentColor.V);
        SetAlpha(CurrentColor.A);
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        var mainWidgetWidth = Width * 3.0f / 4.0f;
        
        ColorPickerNode.Size = new Vector2(mainWidgetWidth, mainWidgetWidth);

        AlphaBarNode.Size = new Vector2(Width / 16.0f, mainWidgetWidth - 60.0f);
        AlphaBarNode.Position = new Vector2(mainWidgetWidth + (Width - mainWidgetWidth) / 3.0f - AlphaBarNode.Width / 2.0f, 30.0f);

        ColorPreviewWithInput.Size = new Vector2(150.0f, 32.0f);
        ColorPreviewWithInput.Position = new Vector2(Width / 2.0f - 75.0f, ColorPickerNode.Y + ColorPickerNode.Height - 1.0f);
    }

    public void SetAlpha(float alpha) {
        CurrentColor = CurrentColor with { A = alpha };

        ColorPreviewWithInput.HsvaColor = CurrentColor;
        AlphaBarNode.HsvaColor = CurrentColor;
    }

    public void SetHue(float hue) {
        CurrentColor = CurrentColor with { H = hue };

        ColorPickerNode.RotationDegrees = hue * 360.0f;
        ColorPickerNode.SelectorColor = CurrentColor;
        ColorPickerNode.SquareColor = CurrentColor with { S = 1.0f, V = 1.0f };

        ColorPreviewWithInput.HsvaColor = CurrentColor;

        AlphaBarNode.HsvaColor = CurrentColor;
    }

    public void SetSaturation(float saturation) {
        CurrentColor = CurrentColor with { S = saturation };
        
        ColorPreviewWithInput.HsvaColor = CurrentColor;
        ColorPickerNode.SelectorColor = CurrentColor;
        
        AlphaBarNode.HsvaColor = CurrentColor;
    }

    public void SetValue(float value) {
        CurrentColor = CurrentColor with { V = value };

        ColorPreviewWithInput.HsvaColor = CurrentColor;
        ColorPickerNode.SelectorColor = CurrentColor;

        AlphaBarNode.HsvaColor = CurrentColor;
    }
}
