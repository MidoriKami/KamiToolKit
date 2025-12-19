using System;
using System.Numerics;
using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Common.Math;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
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
        get => new(X, Y);
        set => ResNode->SetPositionFloat(value.X, value.Y);
    }

    public virtual float ScreenX {
        get => ResNode->ScreenX;
        set => ResNode->ScreenX = value;
    }

    public virtual float ScreenY {
        get => ResNode->ScreenY;
        set => ResNode->ScreenY = value;
    }

    public virtual Vector2 ScreenPosition {
        get => new(ScreenX, ScreenY);
        set {
            ScreenX = value.X;
            ScreenY = value.Y;
        }
    }

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
        get => new(Width, Height);
        set {
            Width = value.X;
            Height = value.Y;
        }
    }

    public Bounds Bounds => new() { TopLeft = Position, BottomRight = Position + Size };

    public Vector2 Center => Position + Size / 2f;

    public virtual float ScaleX {
        get => ResNode->GetScaleX();
        set => ResNode->SetScaleX(value);
    }

    public virtual float ScaleY {
        get => ResNode->GetScaleY();
        set => ResNode->SetScaleY(value);
    }

    public virtual Vector2 Scale {
        get => new(ScaleX, ScaleY);
        set => ResNode->SetScale(value.X, value.Y);
    }

    public virtual float Rotation {
        get => ResNode->GetRotation();
        set => ResNode->SetRotation(value);
    }

    public virtual float RotationDegrees {
        get => ResNode->GetRotationDegrees();
        set => ResNode->SetRotationDegrees(value - (int)(value / 360.0f) * 360.0f);
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
        get => new(OriginX, OriginY);
        set => ResNode->SetOrigin(value.X, value.Y);
    }

    public bool IsVisible {
        get => ResNode->IsVisible();
        set {
            ResNode->ToggleVisibility(value);
            OnVisibilityToggled?.Invoke(value);
        }
    }

    private Action<bool>? OnVisibilityToggled { get; set; }

    public NodeFlags NodeFlags {
        get => ResNode->NodeFlags;
        set => ResNode->NodeFlags = value;
    }

    public virtual Vector4 Color {
        get => ResNode->Color.ToVector4();
        set => ResNode->Color = value.ToByteColor();
    }

    public virtual ColorHelpers.HsvaColor HsvaColor {
        get => ColorHelpers.RgbaToHsv(Color);
        set => Color = ColorHelpers.HsvToRgb(value);
    }

    public virtual float Alpha {
        get => ResNode->Color.A;
        set => ResNode->SetAlpha((byte)(value * 255.0f));
    }

    public virtual Vector3 AddColor {
        get => new Vector3(ResNode->AddRed, ResNode->AddGreen, ResNode->AddBlue) / 255.0f;
        set {
            ResNode->AddRed = (short)(value.X * 255);
            ResNode->AddGreen = (short)(value.Y * 255);
            ResNode->AddBlue = (short)(value.Z * 255);
        }
    }

    public virtual ColorHelpers.HsvaColor HsvaAddColor {
        get => ColorHelpers.RgbaToHsv(AddColor.AsVector4());
        set => AddColor = ColorHelpers.HsvToRgb(value).AsVector3();
    }

    public virtual Vector3 MultiplyColor {
        get => new Vector3(ResNode->MultiplyRed, ResNode->MultiplyGreen, ResNode->MultiplyBlue) / 100.0f;
        set {
            ResNode->MultiplyRed = (byte)(value.X * 100.0f);
            ResNode->MultiplyGreen = (byte)(value.Y * 100.0f);
            ResNode->MultiplyBlue = (byte)(value.Z * 100.0f);
        }
    }

    public virtual ColorHelpers.HsvaColor HsvaMultiplyColor {
        get => ColorHelpers.RgbaToHsv(MultiplyColor.AsVector4());
        set => MultiplyColor = ColorHelpers.HsvToRgb(value).AsVector3();
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

    public int Priority {
        get => ResNode->GetPriority();
        set => ResNode->SetPriority((ushort)value);
    }

    protected virtual NodeType NodeType {
        get => ResNode->GetNodeType();
        set => ResNode->Type = value;
    }

    public virtual int ChildCount => ResNode->ChildCount;

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
        => VisitChildren(ResNode, pointer => pointer.Value->DrawFlags |= 1);

    public bool CheckCollision(short x, short y, bool inclusive = true)
        => ResNode->CheckCollisionAtCoords(x, y, true);

    public bool CheckCollision(AtkEventData* eventData, bool inclusive = true)
        => CheckCollision(eventData->MouseData.PosX, eventData->MouseData.PosY, inclusive);

    public Matrix2x2 Transform {
        get => ResNode->Transform;
        set => ResNode->Transform = value;
    }
}
