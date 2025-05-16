using System.Numerics;
using Dalamud.Utility.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Extensions;
using Newtonsoft.Json;

namespace KamiToolKit.System;

public abstract unsafe partial class NodeBase {
    [JsonIgnore] public float X {
        get => InternalResNode->GetXFloat();
        set => InternalResNode->SetXFloat(value);
    }

    [JsonIgnore] public float Y {
        get => InternalResNode->GetYFloat();
        set => InternalResNode->SetYFloat(value);
    }

    public Vector2 Position {
        get => new(X, Y);
        set => InternalResNode->SetPositionFloat(value.X, value.Y);
    }

    [JsonIgnore] public float ScreenX {
        get => InternalResNode->ScreenX;
        set => InternalResNode->ScreenX = value;
    }

    [JsonIgnore] public float ScreenY {
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
    
    [JsonIgnore] public float Width {
        get => InternalResNode->GetWidth();
        set => InternalResNode->SetWidth((ushort)value);
    }

    [JsonIgnore] public float Height {
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

    [JsonIgnore] private float ScaleX {
        get => InternalResNode->GetScaleX();
        set => InternalResNode->SetScaleX(value);
    }

    [JsonIgnore] private float ScaleY {
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

    [JsonIgnore] public float OriginX {
        get => InternalResNode->OriginX;
        set => InternalResNode->OriginX = value;
    }

    [JsonIgnore] public float OriginY {
        get => InternalResNode->OriginY;
        set => InternalResNode->OriginY = value;
    }

    public Vector2 Origin {
        get => new(OriginX, OriginY);
        set {
            OriginX = value.X;
            OriginY = value.Y;
        }
    }

    public bool IsVisible {
        get => InternalResNode->IsVisible();
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
        get => InternalResNode->Color.ToVector4();
        set => InternalResNode->Color = value.ToByteColor();
    }

    [JsonIgnore] public float Alpha {
        get => InternalResNode->Color.A;
        set => InternalResNode->Color.A = (byte)(value * 255.0f);
    }

    public Vector3 AddColor {
        get => new Vector3(InternalResNode->AddRed, InternalResNode->AddGreen, InternalResNode->AddBlue) /  255.0f;
        set {
            InternalResNode->AddRed = (short)(value.X * 255);
            InternalResNode->AddGreen = (short)(value.Y * 255);
            InternalResNode->AddBlue = (short)(value.Z * 255);
        }
    }

    public Vector3 MultiplyColor {
        get => new Vector3(InternalResNode->MultiplyRed, InternalResNode->MultiplyGreen, InternalResNode->MultiplyBlue) / 255.0f;
        set {
            InternalResNode->MultiplyRed = (byte)(value.X * 255);
            InternalResNode->MultiplyGreen = (byte)(value.Y * 255);
            InternalResNode->MultiplyBlue = (byte)(value.Z * 255);
        }
    }

    internal uint ChildCount {
        get => InternalResNode->ChildCount;
        private set => InternalResNode->ChildCount = (ushort)value;
    }

    private AtkResNode* ParentNode {
        get => InternalResNode->ParentNode;
        set => InternalResNode->ParentNode = value;
    }
    
    private AtkResNode* PrevSiblingNode {
        get => InternalResNode->PrevSiblingNode;
        set => InternalResNode->PrevSiblingNode = value;
    }
    
    private AtkResNode* NextSiblingNode {
        get => InternalResNode->NextSiblingNode;
        set => InternalResNode->NextSiblingNode = value;
    }
    
    private AtkResNode* ChildNode {
        get => InternalResNode->ChildNode;
        set => InternalResNode->ChildNode = value;
    }
    
    [JsonIgnore] public uint NodeId {
        get => InternalResNode->NodeId;
        set => InternalResNode->NodeId = value;
    }

    public uint DrawFlags {
        get => InternalResNode->DrawFlags;
        private set => InternalResNode->DrawFlags = value;
    }

    protected NodeType NodeType {
        get => InternalResNode->Type;
        set => InternalResNode->Type = value;
    }
}