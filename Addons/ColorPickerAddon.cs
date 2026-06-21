using System;
using System.Numerics;
using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.BaseTypes;
using KamiToolKit.Nodes;

namespace KamiToolKit.Addons;

/// <summary>
/// Prebuilt Color Picker window with color wheel, color gradiant square, and alpha siders.
/// </summary>
public class ColorPickerAddon : NativeAddon {

    /// <summary>
    /// Gets or sets the action that is called when the color is changed, but not confirmed or canceled.
    /// </summary>
    public Action<Vector4>? OnColorPreviewed { get; set; }

    /// <summary>
    /// Gets or sets the action that is called when the color is changed, but not confirmed or canceled.
    /// </summary>
    public Action<ColorHelpers.HsvaColor>? OnHsvaColorPreviewed { get; set; }

    /// <summary>
    /// Gets or sets the action that is called when the color is confirmed.
    /// </summary>
    public Action<Vector4>? OnColorConfirmed { get; set; }

    /// <summary>
    /// Gets or sets the action that is called when the color is confirmed.
    /// </summary>
    public Action<ColorHelpers.HsvaColor>? OnHsvaColorConfirmed { get; set; }

    /// <summary>
    /// Gets or sets an action to be called when the color is canceled.
    /// </summary>
    public Action? OnColorCancelled { get; set; }

    /// <summary>
    /// Gets or sets the default color as HSVA to reset to when 'Default' is clicked.
    /// </summary>
    public ColorHelpers.HsvaColor? DefaultHsvaColor { get; set; }

    /// <summary>
    /// Gets or sets the default color as RGBA to reset to when 'Default' is clicked.
    /// </summary>
    public Vector4? DefaultColor {
        get;
        set {
            field = value;
            DefaultHsvaColor = value is null ? null : ColorHelpers.RgbaToHsv(value.Value);
        }
    }

    /// <summary>
    /// Gets or sets the initial color when this window is opened.
    /// </summary>
    public Vector4 InitialColor {
        get => ColorHelpers.HsvToRgb(InitialHsvaColor);
        set => InitialHsvaColor = ColorHelpers.RgbaToHsv(value);
    }

    /// <summary>
    /// Gets or sets the initial color as HSVA when this window is opened.
    /// </summary>
    public ColorHelpers.HsvaColor InitialHsvaColor { get; set; } =
        ColorHelpers.RgbaToHsv(new Vector4(1.0f, 0.0f, 0.0f, 1.0f));

    /// <inheritdoc />
    protected override unsafe void OnSetup(AtkUnitBase* addon, Span<AtkValue> atkValueSpan) {
        SetWindowSize(new Vector2(400.0f, 425.0f));

        initialHsva = InitialHsvaColor;
        initialRgba = ColorHelpers.HsvToRgb(initialHsva);

        ColorPicker = new ColorPickerNode {
            Position = ContentStartPosition,
            Size = ContentSize,
        };
        ColorPicker.AttachNode(this);

        ColorPicker.ColorPreviewed += hsva => OnHsvaColorPreviewed?.Invoke(hsva);
        ColorPicker.RgbaColorPreviewed += rgba => OnColorPreviewed?.Invoke(rgba);

        ColorPicker.SetColor(initialRgba);

        HorizontalLine = new HorizontalLineNode {
            Position = ContentStartPosition + new Vector2(2.0f, ContentSize.Y - 40.0f),
            Size = new Vector2(ContentSize.X - 4.0f, 2.0f),
        };
        HorizontalLine.AttachNode(this);

        ConfirmButton = new TextButtonNode {
            Position = ContentStartPosition + new Vector2(0.0f, ContentSize.Y - 24.0f),
            Size = new Vector2(100.0f, 24.0f),
            String = "Confirm",
            OnClick = OnConfirmClicked,
        };
        ConfirmButton.AttachNode(this);

        if (DefaultHsvaColor is { } defaultColor) {
            DefaultColorPreview = new ColorSquareTextButtonNode {
                Size = new Vector2(100.0f, 24.0f),
                Position = ContentStartPosition + new Vector2(ContentSize.X / 2.0f - 50.0f, ContentSize.Y - 24.0f),
                String = "Default",
                OnClick = OnDefaultClicked,
                DefaultHsvaColor = defaultColor,
            };
            DefaultColorPreview.AttachNode(this);
        }

        CancelButton = new TextButtonNode {
            Position = ContentStartPosition + new Vector2(ContentSize.X - 100.0f, ContentSize.Y - 24.0f),
            Size = new Vector2(100.0f, 24.0f),
            String = "Cancel",
            OnClick = OnCancelClicked,
        };
        CancelButton.AttachNode(this);
    }

    /// <inheritdoc />
    protected override unsafe void OnFinalize(AtkUnitBase* addon) {
        base.OnFinalize(addon);

        ColorPicker = null;
        HorizontalLine = null;
        ConfirmButton = null;
        DefaultColorPreview = null;
        CancelButton = null;
    }

    /// <inheritdoc />
    protected override unsafe void OnHide(AtkUnitBase* addon) {
        if (!isCancelClicked) {
            OnHsvaColorPreviewed?.Invoke(initialHsva);
            OnColorPreviewed?.Invoke(initialRgba);

            OnColorCancelled?.Invoke();
        }
    }

    private void OnConfirmClicked() {
        if (ColorPicker is null) return;

        var rgba = ColorHelpers.HsvToRgb(ColorPicker.CurrentColor);
        OnColorConfirmed?.Invoke(rgba);
        OnHsvaColorConfirmed?.Invoke(ColorPicker.CurrentColor);

        isCancelClicked = true;

        Close();
    }

    private void OnDefaultClicked() {
        if (ColorPicker is null) return;

        if (DefaultHsvaColor is { } defaultColor) {
            ColorPicker.SetColor(defaultColor);
        }
    }

    private void OnCancelClicked() {
        isCancelClicked = true;

        OnHsvaColorPreviewed?.Invoke(initialHsva);
        OnColorPreviewed?.Invoke(initialRgba);

        OnColorCancelled?.Invoke();
        Close();
    }

    private ColorPickerNode? ColorPicker { get; set; }
    private HorizontalLineNode? HorizontalLine { get; set; }
    private TextButtonNode? ConfirmButton { get; set; }
    private ColorSquareTextButtonNode? DefaultColorPreview { get; set; }
    private TextButtonNode? CancelButton { get; set; }

    private bool isCancelClicked;

    private Vector4 initialRgba;
    private ColorHelpers.HsvaColor initialHsva;
}
