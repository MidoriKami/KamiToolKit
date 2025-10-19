using System.Numerics;
using Dalamud.Game.Addon.Events;
using Dalamud.Game.Addon.Events.EventDataTypes;
using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;
using KamiToolKit.Widgets.Parts;

namespace KamiToolKit.Widgets;

public unsafe class ColorPickerWidget : SimpleComponentNode {
    public readonly ColorRingWithSquareNode ColorPickerNode;
    
    public readonly ImGuiImageNode AlphaBarBackgroundNode;
    public readonly ImGuiImageNode AlphaBarNode;
    public readonly ImGuiImageNode AlphaBarSelectorNode;

    public readonly ColorPreviewWithInput ColorPreviewWithInput;

    private ViewportEventListener alphaEventListener;
    private bool isAlphaDragging;

    public ColorHelpers.HsvaColor CurrentColor { get; set; }
    
    public ColorPickerWidget() {
        alphaEventListener = new ViewportEventListener(AlphaSliderEvent);
        
        ColorPickerNode = new ColorRingWithSquareNode {
            IsVisible = true,
            OnHueChanged = SetHue,
            OnSaturationChanged = SetSaturation,
            OnValueChanged = SetValue,
        };
        ColorPickerNode.AttachNode(this);

        AlphaBarBackgroundNode = new ImGuiImageNode {
            TexturePath = DalamudInterface.Instance.GetAssetPath("alpha_background.png"),
            IsVisible = true,
            WrapMode = WrapMode.Tile,
        };
        AlphaBarBackgroundNode.AttachNode(this);

        AlphaBarNode = new ImGuiImageNode {
            TexturePath = DalamudInterface.Instance.GetAssetPath("VerticalGradient_WhiteToAlpha.png"),
            IsVisible = true,
            FitTexture = true,
            EnableEventFlags = true,
        };
        AlphaBarNode.AttachNode(this);
        AlphaBarNode.AddEvent(AddonEventType.MouseDown, OnAlphaBarMouseDown);

        AlphaBarSelectorNode = new ImGuiImageNode {
            TexturePath = DalamudInterface.Instance.GetAssetPath("alpha_selector.png"),
            IsVisible = true,
            FitTexture = true,
            EnableEventFlags = true,
        };
        AlphaBarSelectorNode.AttachNode(this);
        AlphaBarSelectorNode.AddEvent(AddonEventType.MouseDown, OnAlphaBarMouseDown);

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

    protected override void Dispose(bool disposing, bool isNativeDestructor) {
        if (disposing) {
            base.Dispose(disposing, isNativeDestructor);

            alphaEventListener.Dispose();
        }
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        var mainWidgetWidth = Width * 3.0f / 4.0f;
        
        ColorPickerNode.Size = new Vector2(mainWidgetWidth, mainWidgetWidth);

        AlphaBarBackgroundNode.Size = new Vector2(Width / 16.0f, mainWidgetWidth - 60.0f);
        AlphaBarBackgroundNode.Position = new Vector2(mainWidgetWidth + (Width - mainWidgetWidth) / 3.0f - AlphaBarBackgroundNode.Width / 2.0f, 30.0f);

        AlphaBarNode.Size = new Vector2(Width / 16.0f, mainWidgetWidth - 60.0f);
        AlphaBarNode.Position = new Vector2(mainWidgetWidth + (Width - mainWidgetWidth) / 3.0f - AlphaBarBackgroundNode.Width / 2.0f, 30.0f);

        AlphaBarSelectorNode.Size = new Vector2(AlphaBarNode.Width + 4.0f, 10.0f);
        AlphaBarSelectorNode.Position = new Vector2(AlphaBarNode.X - 2.0f, AlphaBarNode.Y);

        ColorPreviewWithInput.Size = new Vector2(150.0f, 32.0f);
        ColorPreviewWithInput.Position = new Vector2(Width / 2.0f - 75.0f, ColorPickerNode.Y + ColorPickerNode.Height - 1.0f);
    }
    
    private void OnAlphaBarMouseDown(AddonEventData obj) {
        if (!isAlphaDragging) {
            alphaEventListener.AddEvent(AtkEventType.MouseMove, AlphaBarNode);
            alphaEventListener.AddEvent(AtkEventType.MouseUp, AlphaBarNode);
            isAlphaDragging = true;
        }
    }
    
    private void AlphaSliderEvent(AtkEventListener* thisPtr, AtkEventType eventType, int eventParam, AtkEvent* atkEvent, AtkEventData* atkEventData) {
        if (eventType is AtkEventType.MouseUp) {
            alphaEventListener.RemoveEvent(AtkEventType.MouseMove);
            alphaEventListener.RemoveEvent(AtkEventType.MouseUp);
            isAlphaDragging = false;
        }

        if (eventType is AtkEventType.MouseMove) {
            var mousePosition = new Vector2(atkEventData->MouseData.PosX, atkEventData->MouseData.PosY);
            var minY = AlphaBarNode.ScreenY;
            var maxY = AlphaBarNode.ScreenY + AlphaBarNode.Height;

            if (mousePosition.Y >= minY && mousePosition.Y <= maxY) {
                var alphaRatio = 1.0f - (mousePosition.Y - AlphaBarNode.ScreenY) / AlphaBarNode.Height;
                SetAlpha(alphaRatio);
            }
        }
    }

    public void SetAlpha(float alpha) {
        CurrentColor = CurrentColor with { A = alpha };

        AlphaBarSelectorNode.Y = AlphaBarNode.Y + AlphaBarNode.Height - AlphaBarNode.Height * CurrentColor.A - 5.0f;

        ColorPreviewWithInput.HsvaColor = CurrentColor;
    }

    public void SetHue(float hue) {
        CurrentColor = CurrentColor with { H = hue };

        ColorPickerNode.RotationDegrees = hue * 360.0f;
        ColorPickerNode.SelectorColor = CurrentColor;
        ColorPickerNode.SquareColor = CurrentColor with { S = 1.0f, V = 1.0f };

        ColorPreviewWithInput.HsvaColor = CurrentColor;

        AlphaBarNode.HsvaMultiplyColor = CurrentColor with { A = 1.0f };
    }

    public void SetSaturation(float saturation) {
        CurrentColor = CurrentColor with { S = saturation };
        
        ColorPreviewWithInput.HsvaColor = CurrentColor;
        ColorPickerNode.SelectorColor = CurrentColor;
        
        AlphaBarNode.HsvaMultiplyColor = CurrentColor with { A = 1.0f };
    }

    public void SetValue(float value) {
        CurrentColor = CurrentColor with { V = value };

        ColorPreviewWithInput.HsvaColor = CurrentColor;
        ColorPickerNode.SelectorColor = CurrentColor;

        AlphaBarNode.HsvaMultiplyColor = CurrentColor with { A = 1.0f };
    }
}
