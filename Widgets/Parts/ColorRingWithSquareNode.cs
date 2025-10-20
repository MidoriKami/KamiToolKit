using System;
using System.Numerics;
using Dalamud.Game.Addon.Events;
using Dalamud.Game.Addon.Events.EventDataTypes;
using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;

namespace KamiToolKit.Widgets.Parts;

public unsafe class ColorRingWithSquareNode : SimpleComponentNode {
    public readonly ColorSquareNode ColorSquareNode;
    public readonly ImGuiImageNode ColorRingNode;
    public readonly ImGuiImageNode ColorRingSelectorNode;

    private bool isRingDrag;
    private bool isSquareDrag;

    private ViewportEventListener eventListener;

    public ColorRingWithSquareNode() {
        eventListener = new ViewportEventListener(SquareEventHandler);
        
        ColorSquareNode = new ColorSquareNode {
            IsVisible = true,
            Color = new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
            DrawFlags = DrawFlags.UseTransformedCollision,
        };
        ColorSquareNode.AttachNode(this);

        ColorRingNode = new ImGuiImageNode {
            TexturePath = DalamudInterface.Instance.GetAssetPath("color_ring.png"),
            IsVisible = true,
            FitTexture = true,
            ImageNodeFlags = ImageNodeFlags.FlipV,
        };
        ColorRingNode.AttachNode(this);

        ColorRingSelectorNode = new ImGuiImageNode {
            TexturePath = DalamudInterface.Instance.GetAssetPath("color_ring_selector.png"),
            IsVisible = true,
            FitTexture = true,
            MultiplyColor = new Vector3(1.0f, 0.0f, 0.0f),
        };
        ColorRingSelectorNode.AttachNode(this);
        
        AddEvent(AddonEventType.MouseDown, OnMouseDown);
        AddEvent(AddonEventType.MouseUp, OnMouseUp);
        AddEvent(AddonEventType.MouseMove, OnMouseMove);
        AddEvent(AddonEventType.MouseOut, OnMouseOut);
        EnableEventFlags = true;
    }

    protected override void Dispose(bool disposing, bool isNativeDestructor) {
        if (disposing) {
            base.Dispose(disposing, isNativeDestructor);
            eventListener.Dispose();
        }
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        ColorSquareNode.Size = Size / 2.0f - new Vector2(24.0f, 24.0f);
        ColorSquareNode.Position = Size / 4.0f + new Vector2(12.0f, 12.0f);
        ColorSquareNode.RotationDegrees = 45.0f;

        ColorRingNode.Size = Size;

        ColorRingSelectorNode.Size = Size;
        ColorRingSelectorNode.Origin = Size / 2.0f;
    }

    private bool IsRingClicked(ref AtkEventData.AtkMouseData mouseData) {
        var clickPosition = new Vector2(mouseData.PosX, mouseData.PosY);
        var center = ColorRingNode.ScreenPosition + ColorRingNode.Size / 2.0f;
        var distance = Vector2.Distance(clickPosition, center);
        var scaledDistance = distance / (Width / 256.0f);

        return scaledDistance is >= 82.0f and <= 99.0f;
    }

    private float GetRingClickAngle(ref AtkEventData.AtkMouseData mouseData) {
        var clickPosition = new Vector2(mouseData.PosX, mouseData.PosY);
        var center = ColorRingNode.ScreenPosition + ColorRingNode.Size / 2.0f;
        var relativePosition = clickPosition - center;
        var calculatedAngle = MathF.Atan2(relativePosition.Y, relativePosition.X) * 180.0f / MathF.PI;

        return calculatedAngle;
    }
    
    private void OnMouseDown(AddonEventData obj) {
        if (ColorSquareNode.CheckCollision((AtkEventData*)obj.AtkEventDataPointer)) {
            UpdateSquareColor(obj.GetMousePosition());

            if (!isSquareDrag) {
                isSquareDrag = true;
                eventListener.AddEvent(AtkEventType.MouseMove, ColorSquareNode);
                eventListener.AddEvent(AtkEventType.MouseUp, ColorSquareNode);
            }
        }

        if (IsRingClicked(ref obj.GetMouseData())) {
            isRingDrag = true;
            UpdateRingColor(ref obj);
        }
    }

