using System;
using System.Numerics;
using Dalamud.Game.Addon.Events;
using Dalamud.Game.Addon.Events.EventDataTypes;
using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;

namespace KamiToolKit.Widgets.Parts;

public unsafe class AlphaBarNode : SimpleComponentNode {
    public readonly ImGuiImageNode AlphaBarBackgroundNode;
    public readonly ImGuiImageNode AlphaBarGradientNode;
    public readonly ImGuiImageNode AlphaBarSelectorNode;
    
    private ViewportEventListener alphaEventListener;
    private bool isAlphaDragging;

    public AlphaBarNode() {
        alphaEventListener = new ViewportEventListener(AlphaSliderEvent);

        AlphaBarBackgroundNode = new ImGuiImageNode {
            TexturePath = DalamudInterface.Instance.GetAssetPath("alpha_background.png"),
            IsVisible = true,
            WrapMode = WrapMode.Tile,
        };
        AlphaBarBackgroundNode.AttachNode(this);

        AlphaBarGradientNode = new ImGuiImageNode {
            TexturePath = DalamudInterface.Instance.GetAssetPath("VerticalGradient_WhiteToAlpha.png"),
            IsVisible = true,
            FitTexture = true,
            EnableEventFlags = true,
        };
        AlphaBarGradientNode.AttachNode(this);
        AlphaBarGradientNode.AddEvent(AddonEventType.MouseDown, OnAlphaBarMouseDown);

        AlphaBarSelectorNode = new ImGuiImageNode {
            TexturePath = DalamudInterface.Instance.GetAssetPath("alpha_selector.png"),
            IsVisible = true,
            FitTexture = true,
            EnableEventFlags = true,
        };
        AlphaBarSelectorNode.AttachNode(this);
        AlphaBarSelectorNode.AddEvent(AddonEventType.MouseDown, OnAlphaBarMouseDown);
    }
    
    protected override void Dispose(bool disposing, bool isNativeDestructor) {
        if (disposing) {
            base.Dispose(disposing, isNativeDestructor);

            alphaEventListener.Dispose();
        }
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        AlphaBarBackgroundNode.Size = Size;
        AlphaBarGradientNode.Size = Size;

        AlphaBarSelectorNode.Size = new Vector2(Width + 4.0f, 10.0f);
        AlphaBarSelectorNode.Position = new Vector2(-2.0f, 0.0f);
    }

    private void OnAlphaBarMouseDown(AddonEventData obj) {
        if (!isAlphaDragging) {
            alphaEventListener.AddEvent(AtkEventType.MouseMove, AlphaBarGradientNode);
            alphaEventListener.AddEvent(AtkEventType.MouseUp, AlphaBarGradientNode);
            isAlphaDragging = true;
        }
    }
    
    private void AlphaSliderEvent(AtkEventListener* thisPtr, AtkEventType eventType, int eventParam, AtkEvent* atkEvent, AtkEventData* atkEventData) {
        switch (eventType) {
            case AtkEventType.MouseUp:
                alphaEventListener.RemoveEvent(AtkEventType.MouseMove);
                alphaEventListener.RemoveEvent(AtkEventType.MouseUp);
                isAlphaDragging = false;
                break;

            case AtkEventType.MouseMove: {
                var mousePosition = new Vector2(atkEventData->MouseData.PosX, atkEventData->MouseData.PosY);
                var minY = AlphaBarGradientNode.ScreenY;
                var maxY = AlphaBarGradientNode.ScreenY + AlphaBarGradientNode.Height;

                if (mousePosition.Y >= minY && mousePosition.Y <= maxY) {
                    var alphaRatio = 1.0f - (mousePosition.Y - AlphaBarGradientNode.ScreenY) / AlphaBarGradientNode.Height;

                    AlphaBarSelectorNode.Y = Height - Height * alphaRatio - 5.0f;
                    OnAlphaChanged?.Invoke(alphaRatio);
                }
                else if (mousePosition.Y < minY) {
                    AlphaBarSelectorNode.Y = -4.0f;
                    OnAlphaChanged?.Invoke(1.0f);
                }
                else if (mousePosition.Y > maxY) {
                    AlphaBarSelectorNode.Y = Height - 4.0f;
                    OnAlphaChanged?.Invoke(0.0f);
                }
                
                break;
            }
        }
    }

    public Action<float>? OnAlphaChanged { get; init; }

    public override Vector4 Color {
        get => AlphaBarGradientNode.Color;
        set => AlphaBarGradientNode.Color = value;
    }

    public override ColorHelpers.HsvaColor HsvaColor {
        get => AlphaBarGradientNode.HsvaMultiplyColor;
        set => AlphaBarGradientNode.HsvaMultiplyColor = value;
    }
}
