using System.Drawing;
using System.Numerics;
using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes.NodeBase;

namespace KamiToolKit.Nodes;

public unsafe class ProgressBarNode : NodeBase<AtkResNode> {
    private readonly NineGridNode backgroundImageNode;
    private readonly NineGridNode progressNode;
    private readonly NineGridNode borderImageNode;
    
    private Vector2 ActualSize { get; set; }

    public ProgressBarNode(uint baseId) : base(NodeType.Res) {
        NodeID = baseId;
        InternalNode->SetWidth((ushort)160.0f);
        InternalNode->SetHeight((ushort)20.0f);
        
        backgroundImageNode = new NineGridNode {
            NodeID = 100 + baseId,
            Size = new Vector2(160.0f, 20.0f),
            TextureHeight = 20,
            TextureWidth = 160,
            TextureCoordinates = new Vector2(0.0f, 100.0f),
            NodeFlags = NodeFlags.Visible,
            PartsRenderType = PartsRenderType.RenderType,
            LeftOffset = 15,
            RightOffset = 15,
        };
        
        backgroundImageNode.LoadTexture("ui/uld/Parameter_Gauge_hr1.tex");
        backgroundImageNode.AttachNode(this, NodePosition.AsLastChild);

        progressNode = new NineGridNode {
            NodeID = 200 + baseId,
            Size = new Vector2(160.0f, 20.0f),
            TextureHeight = 20,
            TextureWidth = 160,
            TextureCoordinates = new Vector2(0.0f, 40.0f),
            NodeFlags = NodeFlags.Visible,
            Width = 80,
            Color = KnownColor.Yellow.Vector(),
            LeftOffset = 10,
            RightOffset = 10,
            PartsRenderType = PartsRenderType.RenderType,
        };
        
        progressNode.LoadTexture("ui/uld/Parameter_Gauge_hr1.tex");
        progressNode.AttachNode(this, NodePosition.AsLastChild);

        borderImageNode = new NineGridNode {
            NodeID = 300 + baseId,
            Size = new Vector2(160.0f, 20.0f),
            TextureHeight = 20,
            TextureWidth = 160,
            TextureCoordinates = new Vector2(0.0f, 0.0f),
            NodeFlags = NodeFlags.Visible,
            PartsRenderType = PartsRenderType.RenderType,
            LeftOffset = 15,
            RightOffset = 15,
        };
        
        borderImageNode.LoadTexture("ui/uld/Parameter_Gauge_hr1.tex");
        borderImageNode.AttachNode(this, NodePosition.AsLastChild);
    }

    protected override void Dispose(bool disposing) {
        if (disposing) {
            backgroundImageNode.Dispose();
            progressNode.Dispose();
            borderImageNode.Dispose();
            
            base.Dispose(disposing);
        }
    }

    public float Progress {
        get => progressNode.Width / ActualSize.X;
        set => progressNode.Width = ActualSize.X * value;
    }

    public new float Width {
        get => ActualSize.X;
        set {
            InternalNode->SetWidth((ushort)value);
            backgroundImageNode.Width = value;
            progressNode.Width = value;
            borderImageNode.Width = value;
        }
    }

    public new float Height {
        get => ActualSize.Y;
        set {
            InternalNode->SetHeight((ushort)value);
            backgroundImageNode.Height = value;
            progressNode.Height = value;
            borderImageNode.Height = value;
        }
    }

    public new Vector2 Size {
        get => ActualSize;
        set {
            Width = value.X;
            Height = value.Y;
        }
    }
}