    private void OnMouseMove(AddonEventData obj) {
        if (isRingDrag && !isSquareDrag) {
            UpdateRingColor(ref obj);
        }
    }

    private void OnMouseUp(AddonEventData obj) {
        isRingDrag = false;
        isSquareDrag = false;
    }

    private void OnMouseOut(AddonEventData obj) {
        isRingDrag = false;
        isSquareDrag = false;
    }

    private void UpdateRingColor(ref AddonEventData mouseData) {
        var angle = GetRingClickAngle(ref mouseData.GetMouseData());

        if (angle < 0) {
            angle += 360.0f;
        }

        OnHueChanged?.Invoke(angle / 360.0f);
    }

    private void UpdateSquareColor(Vector2 clickPosition) {
        // Note: ColorSquareNode.ScreenPosition changes as the node rotates
        // However, Position does not change
        var center = ScreenPosition + ColorSquareNode.Position + ColorSquareNode.Origin; 

        var relativePosition = clickPosition - center;
        var rotatedPoint = RotatePoint(relativePosition, Vector2.Zero, -ColorSquareNode.RotationDegrees);

        var xClamped = Math.Clamp(rotatedPoint.X, -ColorSquareNode.Width / 2, ColorSquareNode.Width / 2);
        var yClamped = Math.Clamp(rotatedPoint.Y, -ColorSquareNode.Height / 2, ColorSquareNode.Height / 2);

        ColorSquareNode.ColorDotPosition = new Vector2(xClamped, yClamped) + ColorSquareNode.Origin;

        var saturation = ColorSquareNode.ColorDotPosition.X / ColorSquareNode.Width;
        var lightness = 1 - ColorSquareNode.ColorDotPosition.Y / ColorSquareNode.Height;

        OnSaturationChanged?.Invoke(saturation);
        OnValueChanged?.Invoke(lightness);
    }

    private void SquareEventHandler(AtkEventListener* thisPtr, AtkEventType eventType, int eventParam, AtkEvent* atkEvent, AtkEventData* atkEventData) {
        if (eventType is AtkEventType.MouseMove && isSquareDrag && !isRingDrag) {
            UpdateSquareColor(new Vector2(atkEventData->MouseData.PosX, atkEventData->MouseData.PosY));
        }

        if (eventType is AtkEventType.MouseUp) {
            isSquareDrag = false;
            eventListener.RemoveEvent(AtkEventType.MouseMove);
            eventListener.RemoveEvent(AtkEventType.MouseUp);
        }
    }
    
    private static Vector2 RotatePoint(Vector2 pointToRotate, Vector2 centerPoint, float angleInDegrees) {
        var angleInRadians = angleInDegrees * (MathF.PI / 180);
        var cosTheta = MathF.Cos(angleInRadians);
        var sinTheta = MathF.Sin(angleInRadians);
        return new Vector2 { 
            X = cosTheta * (pointToRotate.X - centerPoint.X) - sinTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.X,
            Y = sinTheta * (pointToRotate.X - centerPoint.X) + cosTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.Y,
        };
    }

    public Action<float>? OnHueChanged { get; init; }
    public Action<float>? OnSaturationChanged { get; init; }
    public Action<float>? OnValueChanged { get; init; }

    public override float RotationDegrees {
        get => ColorSquareNode.RotationDegrees;
        set {
            ColorSquareNode.RotationDegrees = value + 45.0f;
            ColorRingSelectorNode.RotationDegrees = value;
        }
    }

    public ColorHelpers.HsvaColor SelectorColor {
        get => ColorRingSelectorNode.HsvaMultiplyColor;
        set => ColorRingSelectorNode.HsvaMultiplyColor = value;
    }

    public ColorHelpers.HsvaColor SquareColor {
        get => ColorSquareNode.HsvaMultiplyColor;
        set {
            ColorSquareNode.HsvaMultiplyColor = value with { S = 1.0f, V = 1.0f };
            ColorSquareNode.ColorDotPosition = new Vector2(ColorSquareNode.Width * value.S, ColorSquareNode.Height - ColorSquareNode.Height * value.V);
        }
    }
}
