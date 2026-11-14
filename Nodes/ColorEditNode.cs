using System;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Client.System.Input;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Addons;
using KamiToolKit.Interfaces;
using KamiToolKit.Nodes.Simplified;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Nodes;

/// <summary>
/// A node representing a colored square, and a label, all of which are clickable to open a color picker window.
/// </summary>
public class ColorEditNode : SimpleComponentNode, IColorEditNode {

    /// <inheritdoc />
    public Vector4 CurrentColor {
        get => squareNode.Color;
        set => squareNode.Color = value;
    }

    /// <inheritdoc />
    public ReadOnlySeString String {
        get => labelNode.String;
        set => labelNode.String = value;
    }

    /// <inheritdoc />
    public Vector4 TextColor {
        get => labelNode.TextColor;
        set => labelNode.TextColor = value;
    }

    /// <inheritdoc />
    public Vector4 TextOutlineColor {
        get => labelNode.TextOutlineColor;
        set => labelNode.TextOutlineColor = value;
    }

    /// <inheritdoc />
    public AlignmentType AlignmentType {
        get => labelNode.AlignmentType;
        set => labelNode.AlignmentType = value;
    }

    /// <inheritdoc />
    public FontType FontType {
        get => labelNode.FontType;
        set => labelNode.FontType = value;
    }

    /// <inheritdoc />
    public TextFlags TextFlags {
        get => labelNode.TextFlags;
        set => labelNode.TextFlags = value;
    }

    /// <inheritdoc />
    public uint FontSize {
        get => labelNode.FontSize;
        set => labelNode.FontSize = value;
    }

    /// <inheritdoc />
    public Vector4? DefaultColor { get; set; }

    /// <inheritdoc />
    public Action? OnColorCancelled { get; set; }

    /// <inheritdoc />
    public Action<Vector4>? OnColorPreviewed { get; set; }

    /// <inheritdoc />
    public Action<Vector4>? OnColorConfirmed { get; set; }

    /// <summary>
    /// Constructs a <see cref="ColorEditNode"/> instance.
    /// </summary>
    public unsafe ColorEditNode() {
        squareNode = new ColorSquareNode();
        squareNode.AttachNode(this);

        labelNode = new TextNode {
            AlignmentType = AlignmentType.Left,
        };
        labelNode.AttachNode(this);

        squareNode.ShowClickableCursor = true;
        squareNode.AddEvent(AtkEventType.MouseClick, OnClicked);

        labelNode.ShowClickableCursor = true;
        labelNode.AddEvent(AtkEventType.MouseClick, OnClicked);

        CollisionNode.ShowClickableCursor = true;
        CollisionNode.AddEvent(AtkEventType.MouseClick, OnClicked);
        CollisionNode.AddEvent(AtkEventType.InputReceived, OnInputReceived);
    }

    /// <inheritdoc />
    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        squareNode.Size = new Vector2(Height - 6.0f, Height - 6.0f);
        squareNode.Position = new Vector2(3.0f, 3.0f);

        labelNode.Size = new Vector2(Width - Height - 12.0f, Height);
        labelNode.Position = new Vector2(squareNode.Bounds.Right + 12.0f, 0.0f);
    }

    /// <inheritdoc />
    protected override void Dispose(bool isNativeDestructor) {
        if (IsDisposed) return;

        OnColorCancelled = null;
        OnColorPreviewed = null;
        OnColorConfirmed = null;

        colorPicker?.Dispose();
        colorPicker = null;

        base.Dispose(isNativeDestructor);
    }

    private unsafe void OnInputReceived(AtkEventListener* thisPtr, AtkEventType eventType, int eventParam, AtkEvent* atkEvent, AtkEventData* atkEventData) {
        if (eventType is AtkEventType.InputReceived && (InputId)atkEventData->InputData.InputId is InputId.OK) {
            OnClicked();
        }
    }

    private void OnClicked() {
        var originalColor = CurrentColor;
        colorPicker?.DefaultColor = DefaultColor;
        colorPicker?.InitialColor = CurrentColor;

        colorPicker?.OnColorPreviewed = color => {
            squareNode.Color = color;
            CurrentColor = color;
            OnColorPreviewed?.Invoke(color);
        };

        colorPicker?.OnColorCancelled = () => {
            CurrentColor = originalColor;
            OnColorCancelled?.Invoke();
        };

        colorPicker?.OnColorConfirmed = color => {
            CurrentColor = color;
            OnColorConfirmed?.Invoke(color);
        };

        colorPicker?.Toggle();
    }

    private ColorPickerAddon? colorPicker = new() {
        InternalName = "ColorPicker",
        Title = "Color Picker",
    };

    private readonly ColorSquareNode squareNode;
    private readonly TextNode labelNode;
}
