using System;
using System.Numerics;
using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;

namespace KamiToolKit.Premade.Color;

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
            DrawFlags = DrawFlags.UseTransformedCollision,
        };
        ColorSquareNode.AttachNode(this);

        ColorRingNode = new ImGuiImageNode {
            TexturePath = DalamudInterface.Instance.GetAssetPath("color_ring.png"),
            FitTexture = true,
            ImageNodeFlags = ImageNodeFlags.FlipV,
        };
        ColorRingNode.AttachNode(this);

        ColorRingSelectorNode = new ImGuiImageNode {
            TexturePath = DalamudInterface.Instance.GetAssetPath("color_ring_selector.png"),
            FitTexture = true,
            MultiplyColor = new Vector3(1.0f, 0.0f, 0.0f),
        };
        ColorRingSelectorNode.AttachNode(this);

        AddEvent(AtkEventType.MouseDown, OnMouseDown);
        AddEvent(AtkEventType.MouseUp, OnMouseUp);
        AddEvent(AtkEventType.MouseMove, OnMouseMove);
        AddEvent(AtkEventType.MouseOut, OnMouseOut);
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

    private bool IsRingClicked(AtkEventData* data) {
        var clickPosition = data->MousePosition;
        var scale = ParentAddon is not null ? ParentAddon->Scale : 1.0f;
        var center = ColorRingNode.ScreenPosition + ColorRingNode.Size * scale / 2.0f;
        var distance = Vector2.Distance(clickPosition, center);
        var scaledDistance = distance / (Width * scale / 256.0f);

        return scaledDistance is >= 82.0f and <= 99.0f;
    }

    private float GetRingClickAngle(AtkEventData* data) {
        var clickPosition = data->MousePosition;
        var scale = ParentAddon is not null ? ParentAddon->Scale : 1.0f;
        var center = ColorRingNode.ScreenPosition + ColorRingNode.Size * scale / 2.0f;
        var relativePosition = clickPosition - center;
        var calculatedAngle = MathF.Atan2(relativePosition.Y, relativePosition.X) * 180.0f / MathF.PI;

        return calculatedAngle;
    }
    
    private void OnMouseDown(AtkEventListener* thisPtr, AtkEventType eventType, int eventParam, AtkEvent* atkEvent, AtkEventData* atkEventData) {
        if (ColorSquareNode.CheckCollision(atkEventData)) {
            UpdateSquareColor(atkEventData->MousePosition);

            if (!isSquareDrag) {
                isSquareDrag = true;
                eventListener.AddEvent(AtkEventType.MouseMove, ColorSquareNode);
                eventListener.AddEvent(AtkEventType.MouseUp, ColorSquareNode);
            }
        }

        if (IsRingClicked(atkEventData)) {
            isRingDrag = true;
            UpdateRingColor(atkEventData);
        }
    }

    private void OnMouseMove(AtkEventListener* thisPtr, AtkEventType eventType, int eventParam, AtkEvent* atkEvent, AtkEventData* atkEventData) {
        if (isRingDrag && !isSquareDrag) {
            UpdateRingColor(atkEventData);
        }
    }

    private void OnMouseUp() {
        isRingDrag = false;
        isSquareDrag = false;
    }

    private void OnMouseOut() {
        isRingDrag = false;
        isSquareDrag = false;
    }

    private void UpdateRingColor(AtkEventData* data) {
        var angle = GetRingClickAngle(data);

        if (angle < 0) {
            angle += 360.0f;
        }

        OnHueChanged?.Invoke(angle / 360.0f);
    }

    private void UpdateSquareColor(Vector2 clickPosition) {
        // Note: ColorSquareNode.ScreenPosition changes as the node rotates
        // However, Position does not change
        var scale = ParentAddon is not null ? ParentAddon->Scale : 1.0f;
        var center = ScreenPosition + (ColorSquareNode.Position + ColorSquareNode.Origin) * scale;

        var relativePosition = clickPosition - center;
        var rotatedPoint = RotatePoint(relativePosition / scale, Vector2.Zero, -ColorSquareNode.RotationDegrees) / ColorSquareNode.Scale;

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
        get => ColorRingSelectorNode.MultiplyColorHsva;
        set => ColorRingSelectorNode.MultiplyColorHsva = value;
    }

    public ColorHelpers.HsvaColor SquareColor {
        get => ColorSquareNode.MultiplyColorHsva;
        set => ColorSquareNode.MultiplyColorHsva = value with { S = 1.0f, V = 1.0f };
    }

    public ColorHelpers.HsvaColor SquareSaturationValue {
        get => ColorSquareNode.MultiplyColorHsva;
        set => ColorSquareNode.ColorDotPosition = new Vector2(ColorSquareNode.Width * value.S, ColorSquareNode.Height - ColorSquareNode.Height * value.V);
    }
}
