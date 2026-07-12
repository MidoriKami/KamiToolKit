using System;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Client.System.Input;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Addons;
using KamiToolKit.Nodes.Simplified;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Nodes;

/// <summary>
/// A node representing a colored square, and a label, all of which are clickable to open a color picker window.
/// </summary>
public class ColorEditNode : SimpleComponentNode {

    /// <summary>
    /// Gets or sets the current color.
    /// </summary>
    public Vector4 CurrentColor {
        get => squareNode.Color;
        set => squareNode.Color = value;
    }

    /// <summary>
    /// Gets or sets the string displayed next to the color box.
    /// </summary>
    public ReadOnlySeString String {
        get => labelNode.String;
        set => labelNode.String = value;
    }

    /// <summary>
    /// Gets or sets a color that will be set when the user clicks "Default" in the color picker window.
    /// </summary>
    public Vector4? DefaultColor { get; set; }

    /// <summary>
    /// Action to be called if the color picker window is closed, or canceled without confirming a new color.
    /// </summary>
    public Action? OnColorCancelled { get; set; }

    /// <summary>
    /// Action to be called on any color change within the color picker.
    /// </summary>
    /// <remarks>
    /// Use this to show a preview of what the new colors effect will be, without saving the color.
    /// You can then use <see cref="OnColorCancelled"/> to restore the original color.
    /// </remarks>
    public Action<Vector4>? OnColorPreviewed { get; set; }

    /// <summary>
    /// Action to be called when a color is confirmed from the color picker.
    /// </summary>
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
