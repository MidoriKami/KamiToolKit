using System;
using System.Numerics;
using Dalamud.Utility.Numerics;
using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiToolKit;

public unsafe class ResNode : IDisposable {
    private readonly AtkResNode* internalNode;

    public ResNode() {
        internalNode = IMemorySpace.GetUISpace()->Create<AtkResNode>();

        if (internalNode is null) {
            throw new Exception("Unable to create memory for AtkResNode");
        }
    }

    public void Dispose() {
        IMemorySpace.Free(internalNode);
    }
    
    public float X {
        get => internalNode->GetX();
        set => internalNode->SetX(value);
    }

    public float Y {
        get => internalNode->GetY();
        set => internalNode->SetY(value);
    }

    public Vector2 Position {
        get => new(X, Y);
        set => internalNode->SetPositionFloat(value.X, value.Y);
    }

    public float ScreenX {
        get => internalNode->ScreenX;
        set => internalNode->ScreenX = value;
    }

    public float ScreenY {
        get => internalNode->ScreenY;
        set => internalNode->ScreenY = value;
    }

    public Vector2 ScreenPosition {
        get => new(ScreenX, ScreenY);
        set {
            ScreenX = value.X;
            ScreenY = value.Y;
        }
    }
    
    public float Width {
        get => internalNode->GetWidth();
        set => internalNode->SetWidth((ushort)value);
    }

    public float Height {
        get => internalNode->Height;
        set => internalNode->SetHeight((ushort) value);
    }
    
    public Vector2 Size {
        get => new(Width, Height);
        set {
            Width = value.X;
            Height = value.Y;
        }
    }

    private float ScaleX {
        get => internalNode->GetScaleX();
        set => internalNode->SetScaleX(value);
    }

    private float ScaleY {
        get => internalNode->GetScaleY();
        set => internalNode->SetScaleY(value);
    }
    
    public Vector2 Scale {
        get => new(ScaleX, ScaleY);
        set => internalNode->SetScale(value.X, value.Y);
    }

    public float Rotation {
        get => internalNode->Rotation;
        set => internalNode->Rotation = value;
    }

    private float OriginX {
        get => internalNode->OriginX;
        set => internalNode->OriginX = value;
    }

    private float OriginY {
        get => internalNode->OriginY;
        set => internalNode->OriginY = value;
    }

    private Vector2 Origin {
        get => new(OriginX, OriginY);
        set {
            OriginX = value.X;
            OriginY = value.Y;
        }
    }

    public bool IsVisible {
        get => internalNode->IsVisible;
        set => internalNode->ToggleVisibility(value);
    }

    public NodeFlags NodeFlags {
        get => internalNode->NodeFlags;
        set => internalNode->NodeFlags = value;
    }

    public void AddFlags(NodeFlags flags)
        => NodeFlags |= flags;

    public void RemoveFlags(NodeFlags flags)
        => NodeFlags &= ~flags;

    public Vector4 Color {
        get => new(internalNode->Color.R * 255.0f, internalNode->Color.G * 255.0f, internalNode->Color.B * 255.0f, internalNode->Color.A * 255.0f);
        set => internalNode->Color = value.ToByteColor();
    }
    
    public virtual NodeType NodeType => NodeType.Res;
    
    public uint NodeID { get; set; }
}