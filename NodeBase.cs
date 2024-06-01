using System.Numerics;
using Dalamud.Utility.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiToolKit;

public abstract unsafe class NodeBase<T> where T : unmanaged {
    protected abstract T* InternalNode { get; set; }

    private AtkResNode* InternalResNode => (AtkResNode*) InternalNode;
    
    public float X {
        get => InternalResNode->GetX();
        set => InternalResNode->SetX(value);
    }

    public float Y {
        get => InternalResNode->GetY();
        set => InternalResNode->SetY(value);
    }

    public Vector2 Position {
        get => new(X, Y);
        set => InternalResNode->SetPositionFloat(value.X, value.Y);
    }

    public float ScreenX {
        get => InternalResNode->ScreenX;
        set => InternalResNode->ScreenX = value;
    }

    public float ScreenY {
        get => InternalResNode->ScreenY;
        set => InternalResNode->ScreenY = value;
    }

    public Vector2 ScreenPosition {
        get => new(ScreenX, ScreenY);
        set {
            ScreenX = value.X;
            ScreenY = value.Y;
        }
    }
    
    public float Width {
        get => InternalResNode->GetWidth();
        set => InternalResNode->SetWidth((ushort)value);
    }

    public float Height {
        get => InternalResNode->Height;
        set => InternalResNode->SetHeight((ushort) value);
    }
    
    public Vector2 Size {
        get => new(Width, Height);
        set {
            Width = value.X;
            Height = value.Y;
        }
    }

    private float ScaleX {
        get => InternalResNode->GetScaleX();
        set => InternalResNode->SetScaleX(value);
    }

    private float ScaleY {
        get => InternalResNode->GetScaleY();
        set => InternalResNode->SetScaleY(value);
    }
    
    public Vector2 Scale {
        get => new(ScaleX, ScaleY);
        set => InternalResNode->SetScale(value.X, value.Y);
    }

    public float Rotation {
        get => InternalResNode->Rotation;
        set => InternalResNode->Rotation = value;
    }

    private float OriginX {
        get => InternalResNode->OriginX;
        set => InternalResNode->OriginX = value;
    }

    private float OriginY {
        get => InternalResNode->OriginY;
        set => InternalResNode->OriginY = value;
    }

    private Vector2 Origin {
        get => new(OriginX, OriginY);
        set {
            OriginX = value.X;
            OriginY = value.Y;
        }
    }

    public bool IsVisible {
        get => InternalResNode->IsVisible;
        set => InternalResNode->ToggleVisibility(value);
    }

    public NodeFlags NodeFlags {
        get => InternalResNode->NodeFlags;
        set => InternalResNode->NodeFlags = value;
    }

    public void AddFlags(NodeFlags flags)
        => NodeFlags |= flags;

    public void RemoveFlags(NodeFlags flags)
        => NodeFlags &= ~flags;

    public Vector4 Color {
        get => new(InternalResNode->Color.R * 255.0f, InternalResNode->Color.G * 255.0f, InternalResNode->Color.B * 255.0f, InternalResNode->Color.A * 255.0f);
        set => InternalResNode->Color = value.ToByteColor();
    }
    
    public abstract NodeType NodeType { get; }
    
    public uint NodeID { get; set; }
}