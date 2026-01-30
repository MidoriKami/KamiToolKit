using System;
using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Common.Math;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Enums;
using Bounds = KamiToolKit.Classes.Bounds;
using Vector2 = System.Numerics.Vector2;
using Vector3 = System.Numerics.Vector3;
using Vector4 = System.Numerics.Vector4;

namespace KamiToolKit;

public abstract unsafe partial class NodeBase {
    public virtual float X {
        get => ResNode->GetXFloat();
        set => ResNode->SetXFloat(value);
    }

    public virtual float Y {
        get => ResNode->GetYFloat();
        set => ResNode->SetYFloat(value);
    }

    public virtual Vector2 Position {
        get => ResNode->Position;
        set => ResNode->Position = value;
    }

    public virtual float ScreenX {
        get => ResNode->ScreenX;
        set => ResNode->ScreenX = value;
    }

    public virtual float ScreenY {
        get => ResNode->ScreenY;
        set => ResNode->ScreenY = value;
    }

    public virtual Vector2 ScreenPosition 
        => ResNode->ScreenPosition;

    public virtual float Width {
        get => ResNode->GetWidth();
        set {
            ResNode->SetWidth((ushort)value);
            OnSizeChanged();
        }
    }

    public virtual float Height {
        get => ResNode->GetHeight();
        set {
            ResNode->SetHeight((ushort)value);
            OnSizeChanged();
        }
    }

    public virtual Vector2 Size {
        get => ResNode->Size;
        set {
            ResNode->SetWidth((ushort)value.X);
            ResNode->SetHeight((ushort)value.Y);
            OnSizeChanged();
        }
    }

    public Bounds Bounds 
        => ResNode->Bounds;

    public Vector2 Center
        => ResNode->Center;

    public virtual float ScaleX {
        get => ResNode->GetScaleX();
        set => ResNode->SetScaleX(value);
    }

    public virtual float ScaleY {
        get => ResNode->GetScaleY();
        set => ResNode->SetScaleY(value);
    }

    public virtual Vector2 Scale {
        get => ResNode->Scale;
        set => ResNode->Scale = value;
    }

    public virtual float Rotation {
        get => ResNode->GetRotation();
        set => ResNode->SetRotation(value);
    }

    public virtual float RotationDegrees {
        get => ResNode->RotationDegrees;
        set => ResNode->RotationDegrees = value;
    }

    public virtual float OriginX {
        get => ResNode->OriginX;
        set => ResNode->OriginX = value;
    }

    public virtual float OriginY {
        get => ResNode->OriginY;
        set => ResNode->OriginY = value;
    }

    public virtual Vector2 Origin {
        get => ResNode->Origin;
        set => ResNode->Origin = value;
    }

    private bool? lastIsVisible;
    
    public virtual bool IsVisible {
        get => ResNode->Visible;
        set {
            ResNode->Visible = value;
            if (lastIsVisible is null || lastIsVisible != value) {
                OnVisibilityToggled?.Invoke(value);
                lastIsVisible = value;
            }
        }
    }

    private Action<bool>? OnVisibilityToggled { get; set; }

    public NodeFlags NodeFlags {
        get => ResNode->NodeFlags;
        set => ResNode->NodeFlags = value;
    }

    public virtual Vector4 Color {
        get => ResNode->ColorVector;
        set => ResNode->ColorVector = value;
    }

    public virtual ColorHelpers.HsvaColor ColorHsva {
        get => ResNode->ColorHsva;
        set => ResNode->ColorHsva = value;
    }

    public virtual float Alpha {
        get => ResNode->Color.A;
        set => ResNode->SetAlpha((byte)(value * 255.0f));
    }

    public virtual Vector3 AddColor {
        get => ResNode->AddColor;
        set => ResNode->AddColor = value;
    }

    public virtual ColorHelpers.HsvaColor AddColorHsva {
        get => ResNode->AddColorHsva;
        set => ResNode->AddColorHsva = value;
    }

    public virtual Vector3 MultiplyColor {
        get => ResNode->MultiplyColor;
        set => ResNode->MultiplyColor = value;
    }

    public virtual ColorHelpers.HsvaColor MultiplyColorHsva {
        get => ResNode->MultiplyColorHsva;
        set => ResNode->MultiplyColorHsva = value;
    }

    public uint NodeId {
        get => ResNode->NodeId;
        set => ResNode->NodeId = value;
    }

    public virtual DrawFlags DrawFlags {
        get => (DrawFlags) ResNode->DrawFlags;
        set => ResNode->DrawFlags = (uint) value & 0b1111_1111_1111_1100_0000_0011_1111_1111 | 
                                            ResNode->DrawFlags & 0b0000_0000_0000_0011_1111_1100_0000_0000;
    }

    public virtual int ClipCount {
        get => (int)((ResNode->DrawFlags & 0b0000_0000_0000_0011_1111_1100_0000_0000) >> 10);
        set => ResNode->DrawFlags = (uint)(value << 10 & 0b0000_0000_0000_0011_1111_1100_0000_0000) 
                                            | ResNode->DrawFlags & 0b1111_1111_1111_1100_0000_0011_1111_1111;
    }

    public void AddDrawFlags(params DrawFlags[] flags) {
        foreach (var flag in flags) {
            DrawFlags |= flag;
        }
    }

    public void RemoveDrawFlags(params DrawFlags[] flags) {
        foreach (var flag in flags) {
            DrawFlags &= ~flag;
        }
    }

    public int Priority {
        get => ResNode->GetPriority();
        set => ResNode->SetPriority((ushort)value);
    }

    protected virtual NodeType NodeType {
        get => ResNode->GetNodeType();
        set => ResNode->Type = value;
    }

    public virtual int ChildCount 
        => ResNode->ChildCount;

    protected virtual void OnSizeChanged() { }

    public void AddNodeFlags(params NodeFlags[] flags) {
        foreach (var flag in flags) {
            NodeFlags |= flag;
        }
    }

    public void RemoveNodeFlags(params NodeFlags[] flags) {
        foreach (var flag in flags) {
            NodeFlags &= ~flag;
        }
    }

    public void MarkDirty() {
        foreach (var child in GetAllChildren(this)) {
            child.ResNode->AddDrawFlag( [ DrawFlags.IsDirty ] );
        }
        ResNode->AddDrawFlag([ DrawFlags.IsDirty ] );
    }

    public bool CheckCollision(short x, short y, bool inclusive = true)
        => ResNode->CheckCollision(x, y, inclusive);
    
    public bool CheckCollision(Vector2 position, bool inclusive = true)
        => ResNode->CheckCollision((short) position.X, (short) position.Y, inclusive);

    public bool CheckCollision(AtkEventData* eventData, bool inclusive = true)
        => ResNode->CheckCollision(eventData, inclusive);

    public Matrix2x2 Transform {
        get => ResNode->Transform;
        set => ResNode->Transform = value;
    }
}
