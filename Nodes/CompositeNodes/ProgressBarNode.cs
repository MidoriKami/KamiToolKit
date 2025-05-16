using System.Drawing;
using System.Numerics;
using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.System;

namespace KamiToolKit.Nodes;

public unsafe class ProgressBarNode : NodeBase<AtkResNode> {
    public readonly SimpleNineGridNode BackgroundImageNode;
    public readonly SimpleNineGridNode ProgressNode;
    public readonly SimpleNineGridNode BorderImageNode;

    private Vector2 actualSize;

    public ProgressBarNode(uint baseId) : base(NodeType.Res) {
        NodeId = baseId;
        InternalNode->SetWidth((ushort)160.0f);
        InternalNode->SetHeight((ushort)20.0f);
        
        BackgroundImageNode = new SimpleNineGridNode {
            NodeId = 100 + baseId,
            Size = new Vector2(160.0f, 20.0f),
            TextureSize = new Vector2(160.0f, 20.0f),
            TextureCoordinates = new Vector2(0.0f, 100.0f),
            NodeFlags = NodeFlags.Visible,
            PartsRenderType = 4,
            LeftOffset = 15,
            RightOffset = 15,
            TexturePath = "ui/uld/Parameter_Gauge_hr1.tex",
        };
        
        BackgroundImageNode.AttachNode(this, NodePosition.AsLastChild);

        ProgressNode = new SimpleNineGridNode {
            NodeId = 200 + baseId,
            Size = new Vector2(160.0f, 20.0f),
            TextureSize = new Vector2(160.0f, 20.0f),
            TextureCoordinates = new Vector2(0.0f, 40.0f),
            NodeFlags = NodeFlags.Visible,
            Width = 80,
            Color = KnownColor.Yellow.Vector(),
            LeftOffset = 10,
            RightOffset = 10,
            PartsRenderType = 4,
            TexturePath = "ui/uld/Parameter_Gauge_hr1.tex",
        };
        
        ProgressNode.AttachNode(this, NodePosition.AsLastChild);

        BorderImageNode = new SimpleNineGridNode {
            NodeId = 300 + baseId,
            Size = new Vector2(160.0f, 20.0f),
            TextureSize = new Vector2(160.0f, 20.0f),
            TextureCoordinates = new Vector2(0.0f, 0.0f),
            NodeFlags = NodeFlags.Visible,
            PartsRenderType = 4,
            LeftOffset = 15,
            RightOffset = 15,
            TexturePath = "ui/uld/Parameter_Gauge_hr1.tex",
        };
        
        BorderImageNode.AttachNode(this, NodePosition.AsLastChild);
    }

    protected override void Dispose(bool disposing) {
        if (disposing) {
            BackgroundImageNode.Dispose();
            ProgressNode.Dispose();
            BorderImageNode.Dispose();
            
            base.Dispose(disposing);
        }
    }

    public float Progress {
        get => ProgressNode.Width / actualSize.X;
        set => ProgressNode.Width = actualSize.X * value;
    }

    public new float Width {
        get => actualSize.X;
        set {
            InternalNode->SetWidth((ushort)value);
            BackgroundImageNode.Width = value;
            ProgressNode.Width = value;
            BorderImageNode.Width = value;
            actualSize.X = value;
        }
    }

    public new float Height {
        get => actualSize.Y;
        set {
            InternalNode->SetHeight((ushort)value);
            BackgroundImageNode.Height = value;
            ProgressNode.Height = value;
            BorderImageNode.Height = value;
            actualSize.Y = value;
        }
    }

    public new Vector2 Size {
        get => actualSize;
        set {
            Width = value.X;
            Height = value.Y;
        }
    }

    public Vector4 BackgroundColor {
        get => BackgroundImageNode.Color;
        set => BackgroundImageNode.Color = value;
    }

    public Vector4 BarColor {
        get => ProgressNode.Color;
        set => ProgressNode.Color = value;
    }
}