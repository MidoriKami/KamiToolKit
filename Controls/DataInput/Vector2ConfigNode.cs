using System;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Core.Enums;
using KamiToolKit.UI.Nodes;
using Lumina.Data.Parsing.Uld;

namespace KamiToolKit.Controls.DataInput;

/// <summary>
/// Helper node that represents the controls needed to modify a Vector2
/// </summary>
/// <remarks>
/// Has a fixed height of 46px.
/// </remarks>
public class Vector2ConfigNode : ResNode {

    /// <summary>
    /// Gets or sets the action callback when the value is changed.
    /// </summary>
    public Action<Vector2>? OnValueChanged { get; set; }

    /// <summary>
    /// Gets or sets the currently displayed value.
    /// </summary>
    /// <remarks>
    /// Changing this triggers OnValueChanged.
    /// </remarks>
    public Vector2 Value {
        get;
        set {
            field = value;
            WidthInputNode.Value = (int) value.X;
            HeightInputNode.Value = (int) value.Y;
            OnValueChanged?.Invoke(value);
        }
    }

    /// <summary>
    /// Gets or sets the minimum values allowed.
    /// </summary>
    public Vector2 MinValue {
        get;
        set {
            field = value;
            WidthInputNode.Min = (int)value.X;
            HeightInputNode.Min = (int)value.Y;
        }
    }

    /// <summary>
    /// Gets or sets the maximum values allowed.
    /// </summary>
    public Vector2 MaxValue {
        get;
        set {
            field = value;
            WidthInputNode.Max = (int)value.X;
            HeightInputNode.Max = (int)value.Y;
        }
    }

    /// <summary>
    /// Nav index for use with setting contained nodes controller nav values.
    /// </summary>
    /// <remarks>
    /// Must be non-zero to apply nav to contained nodes.
    /// </remarks>
    public int NavIndex {
        get => InputContainerNode.NavIndex;
        set {
            InputContainerNode.NavIndex = value;
            InputContainerNode.UpdateLayout();
        }
    }

    /// <summary>
    /// Nav index for use with setting contained nodes controller nav values.
    /// </summary>
    /// <remarks>
    /// Must be non-zero to apply nav to contained nodes.
    /// </remarks>
    public int NavUp {
        get => InputContainerNode.NavUp;
        set {
            InputContainerNode.NavUp = value;
            InputContainerNode.UpdateLayout();
        }
    }

    /// <summary>
    /// Nav index for use with setting contained nodes controller nav values.
    /// </summary>
    /// <remarks>
    /// Must be non-zero to apply nav to contained nodes.
    /// </remarks>
    public int NavDown {
        get => InputContainerNode.NavDown;
        set {
            InputContainerNode.NavDown = value;
            InputContainerNode.UpdateLayout();
        }
    }

    /// <summary>
    /// Gets the input node used for inputting the Width value.
    /// </summary>
    protected NumericInputNode WidthInputNode { get; }

    /// <summary>
    /// Gets the input node used for inputting the Height value.
    /// </summary>
    protected NumericInputNode HeightInputNode { get; }

    /// <summary>
    /// Gets the layout node used for building this node's layout.
    /// </summary>
    protected VerticalListNode LayoutNode { get; }

    /// <summary>
    /// Gets the layout node used for the numeric input nodes.
    /// </summary>
    protected HorizontalFlexNode InputContainerNode { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="Vector2ConfigNode"/>.
    /// </summary>
    public Vector2ConfigNode() {
        LayoutNode = new VerticalListNode {
            StretchWidth = true,
            InitialNodes = [
                new HorizontalFlexNode {
                    Height = 22.0f,
                    AlignmentFlags = FlexFlags.FitHeight | FlexFlags.FitWidth,
                    InitialNodes = [
                        new TextNode {
                            AlignmentType = AlignmentType.Bottom,
                            TextId = 4030, // "Width"
                            SheetType = NodeData.SheetType.Addon,
                        },
                        new TextNode {
                            AlignmentType = AlignmentType.Bottom,
                            TextId = 4031, // "Height"
                            SheetType = NodeData.SheetType.Addon,
                        },
                    ],
                },
                InputContainerNode = new HorizontalFlexNode {
                    Height = 24.0f,
                    AlignmentFlags = FlexFlags.FitHeight | FlexFlags.FitWidth,
                    InitialNodes = [
                        WidthInputNode = new NumericInputNode {
                            OnValueUpdate = newValue => Value = Value with { X = newValue },
                        },
                        HeightInputNode = new NumericInputNode {
                            OnValueUpdate = newValue => Value = Value with { Y = newValue },
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

        Height = 46.0f;

        LayoutNode.Size = Size;
        LayoutNode.Position = new Vector2(0.0f, 0.0f);
        LayoutNode.UpdateLayout();
    }
}
