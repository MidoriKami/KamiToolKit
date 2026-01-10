using System;
using System.Numerics;
using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;

namespace KamiToolKit.Premade.Color;

public class ColorPickerAddon : NativeAddon {
    private ColorPickerWidget? colorPicker;
    private HorizontalLineNode? horizontalLine;
    private TextButtonNode? confirmButton;
    private ColorOptionTextButtonNode? defaultColorPreview;
    private TextButtonNode? cancelButton;

    private bool isCancelClicked;

    private Vector4 initialRgba;
    private ColorHelpers.HsvaColor initialHsva;

    protected override unsafe void OnSetup(AtkUnitBase* addon) {
        SetWindowSize(new Vector2(400.0f, 425.0f));

        initialHsva = InitialHsvaColor;
        initialRgba = ColorHelpers.HsvToRgb(initialHsva);

        colorPicker = new ColorPickerWidget {
            Position = ContentStartPosition,
            Size = ContentSize,
        };
        colorPicker.AttachNode(this);

        colorPicker.ColorPreviewed += hsva => OnHsvaColorPreviewed?.Invoke(hsva);
        colorPicker.RgbaColorPreviewed += rgba => OnColorPreviewed?.Invoke(rgba);

        colorPicker.SetColor(initialRgba);

        horizontalLine = new HorizontalLineNode {
            Position = ContentStartPosition + new Vector2(2.0f, ContentSize.Y - 40.0f),
            Size = new Vector2(ContentSize.X - 4.0f, 2.0f),
        };
        horizontalLine.AttachNode(this);

        confirmButton = new TextButtonNode {
            Position = ContentStartPosition + new Vector2(0.0f, ContentSize.Y - 24.0f),
            Size = new Vector2(100.0f, 24.0f),
            String = "Confirm",
            OnClick = OnConfirmClicked,
        };
        confirmButton.AttachNode(this);

        if (DefaultHsvaColor is { } defaultColor) {
            defaultColorPreview = new ColorOptionTextButtonNode {
                Size = new Vector2(100.0f, 24.0f),
                Position = ContentStartPosition + new Vector2(ContentSize.X / 2.0f - 50.0f, ContentSize.Y - 24.0f),
                String = "Default",
                OnClick = OnDefaultClicked,
                DefaultHsvaColor = defaultColor,
            };
            defaultColorPreview.AttachNode(this);
        }

        cancelButton = new TextButtonNode {
            Position = ContentStartPosition + new Vector2(ContentSize.X - 100.0f, ContentSize.Y - 24.0f),
            Size = new Vector2(100.0f, 24.0f),
            String = "Cancel",
            OnClick = OnCancelClicked,
        };
        cancelButton.AttachNode(this);
    }

    protected override unsafe void OnHide(AtkUnitBase* addon) {
        if (!isCancelClicked) {
            OnHsvaColorPreviewed?.Invoke(initialHsva);
            OnColorPreviewed?.Invoke(initialRgba);

            OnColorCancelled?.Invoke();
        }
    }

    private void OnConfirmClicked() {
        if (colorPicker is null) return;

        var rgba = ColorHelpers.HsvToRgb(colorPicker.CurrentColor);
        OnColorConfirmed?.Invoke(rgba);
        OnHsvaColorConfirmed?.Invoke(colorPicker.CurrentColor);

        isCancelClicked = true;

        Close();
    }

    private void OnDefaultClicked() {
        if (colorPicker is null) return;

        if (DefaultHsvaColor is { } defaultColor) {
            colorPicker.SetColor(defaultColor);
        }
    }

    private void OnCancelClicked() {
        isCancelClicked = true;

        OnHsvaColorPreviewed?.Invoke(initialHsva);
        OnColorPreviewed?.Invoke(initialRgba);

        OnColorCancelled?.Invoke();
        Close();
    }

    public Action<Vector4>? OnColorPreviewed { get; set; }
    public Action<ColorHelpers.HsvaColor>? OnHsvaColorPreviewed { get; set; }

    public Action<Vector4>? OnColorConfirmed { get; set; }
    public Action<ColorHelpers.HsvaColor>? OnHsvaColorConfirmed { get; set; }
    public Action? OnColorCancelled { get; set; }

    public ColorHelpers.HsvaColor? DefaultHsvaColor { get; set; }

    public Vector4? DefaultColor {
        get;
        set {
            field = value;
            DefaultHsvaColor = value is null ? null : ColorHelpers.RgbaToHsv(value.Value);
        }
    }

    public Vector4 InitialColor {
        get => ColorHelpers.HsvToRgb(InitialHsvaColor);
        set => InitialHsvaColor = ColorHelpers.RgbaToHsv(value);
    }

    public ColorHelpers.HsvaColor InitialHsvaColor { get; set; } =
        ColorHelpers.RgbaToHsv(new Vector4(1.0f, 0.0f, 0.0f, 1.0f));
}
