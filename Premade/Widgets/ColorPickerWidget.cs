using System;
using System.Numerics;
using Dalamud.Interface;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;
using KamiToolKit.Premade.Nodes;

namespace KamiToolKit.Premade.Widgets;

public class ColorPickerWidget : SimpleComponentNode {
    public readonly ColorRingWithSquareNode ColorPickerNode;
    public readonly AlphaBarNode AlphaBarNode;
    public readonly ColorPreviewWithInput ColorPreviewWithInput;

    public ColorHelpers.HsvaColor CurrentColor { get; private set; }

    public Action<ColorHelpers.HsvaColor>? ColorPreviewed;
    public Action<Vector4>? RgbaColorPreviewed;

    private int batchDepth;
    private bool previewDirty;

    public ColorPickerWidget() {
        ColorPickerNode = new ColorRingWithSquareNode {
            OnHueChanged = SetHue,
            OnSaturationChanged = SetSaturation,
            OnValueChanged = SetValue,
        };
        ColorPickerNode.AttachNode(this);

        AlphaBarNode = new AlphaBarNode {
            OnAlphaChanged = SetAlpha,
        };
        AlphaBarNode.AttachNode(this);

        ColorPreviewWithInput = new ColorPreviewWithInput {
            OnHsvaColorChanged = newColor => {
                using (BeginBatchUpdate()) {
                    SetHue(newColor.H);
                    SetSaturation(newColor.S);
                    SetValue(newColor.V);
                    SetAlpha(newColor.A);
                }
            },
        };
        ColorPreviewWithInput.AttachNode(this);

        CurrentColor = ColorHelpers.RgbaToHsv(new Vector4(1.0f, 0.0f, 0.0f, 1.0f));

        using (BeginBatchUpdate()) {
            SetHue(CurrentColor.H);
            SetSaturation(CurrentColor.S);
            SetValue(CurrentColor.V);
            SetAlpha(CurrentColor.A);
        }
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

    private IDisposable BeginBatchUpdate() {
        batchDepth++;
        return new BatchToken(this);
    }

    internal void EndBatchUpdate() {
        batchDepth--;
        if (batchDepth <= 0) {
            batchDepth = 0;

            if (previewDirty) {
                previewDirty = false;
                RaisePreview();
            }
        }
    }

    private void RaisePreviewMaybe() {
        if (batchDepth > 0) {
            previewDirty = true;
            return;
        }

        RaisePreview();
    }

    private void RaisePreview() {
        var hsva = CurrentColor;
        ColorPreviewed?.Invoke(hsva);
        RgbaColorPreviewed?.Invoke(ColorHelpers.HsvToRgb(hsva));
    }

    public void SetAlpha(float alpha) {
        CurrentColor = CurrentColor with { A = alpha };

        ColorPreviewWithInput.ColorHsva = CurrentColor;
        AlphaBarNode.ColorHsva = CurrentColor;

        RaisePreviewMaybe();
    }

    public void SetHue(float hue) {
        CurrentColor = CurrentColor with { H = hue };

        ColorPickerNode.RotationDegrees = hue * 360.0f;
        ColorPickerNode.SelectorColor = CurrentColor;
        ColorPickerNode.SquareColor = CurrentColor with { S = 1.0f, V = 1.0f };

        ColorPreviewWithInput.ColorHsva = CurrentColor;
        AlphaBarNode.ColorHsva = CurrentColor;

        RaisePreviewMaybe();
    }

    public void SetSaturation(float saturation) {
        CurrentColor = CurrentColor with { S = saturation };

        ColorPreviewWithInput.ColorHsva = CurrentColor;
        ColorPickerNode.SelectorColor = CurrentColor;

        ColorPickerNode.SquareColor = CurrentColor;
        ColorPickerNode.SquareSaturationValue = CurrentColor;

        AlphaBarNode.ColorHsva = CurrentColor;

        RaisePreviewMaybe();
    }

    public void SetValue(float value) {
        CurrentColor = CurrentColor with { V = value };

        ColorPreviewWithInput.ColorHsva = CurrentColor;
        ColorPickerNode.SelectorColor = CurrentColor;

        ColorPickerNode.SquareColor = CurrentColor;
        ColorPickerNode.SquareSaturationValue = CurrentColor;

        AlphaBarNode.ColorHsva = CurrentColor;

        RaisePreviewMaybe();
    }

    public void SetColor(Vector4 color) {
        var converted = ColorHelpers.RgbaToHsv(color);

        using (BeginBatchUpdate()) {
            SetHue(converted.H);
            SetSaturation(converted.S);
            SetValue(converted.V);
            SetAlpha(converted.A);
        }
    }

    public void SetColor(ColorHelpers.HsvaColor color) {
        using (BeginBatchUpdate()) {
            SetHue(color.H);
            SetSaturation(color.S);
            SetValue(color.V);
            SetAlpha(color.A);
        }
    }
}
