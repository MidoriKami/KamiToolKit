using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Extensions;
using Newtonsoft.Json;

namespace KamiToolKit.System;

[JsonObject(MemberSerialization.OptIn)]
public abstract unsafe partial class NodeBase {
    public virtual float X {
        get => InternalResNode->GetXFloat();
        set => InternalResNode->SetXFloat(value);
    }

    public virtual float Y {
        get => InternalResNode->GetYFloat();
        set => InternalResNode->SetYFloat(value);
    }

    [JsonProperty] public virtual Vector2 Position {
        get => new(X, Y);
        set => InternalResNode->SetPositionFloat(value.X, value.Y);
    }

    public virtual float ScreenX {
        get => InternalResNode->ScreenX;
        set => InternalResNode->ScreenX = value;
    }

    public virtual float ScreenY {
        get => InternalResNode->ScreenY;
        set => InternalResNode->ScreenY = value;
    }

    public virtual Vector2 ScreenPosition {
        get => new(ScreenX, ScreenY);
        set {
            ScreenX = value.X;
            ScreenY = value.Y;
        }
    }

    public virtual float Width {
        get => InternalResNode->GetWidth();
        set {
            InternalResNode->SetWidth((ushort)value);
            OnSizeChanged();
        }
    }

    public virtual float Height {
        get => InternalResNode->GetHeight();
        set {
            InternalResNode->SetHeight((ushort)value);
            OnSizeChanged();
        }
    }

    [JsonProperty] public virtual Vector2 Size {
        get => new(Width, Height);
        set {
            Width = value.X;
            Height = value.Y;
        }
    }

    public virtual float ScaleX {
        get => InternalResNode->GetScaleX();
        set => InternalResNode->SetScaleX(value);
    }

    public virtual float ScaleY {
        get => InternalResNode->GetScaleY();
        set => InternalResNode->SetScaleY(value);
    }

    [JsonProperty] public virtual Vector2 Scale {
        get => new(ScaleX, ScaleY);
        set => InternalResNode->SetScale(value.X, value.Y);
    }

    public virtual float Rotation {
        get => InternalResNode->Rotation;
        set {
            InternalResNode->Rotation = value;
            DrawFlags |= 5;
        }
    }

    public virtual float OriginX {
        get => InternalResNode->OriginX;
        set => InternalResNode->OriginX = value;
    }

    public virtual float OriginY {
        get => InternalResNode->OriginY;
        set => InternalResNode->OriginY = value;
    }

    public virtual Vector2 Origin {
        get => new(OriginX, OriginY);
        set {
            OriginX = value.X;
            OriginY = value.Y;
        }
    }

    [JsonProperty] public virtual bool IsVisible {
        get => InternalResNode->IsVisible();
        set => InternalResNode->ToggleVisibility(value);
    }

    public NodeFlags NodeFlags {
        get => InternalResNode->NodeFlags;
        set => InternalResNode->NodeFlags = value;
    }

    [JsonProperty] public virtual Vector4 Color {
        get => InternalResNode->Color.ToVector4();
        set => InternalResNode->Color = value.ToByteColor();
    }

    public virtual float Alpha {
        get => InternalResNode->Color.A;
        set => InternalResNode->SetAlpha((byte)(value * 255.0f));
    }

    [JsonProperty] public virtual Vector3 AddColor {
        get => new Vector3(InternalResNode->AddRed, InternalResNode->AddGreen, InternalResNode->AddBlue) / 255.0f;
        set {
            InternalResNode->AddRed = (short)(value.X * 255);
            InternalResNode->AddGreen = (short)(value.Y * 255);
            InternalResNode->AddBlue = (short)(value.Z * 255);
        }
    }

    [JsonProperty] public virtual Vector3 MultiplyColor {
        get => new Vector3(InternalResNode->MultiplyRed, InternalResNode->MultiplyGreen, InternalResNode->MultiplyBlue) / 255.0f;
        set {
            InternalResNode->MultiplyRed = (byte)(value.X * 255);
            InternalResNode->MultiplyGreen = (byte)(value.Y * 255);
            InternalResNode->MultiplyBlue = (byte)(value.Z * 255);
        }
    }

    public uint NodeId {
        get => InternalResNode->NodeId;
        set => InternalResNode->NodeId = value;
    }

    public virtual uint DrawFlags {
        get => InternalResNode->DrawFlags;
        internal set => InternalResNode->DrawFlags = value;
    }

    public int Priority {
        get => InternalResNode->GetPriority();
        set => InternalResNode->SetPriority((ushort)value);
    }

    protected virtual NodeType NodeType {
        get => InternalResNode->GetNodeType();
        set => InternalResNode->Type = value;
    }

    public virtual int ChildCount => InternalResNode->ChildCount;

    protected virtual void OnSizeChanged() { }

    public void AddFlags(params NodeFlags[] flags) {
        foreach (var flag in flags) {
            AddFlags(flag);
        }
    }

    public void AddFlags(NodeFlags flags)
        => NodeFlags |= flags;

    public void RemoveFlags(params NodeFlags[] flags) {
        foreach (var flag in flags) {
            RemoveFlags(flag);
        }
    }

    public void RemoveFlags(NodeFlags flags)
        => NodeFlags &= ~flags;

    public void MarkDirty()
        => VisitChildren(InternalResNode, pointer => pointer.Value->DrawFlags |= 1);

    public bool CheckCollision(short x, short y)
        => InternalResNode->CheckCollisionAtCoords(x, y, true);

    public bool CheckCollision(AtkEventData* eventData)
        => CheckCollision(eventData->MouseData.PosX, eventData->MouseData.PosY);
}
