using System;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Core.Enums;
using KamiToolKit.UI.Nodes;
using KamiToolKit.UI.Nodes.Components;
using KamiToolKit.UI.Nodes.Layout;
using Lumina.Data.Parsing.Uld;

namespace KamiToolKit.Controls.DataInput;

/// <summary>
/// Helper node that represents the controls needed to modify a Vector4
/// </summary>
/// <remarks>
/// Has a fixed height of 92px.
/// </remarks>
public class Vector4ConfigNode : ResNode {

    /// <summary>
    /// Gets or sets the action callback when the value is changed.
    /// </summary>
    public Action<Vector4>? OnValueChanged { get; set; }

    /// <summary>
    /// Gets or sets the currently displayed value.
    /// </summary>
    /// <remarks>
    /// Changing this triggers OnValueChanged.
    /// Ordering is (Left, Top, Right, Bottom)
    /// </remarks>
    public Vector4 Value {
        get;
        set {
            field = value;
            LeftInputNode.Value = (int)value.X;
            TopInputNode.Value = (int)value.Y;
            RightInputNode.Value = (int)value.Z;
            BottomInputNode.Value = (int)value.W;
            OnValueChanged?.Invoke(Value);
        }
    }

    /// <summary>
    /// Gets or sets the minimum values allowed.
    /// </summary>
    public Vector4 MinValue {
        get;
        set {
            field = value;
            LeftInputNode.Min = (int)value.X;
            TopInputNode.Min = (int)value.Y;
            RightInputNode.Min = (int)value.Z;
            BottomInputNode.Min = (int)value.W;
        }
    }

    /// <summary>
    /// Gets or sets the maximum values allowed.
    /// </summary>
    public Vector4 MaxValue {
        get;
        set {
            field = value;
            LeftInputNode.Max = (int)value.X;
            TopInputNode.Max = (int)value.Y;
            RightInputNode.Max = (int)value.Z;
            BottomInputNode.Max = (int)value.W;
        }
    }

    /// <summary>
    /// Gets the input node used for inputting the Width value.
    /// </summary>
    protected NumericInputNode LeftInputNode { get; }

    /// <summary>
    /// Gets the input node used for inputting the Height value.
    /// </summary>
    protected NumericInputNode RightInputNode { get; }

    /// <summary>
    /// Gets the input node used for inputting the Width value.
    /// </summary>
    protected NumericInputNode TopInputNode { get; }

    /// <summary>
    /// Gets the input node used for inputting the Height value.
    /// </summary>
    protected NumericInputNode BottomInputNode { get; }

    /// <summary>
    /// Gets the layout node used for building this node's layout.
    /// </summary>
    protected VerticalListNode LayoutNode { get; }

    /// <summary>
    /// Gets the layout node used for the Left/Right input controls.
    /// </summary>
    protected HorizontalFlexNode LeftRightInputContainerNode { get; }

    /// <summary>
    /// Gets the layout node used for the Top/Bottom input controls.
    /// </summary>
    protected HorizontalFlexNode TopBottomInputContainerNode { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="Vector4ConfigNode"/>.
    /// </summary>
    public Vector4ConfigNode() {
        LayoutNode = new VerticalListNode {
            StretchWidth = true,
            InitialNodes = [
                new HorizontalFlexNode {
                    Height = 22.0f,
                    AlignmentFlags = FlexFlags.FitHeight | FlexFlags.FitWidth,
                    InitialNodes = [
                        new TextNode {
                            AlignmentType = AlignmentType.Bottom,
                            TextId = 14036, // "Left"
                            SheetType = NodeData.SheetType.Addon,
                        },
                        new TextNode {
                            AlignmentType = AlignmentType.Bottom,
                            TextId = 14037, // "Right"
                            SheetType = NodeData.SheetType.Addon,
                        },
                    ],
                },
                LeftRightInputContainerNode = new HorizontalFlexNode {
                    Height = 24.0f,
                    AlignmentFlags = FlexFlags.FitHeight | FlexFlags.FitWidth,
                    InitialNodes = [
                        LeftInputNode = new NumericInputNode {
                            OnValueUpdate = newValue => Value = Value with { X = newValue },
                        },
                        RightInputNode = new NumericInputNode {
                            OnValueUpdate = newValue => Value = Value with { Z = newValue },
                        },
                    ],
                },
                new ResNode{ Height = 4.0f },
                new HorizontalFlexNode {
                    Height = 22.0f,
                    AlignmentFlags = FlexFlags.FitHeight | FlexFlags.FitWidth,
                    InitialNodes = [
                        new TextNode {
                            AlignmentType = AlignmentType.Bottom,
                            String = "Top", // Seems there's no sheet entry we can use to auto translate "Top".
                        },
                        new TextNode {
                            AlignmentType = AlignmentType.Bottom,
                            String = "Bottom", // Seems there's no sheet entry we can use to auto translate "Bottom".
                        },
                    ],
                },
                TopBottomInputContainerNode = new HorizontalFlexNode {
                    Height = 24.0f,
                    AlignmentFlags = FlexFlags.FitHeight | FlexFlags.FitWidth,
                    InitialNodes = [
                        TopInputNode = new NumericInputNode {
                            OnValueUpdate = newValue => Value = Value with { Y = newValue },
                        },
                        BottomInputNode = new NumericInputNode {
                            OnValueUpdate = newValue => Value = Value with { W = newValue },
                        },
                    ],
                },
            ],
        };

        LayoutNode.AttachNode(this);
    }

    /// <inheritdoc/>
    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        Height = 96.0f;

        LayoutNode.Size = Size;
        LayoutNode.Position = new Vector2(0.0f, 0.0f);
        LayoutNode.UpdateLayout();
    }
}
