using System;
using System.Numerics;
using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Enums;
using KamiToolKit.Internal.Classes;
using KamiToolKit.Nodes.Simplified;

namespace KamiToolKit.Nodes;

/// <summary>
/// ColorGradiant Square with color ring around it. Not intended for external use.
/// </summary>
public unsafe class ColorRingWithSquareNode : SimpleComponentNode {

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public ColorGradientSquare ColorGradientSquare { get; }

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public ImGuiImageNode ColorRingNode { get; }

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public ImGuiImageNode ColorRingSelectorNode { get; }

    /// <summary>
    /// Gets or sets an action that is called on hue change.
    /// </summary>
    public Action<float>? OnHueChanged { get; init; }

    /// <summary>
    /// Gets or sets an action that is called on saturation changed.
    /// </summary>
    public Action<float>? OnSaturationChanged { get; init; }

    /// <summary>
    /// Gets or sets an action that is called on value changed.
    /// </summary>
    public Action<float>? OnValueChanged { get; init; }

    /// <inheritdoc/>
    public override float RotationDegrees {
        get => ColorGradientSquare.RotationDegrees;
        set {
            ColorGradientSquare.RotationDegrees = value + 45.0f;
            ColorRingSelectorNode.RotationDegrees = value;
        }
    }

    /// <summary>
    /// Gets or sets the rings selector color.
    /// </summary>
    public ColorHelpers.HsvaColor SelectorColor {
        get => ColorRingSelectorNode.MultiplyColorHsva;
        set => ColorRingSelectorNode.MultiplyColorHsva = value;
    }

    /// <summary>
    /// Gets or sets the gradiant squares color.
    /// </summary>
    public ColorHelpers.HsvaColor SquareColor {
        get => ColorGradientSquare.MultiplyColorHsva;
        set => ColorGradientSquare.MultiplyColorHsva = value with { S = 1.0f, V = 1.0f };
    }

    /// <summary>
    /// Gets or sets the squares saturation color.
    /// </summary>
    /// <remarks>
    /// Setting this moves the color dot.
    /// </remarks>
    public ColorHelpers.HsvaColor SquareSaturationValue {
        get => ColorGradientSquare.MultiplyColorHsva;
        set => ColorGradientSquare.ColorDotPosition = new Vector2(ColorGradientSquare.Width * value.S, ColorGradientSquare.Height - ColorGradientSquare.Height * value.V);
    }

    /// <summary>
    /// Constructs a <see cref="ColorRingWithSquareNode"/> instance.
    /// </summary>
    public ColorRingWithSquareNode() {
        eventListener = new ViewportEventListener(SquareEventHandler);

        ColorGradientSquare = new ColorGradientSquare {
            DrawFlags = DrawFlags.UseTransformedCollision,
        };
        ColorGradientSquare.AttachNode(this);

        ColorRingNode = new ImGuiImageNode {
            TexturePath = Services.GetAssetPath("color_ring.png"),
            FitTexture = true,
            ImageNodeFlags = ImageNodeFlags.FlipV,
        };
        ColorRingNode.AttachNode(this);

        ColorRingSelectorNode = new ImGuiImageNode {
            TexturePath = Services.GetAssetPath("color_ring_selector.png"),
            FitTexture = true,
            MultiplyColor = new Vector3(1.0f, 0.0f, 0.0f),
        };
        ColorRingSelectorNode.AttachNode(this);

        AddEvent(AtkEventType.MouseDown, OnMouseDown);
        AddEvent(AtkEventType.MouseUp, OnMouseUp);
        AddEvent(AtkEventType.MouseMove, OnMouseMove);
        AddEvent(AtkEventType.MouseOut, OnMouseOut);
    }

    /// <inheritdoc />
    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        ColorGradientSquare.Size = Size / 2.0f - new Vector2(24.0f, 24.0f);
        ColorGradientSquare.Position = Size / 4.0f + new Vector2(12.0f, 12.0f);
        ColorGradientSquare.RotationDegrees = 45.0f;

        ColorRingNode.Size = Size;

        ColorRingSelectorNode.Size = Size;
        ColorRingSelectorNode.Origin = Size / 2.0f;
    }

    /// <inheritdoc />
    protected override void Dispose(bool isNativeDestructor) {
        if (IsDisposed) return;

        eventListener.Dispose();

        base.Dispose(isNativeDestructor);
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
        if (ColorGradientSquare.CheckCollision(atkEventData)) {
            UpdateSquareColor(atkEventData->MousePosition);

            if (!isSquareDrag) {
                isSquareDrag = true;
                eventListener.AddEvent(AtkEventType.MouseMove, ColorGradientSquare);
                eventListener.AddEvent(AtkEventType.MouseUp, ColorGradientSquare);
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
        var center = ScreenPosition + (ColorGradientSquare.Position + ColorGradientSquare.Origin) * scale;

        var relativePosition = clickPosition - center;
        var rotatedPoint = RotatePoint(relativePosition / scale, Vector2.Zero, -ColorGradientSquare.RotationDegrees) / ColorGradientSquare.Scale;

        var xClamped = Math.Clamp(rotatedPoint.X, -ColorGradientSquare.Width / 2, ColorGradientSquare.Width / 2);
        var yClamped = Math.Clamp(rotatedPoint.Y, -ColorGradientSquare.Height / 2, ColorGradientSquare.Height / 2);

        ColorGradientSquare.ColorDotPosition = new Vector2(xClamped, yClamped) + ColorGradientSquare.Origin;

        var saturation = ColorGradientSquare.ColorDotPosition.X / ColorGradientSquare.Width;
        var lightness = 1 - ColorGradientSquare.ColorDotPosition.Y / ColorGradientSquare.Height;

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

    private bool isRingDrag;
    private bool isSquareDrag;

    private readonly ViewportEventListener eventListener;
}
