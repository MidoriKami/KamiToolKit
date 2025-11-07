using System;
using System.Numerics;
using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Common.Math;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using Newtonsoft.Json;
using Bounds = KamiToolKit.Classes.Bounds;
using Vector2 = System.Numerics.Vector2;
using Vector3 = System.Numerics.Vector3;
using Vector4 = System.Numerics.Vector4;

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

    public Bounds Bounds => new() { TopLeft = Position, BottomRight = Position + Size };

    public Vector2 Center => Position + Size / 2f;

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
        get => InternalResNode->GetRotation();
        set => InternalResNode->SetRotation(value);
    }

    public virtual float RotationDegrees {
        get => InternalResNode->GetRotationDegrees();
        set => InternalResNode->SetRotationDegrees(value - (int)(value / 360.0f) * 360.0f);
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
        set => InternalResNode->SetOrigin(value.X, value.Y);
    }

    [JsonProperty] public virtual bool IsVisible {
        get => InternalResNode->IsVisible();
        set {
            InternalResNode->ToggleVisibility(value);
            OnVisibilityToggled?.Invoke(value);
        }
    }

    private Action<bool>? OnVisibilityToggled { get; set; }

    public NodeFlags NodeFlags {
        get => InternalResNode->NodeFlags;
        set => InternalResNode->NodeFlags = value;
    }

    [JsonProperty] public virtual Vector4 Color {
        get => InternalResNode->Color.ToVector4();
        set => InternalResNode->Color = value.ToByteColor();
    }

    public virtual ColorHelpers.HsvaColor HsvaColor {
        get => ColorHelpers.RgbaToHsv(Color);
        set => Color = ColorHelpers.HsvToRgb(value);
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

    public virtual ColorHelpers.HsvaColor HsvaAddColor {
        get => ColorHelpers.RgbaToHsv(AddColor.AsVector4());
        set => AddColor = ColorHelpers.HsvToRgb(value).AsVector3();
    }

    [JsonProperty] public virtual Vector3 MultiplyColor {
        get => new Vector3(InternalResNode->MultiplyRed, InternalResNode->MultiplyGreen, InternalResNode->MultiplyBlue) / 100.0f;
        set {
            InternalResNode->MultiplyRed = (byte)(value.X * 100.0f);
            InternalResNode->MultiplyGreen = (byte)(value.Y * 100.0f);
            InternalResNode->MultiplyBlue = (byte)(value.Z * 100.0f);
        }
    }

    public virtual ColorHelpers.HsvaColor HsvaMultiplyColor {
        get => ColorHelpers.RgbaToHsv(MultiplyColor.AsVector4());
        set => MultiplyColor = ColorHelpers.HsvToRgb(value).AsVector3();
    }

    public uint NodeId {
        get => InternalResNode->NodeId;
        set => InternalResNode->NodeId = value;
    }

    public virtual DrawFlags DrawFlags {
        get => (DrawFlags) InternalResNode->DrawFlags;
        set => InternalResNode->DrawFlags = (uint) value & 0b1111_1111_1111_1100_0000_0011_1111_1111 | 
                                            InternalResNode->DrawFlags & 0b0000_0000_0000_0011_1111_1100_0000_0000;
    }

    public virtual int ClipCount {
        get => (int)((InternalResNode->DrawFlags & 0b0000_0000_0000_0011_1111_1100_0000_0000) >> 10);
        set => InternalResNode->DrawFlags = (uint)(value << 10 & 0b0000_0000_0000_0011_1111_1100_0000_0000) 
                                            | InternalResNode->DrawFlags & 0b1111_1111_1111_1100_0000_0011_1111_1111;
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

    public bool CheckCollision(short x, short y, bool inclusive = true)
        => InternalResNode->CheckCollisionAtCoords(x, y, true);

    public bool CheckCollision(AtkEventData* eventData, bool inclusive = true)
        => CheckCollision(eventData->MouseData.PosX, eventData->MouseData.PosY, inclusive);

    public Matrix2x2 Transform {
        get => InternalResNode->Transform;
        set => InternalResNode->Transform = value;
    }
}
