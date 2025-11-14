using System;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;

namespace KamiToolKit.Widgets;

public class Vector2EditWidget : SimpleComponentNode {
    public readonly GridNode GridNode;
    public readonly TextNode WidthTextNode;
    public readonly TextNode HeightTextNode;
    public readonly NumericInputNode WidthInputNode;
    public readonly NumericInputNode HeightInputNode;

    public Vector2EditWidget() {
        GridNode = new GridNode {
            GridSize = new GridSize(2, 2),
        };
        GridNode.AttachNode(this);

        WidthTextNode = new TextNode {
            AlignmentType = AlignmentType.Bottom,
            FontType = FontType.Axis,
            FontSize = 14,
            LineSpacing = 14,
            TextColor = ColorHelper.GetColor(8),
            TextOutlineColor = ColorHelper.GetColor(7),
            TextFlags = TextFlags.Edge | TextFlags.AutoAdjustNodeSize,
            String = XLabel ?? "Width",
        };
        WidthTextNode.AttachNode(GridNode[0, 0]);

        HeightTextNode = new TextNode {
            AlignmentType = AlignmentType.Bottom,
            FontType = FontType.Axis,
            FontSize = 14,
            LineSpacing = 14,
            TextColor = ColorHelper.GetColor(8),
            TextOutlineColor = ColorHelper.GetColor(7),
            TextFlags = TextFlags.Edge | TextFlags.AutoAdjustNodeSize,
            String = YLabel ?? "Height",
        };
        HeightTextNode.AttachNode(GridNode[1, 0]);
        
        WidthInputNode = new NumericInputNode {
            Position = new Vector2(2.0f, 2.0f),
            OnValueUpdate = OnXValueUpdated,
        };
        WidthInputNode.AttachNode(GridNode[0, 1]);
        
        HeightInputNode = new NumericInputNode {
            Position = new Vector2(2.0f, 2.0f),
            OnValueUpdate = OnYValueUpdated,
        };
        HeightInputNode.AttachNode(GridNode[1, 1]);
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        GridNode.Size = Size;
        
        WidthTextNode.Size = GridNode[0, 0].Size;
        HeightTextNode.Size = GridNode[1, 0].Size;
        
        WidthInputNode.Size = GridNode[0, 1].Size;
        HeightInputNode.Size = GridNode[1, 1].Size;
    }

    private void OnXValueUpdated(int newValue) {
        Value = Value with { X = newValue };
        OnValueChanged?.Invoke(Value);
    }
    
    private void OnYValueUpdated(int newValue) {
        Value = Value with { Y = newValue };
        OnValueChanged?.Invoke(Value);
    }

    public Vector2 Value {
        get;
        set {
            field = value;
            WidthInputNode.Value = (int) value.X;
            HeightInputNode.Value = (int) value.Y;
        }
    }
    
    public Action<Vector2>? OnValueChanged { get; set; }

    public string? XLabel { get; set; }
    public string? YLabel { get; set; }
}
