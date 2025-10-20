using System;
using System.Numerics;
using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Addon;
using KamiToolKit.Nodes;
using KamiToolKit.Widgets;

namespace KamiToolKit.Addons;

public class ColorPickerAddon : NativeAddon {
    
    private ColorPickerWidget? colorPicker;
    
    private HorizontalLineNode? horizontalLine;
    
    private TextButtonNode? confirmButton;
    private TextButtonNode? cancelButton;

    protected override unsafe void OnSetup(AtkUnitBase* addon) {
        ChangeWindowSize(new Vector2(400.0f, 425.0f));
        
        colorPicker = new ColorPickerWidget {
            Position = ContentStartPosition,
            Size = ContentSize,
            IsVisible = true,
        };
        AttachNode(colorPicker);

        horizontalLine = new HorizontalLineNode {
            Position = ContentStartPosition + new Vector2(2.0f, ContentSize.Y - 40.0f),
            Size = new Vector2(ContentSize.X - 4.0f, 2.0f),
            IsVisible = true,
        };
        AttachNode(horizontalLine);

        confirmButton = new TextButtonNode {
            Position = ContentStartPosition + new Vector2(0.0f, ContentSize.Y - 24.0f),
            Size = new Vector2(100.0f, 24.0f),
            String = "Confirm",
            IsVisible = true,
            OnClick = OnConfirmClicked,
        };
        AttachNode(confirmButton);

        cancelButton = new TextButtonNode {
            Position = ContentStartPosition + new Vector2(ContentSize.X - 100.0f, ContentSize.Y - 24.0f),
            Size = new Vector2(100.0f, 24.0f),
            String = "Cancel",
            IsVisible = true,
            OnClick = OnCancelClicked,
        };
        AttachNode(cancelButton);
    }

    private void OnConfirmClicked() {
        if (colorPicker is null) return;

        OnColorConfirmed?.Invoke(ColorHelpers.HsvToRgb(colorPicker.CurrentColor));
        OnHsvaColorConfirmed?.Invoke(colorPicker.CurrentColor);
        Close();
    }

    private void OnCancelClicked() {
        if (colorPicker is null) return;
        
        OnColorCancelled?.Invoke();
        Close();
    }

    public Action<Vector4>? OnColorConfirmed { get; init; }
    public Action<ColorHelpers.HsvaColor>? OnHsvaColorConfirmed { get; init; }
    public Action? OnColorCancelled { get; init; }
}
