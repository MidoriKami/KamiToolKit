using System.Drawing;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.System;
using Newtonsoft.Json;

namespace KamiToolKit.Nodes;

[JsonObject(MemberSerialization.OptIn)]
public unsafe class ProgressBarNode : NodeBase<AtkResNode> {
    [JsonProperty] private readonly SimpleNineGridNode backgroundImageNode;
    [JsonProperty] private readonly SimpleNineGridNode progressNode;
    [JsonProperty] private readonly SimpleNineGridNode borderImageNode;

    private Vector2 actualSize;

    public ProgressBarNode(uint baseId) : base(NodeType.Res) {
        NodeId = baseId;
        InternalNode->SetWidth((ushort)160.0f);
        InternalNode->SetHeight((ushort)20.0f);
        
        backgroundImageNode = new SimpleNineGridNode {
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
        
        backgroundImageNode.AttachNode(this, NodePosition.AsLastChild);

        progressNode = new SimpleNineGridNode {
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
        
        progressNode.AttachNode(this, NodePosition.AsLastChild);

        borderImageNode = new SimpleNineGridNode {
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
        get => progressNode.Width / actualSize.X;
        set => progressNode.Width = actualSize.X * value;
    }

    public new float Width {
        get => actualSize.X;
        set {
            InternalNode->SetWidth((ushort)value);
            backgroundImageNode.Width = value;
            progressNode.Width = value;
            borderImageNode.Width = value;
            actualSize.X = value;
        }
    }

    public new float Height {
        get => actualSize.Y;
        set {
            InternalNode->SetHeight((ushort)value);
            backgroundImageNode.Height = value;
            progressNode.Height = value;
            borderImageNode.Height = value;
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
        get => backgroundImageNode.Color;
        set => backgroundImageNode.Color = value;
    }

    public Vector4 BarColor {
        get => progressNode.Color;
        set => progressNode.Color = value;
    }

    public override void DrawConfig() {
        base.DrawConfig();

        using (var background = ImRaii.TreeNode("Background")) {
            if (background) {
                backgroundImageNode.DrawConfig();
            }
        }

        using (var progress = ImRaii.TreeNode("Progress")) {
            if (progress) {
                progressNode.DrawConfig();
            }
        }

        using (var border = ImRaii.TreeNode("Border")) {
            if (border) {
                borderImageNode.DrawConfig();
            }
        }
    }
}