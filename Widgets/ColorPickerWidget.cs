using System.Drawing;
using System.Globalization;
using System.Numerics;
using Dalamud.Game.Addon.Events;
using Dalamud.Game.Addon.Events.EventDataTypes;
using Dalamud.Game.Text.SeStringHandling;
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

    public readonly BackgroundImageNode SelectedColorPreviewNode;
    public readonly ImGuiImageNode AlphaLayerPreviewNode;
    public readonly BackgroundImageNode SelectedColorPreviewBorderNode;

    public readonly TextInputNode ColorInputNode;

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
        
        // We'll have to delegate further events to the ViewportListener
        AlphaBarNode.AddEvent(AddonEventType.MouseDown, OnAlphaBarMouseDown);

        AlphaBarSelectorNode = new ImGuiImageNode {
            TexturePath = DalamudInterface.Instance.GetAssetPath("alpha_selector.png"),
            IsVisible = true,
            FitTexture = true,
            EnableEventFlags = true,
        };
        AlphaBarSelectorNode.AttachNode(this);

        AlphaBarSelectorNode.AddEvent(AddonEventType.MouseDown, OnAlphaBarMouseDown);

        SelectedColorPreviewBorderNode = new BackgroundImageNode {
            IsVisible = true,
            Color = KnownColor.White.Vector(),
        };
        SelectedColorPreviewBorderNode.AttachNode(this);

        AlphaLayerPreviewNode = new ImGuiImageNode {
            TexturePath = DalamudInterface.Instance.GetAssetPath("alpha_background.png"),
            IsVisible = true,
            WrapMode = WrapMode.Tile,
        };
        AlphaLayerPreviewNode.AttachNode(this);

        SelectedColorPreviewNode = new BackgroundImageNode {
            IsVisible = true,
            Color = new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
        };
        SelectedColorPreviewNode.AttachNode(this);

        ColorInputNode = new TextInputNode {
            IsVisible = true,
            AutoSelectAll = true,
            OnInputComplete = OnTextInputComplete,
        };
        ColorInputNode.AttachNode(this);

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

        SelectedColorPreviewBorderNode.Size = new Vector2(34.0f, 34.0f);
        SelectedColorPreviewBorderNode.Position = new Vector2(Width / 2.0f - 16.0f - 50.0f - 1.0f, ColorPickerNode.Y + ColorPickerNode.Height - 1.0f);

        AlphaLayerPreviewNode.Size = new Vector2(32.0f, 32.0f);
        AlphaLayerPreviewNode.Position = new Vector2(Width / 2.0f - 16.0f - 50.0f, ColorPickerNode.Y + ColorPickerNode.Height);

        SelectedColorPreviewNode.Size = new Vector2(32.0f, 32.0f);
        SelectedColorPreviewNode.Position = new Vector2(Width / 2.0f - 16.0f - 50.0f, ColorPickerNode.Y + ColorPickerNode.Height);
        
        ColorInputNode.Size = new Vector2(100.0f, 32.0f);
        ColorInputNode.Position = new Vector2(SelectedColorPreviewNode.X + SelectedColorPreviewNode.Width + 10.0f, ColorPickerNode.Y + ColorPickerNode.Height);
    }
    
    private void OnTextInputComplete(SeString obj) {
        Log.Debug("TextInputComplete");
        
        if (!obj.ToString().StartsWith('#')) return;

        var hexString = obj.ToString().TrimStart('#');

        var r = byte.Parse(hexString[0..2], NumberStyles.HexNumber);
        var g = byte.Parse(hexString[2..4], NumberStyles.HexNumber);
        var b = byte.Parse(hexString[4..6], NumberStyles.HexNumber);
        var a = byte.Parse(hexString[6..8], NumberStyles.HexNumber);

        var newColor = new Vector4(r / 255.0f, g / 255.0f, b / 255.0f, a / 255.0f);

        SetRgb(newColor);
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
        Log.Debug($"SetAlpha: {alpha}");
        CurrentColor = CurrentColor with { A = alpha };

        AlphaBarSelectorNode.Y = AlphaBarNode.Y + AlphaBarNode.Height - AlphaBarNode.Height * CurrentColor.A - 5.0f;

        SelectedColorPreviewNode.HsvaColor = CurrentColor;
        
        UpdateColorText();
    }

    public void SetHue(float hue) {
        Log.Debug($"SetHue: {hue}");

        CurrentColor = CurrentColor with { H = hue };

        ColorPickerNode.RotationDegrees = hue * 360.0f;
        ColorPickerNode.SelectorColor = CurrentColor;
        ColorPickerNode.SquareColor = CurrentColor with { S = 1.0f, V = 1.0f };

        SelectedColorPreviewNode.HsvaColor = CurrentColor;

        AlphaBarNode.HsvaMultiplyColor = CurrentColor with { A = 1.0f };
        
        UpdateColorText();
    }

    public void SetSaturation(float saturation) {
        Log.Debug($"SetSaturation: {saturation}");
        
        CurrentColor = CurrentColor with { S = saturation };
        
        SelectedColorPreviewNode.HsvaColor = CurrentColor;
        ColorPickerNode.SelectorColor = CurrentColor;
        
        AlphaBarNode.HsvaMultiplyColor = CurrentColor with { A = 1.0f };
        
        UpdateColorText();
    }

    public void SetValue(float value) {
        CurrentColor = CurrentColor with { V = value };

        SelectedColorPreviewNode.HsvaColor = CurrentColor;
        ColorPickerNode.SelectorColor = CurrentColor;

        AlphaBarNode.HsvaMultiplyColor = CurrentColor with { A = 1.0f };
        
        UpdateColorText();
    }

    private void UpdateColorText(Vector4? color = null) {
        var rgbColor = color ?? ColorHelpers.HsvToRgb(CurrentColor);
        ColorInputNode.String = $"#{(int)(rgbColor.X * 255):X2}{(int)(rgbColor.Y * 255):X2}{(int)(rgbColor.Z * 255):X2}{(int)(rgbColor.W * 255):X2}";
    }

    private void SetRgb(Vector4 value) {
        var converted = ColorHelpers.RgbaToHsv(value);

        SetHue(converted.H);
        SetSaturation(converted.S);
        SetValue(converted.V);
        SetAlpha(converted.A);

        UpdateColorText(value);
    }
}
