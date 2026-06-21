using System;
using System.Numerics;
using Dalamud.Interface;
using KamiToolKit.Internal.Classes;

namespace KamiToolKit.Nodes;

/// <summary>
/// A Color Picker Node, includes Color Ring, Alpha Slider, and text color input.
/// </summary>
public class ColorPickerNode : ResNode {

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public ColorRingWithSquareNode ColorRingNode { get; }

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public AlphaBarNode AlphaBarNode { get; }

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public ColorPreviewWithInput ColorPreviewWithInput { get; }

    /// <summary>
    /// Gets the current HSVA color.
    /// </summary>
    public ColorHelpers.HsvaColor CurrentColor { get; private set; }

    /// <summary>
    /// Gets or sets the action that is called when the color is changed.
    /// </summary>
    public Action<ColorHelpers.HsvaColor>? ColorPreviewed { get; set; }

    /// <summary>
    /// Gets or sets the action that is called when the color is changed.
    /// </summary>
    public Action<Vector4>? RgbaColorPreviewed { get; set; }

    /// <summary>
    /// Sets the current color's alpha.
    /// </summary>
    /// <param name="alpha"></param>
    public void SetAlpha(float alpha) {
        CurrentColor = CurrentColor with { A = alpha };

        ColorPreviewWithInput.ColorHsva = CurrentColor;
        AlphaBarNode.ColorHsva = CurrentColor;

        RaisePreviewMaybe();
    }

    /// <summary>
    /// Set the current color's hue.
    /// </summary>
    public void SetHue(float hue) {
        CurrentColor = CurrentColor with { H = hue };

        ColorRingNode.RotationDegrees = hue * 360.0f;
        ColorRingNode.SelectorColor = CurrentColor;
        ColorRingNode.SquareColor = CurrentColor with { S = 1.0f, V = 1.0f };

        ColorPreviewWithInput.ColorHsva = CurrentColor;
        AlphaBarNode.ColorHsva = CurrentColor;

        RaisePreviewMaybe();
    }

    /// <summary>
    /// Set the current color's saturation.
    /// </summary>
    public void SetSaturation(float saturation) {
        CurrentColor = CurrentColor with { S = saturation };

        ColorPreviewWithInput.ColorHsva = CurrentColor;
        ColorRingNode.SelectorColor = CurrentColor;

        ColorRingNode.SquareColor = CurrentColor;
        ColorRingNode.SquareSaturationValue = CurrentColor;

        AlphaBarNode.ColorHsva = CurrentColor;

        RaisePreviewMaybe();
    }

    /// <summary>
    /// Set the current color's value.
    /// </summary>
    public void SetValue(float value) {
        CurrentColor = CurrentColor with { V = value };

        ColorPreviewWithInput.ColorHsva = CurrentColor;
        ColorRingNode.SelectorColor = CurrentColor;

        ColorRingNode.SquareColor = CurrentColor;
        ColorRingNode.SquareSaturationValue = CurrentColor;

        AlphaBarNode.ColorHsva = CurrentColor;

        RaisePreviewMaybe();
    }

    /// <summary>
    /// Set the current color as RGBA.
    /// </summary>
    public void SetColor(Vector4 color) {
        var converted = ColorHelpers.RgbaToHsv(color);

        using (BeginBatchUpdate()) {
            SetHue(converted.H);
            SetSaturation(converted.S);
            SetValue(converted.V);
            SetAlpha(converted.A);
        }
    }

    /// <summary>
    /// Set the current color as HSVA.
    /// </summary>
    /// <param name="color"></param>
    public void SetColor(ColorHelpers.HsvaColor color) {
        using (BeginBatchUpdate()) {
            SetHue(color.H);
            SetSaturation(color.S);
            SetValue(color.V);
            SetAlpha(color.A);
        }
    }

    /// <summary>
    /// Constructs a <see cref="ColorPickerNode"/> instance.
    /// </summary>
    public ColorPickerNode() {
        ColorRingNode = new ColorRingWithSquareNode {
            OnHueChanged = SetHue,
            OnSaturationChanged = SetSaturation,
            OnValueChanged = SetValue,
        };
        ColorRingNode.AttachNode(this);

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

    /// <inheritdoc />
    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        var mainWidgetWidth = Width * 3.0f / 4.0f;

        ColorRingNode.Size = new Vector2(mainWidgetWidth, mainWidgetWidth);

        AlphaBarNode.Size = new Vector2(Width / 16.0f, mainWidgetWidth - 60.0f);
        AlphaBarNode.Position = new Vector2(mainWidgetWidth + (Width - mainWidgetWidth) / 3.0f - AlphaBarNode.Width / 2.0f, 30.0f);

        ColorPreviewWithInput.Size = new Vector2(150.0f, 32.0f);
        ColorPreviewWithInput.Position = new Vector2(Width / 2.0f - 75.0f, ColorRingNode.Y + ColorRingNode.Height - 1.0f);
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

    private int batchDepth;
    private bool previewDirty;
}
