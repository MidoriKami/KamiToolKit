using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using FFXIVClientStructs.STD;
using KamiToolKit.Extensions;

namespace KamiToolKit.NodeParts;

public class TimelineAnimationKeyFrame : TimelineKeyFrame {

    private readonly NodeTint nodeTint = new();

    public Vector2 Position {
        get => new(Value.Float2.Item1, Value.Float2.Item2);
        set {
            Value = new AtkTimelineKeyValue {
                Float2 = new StdPair<float, float>(value.X, value.Y),
            };

            GroupSelector = KeyFrameGroupType.Position;
            GroupType = AtkTimelineKeyGroupType.Float2;
        }
    }

    public byte Alpha {
        get => Value.Byte;
        set {
            Value = new AtkTimelineKeyValue {
                Byte = value,
            };

            GroupType = AtkTimelineKeyGroupType.Byte;
            GroupSelector = KeyFrameGroupType.Alpha;
        }
    }

    public Vector3 AddColor {
        set {
            nodeTint.AddColor = value;
            UpdateNodeTint();
        }
    }

    public Vector3 MultiplyColor {
        set {
            nodeTint.MultiplyColor = value;
            UpdateNodeTint();
        }
    }

    public float Rotation {
        get => Value.Float;
        set {
            Value = new AtkTimelineKeyValue {
                Float = value,
            };

            GroupType = AtkTimelineKeyGroupType.Float;
            GroupSelector = KeyFrameGroupType.Rotation;
        }
    }

    public Vector2 Scale {
        get => new(Value.Float2.Item1, Value.Float2.Item2);
        set {
            Value = new AtkTimelineKeyValue {
                Float2 = new StdPair<float, float>(value.X, value.Y),
            };

            GroupType = AtkTimelineKeyGroupType.Float2;
            GroupSelector = KeyFrameGroupType.Scale;
        }
    }

    public Vector3 TextColor {
        get => new Vector3(Value.RGB.R, Value.RGB.G, Value.RGB.B) * 255.0f;
        set {
            Value = new AtkTimelineKeyValue {
                RGB = value.AsVector4().ToByteColor(),
            };

            GroupType = AtkTimelineKeyGroupType.RGB;
            GroupSelector = KeyFrameGroupType.TextColor;
        }
    }

    public Vector3 TextEdgeColor {
        get => new Vector3(Value.RGB.R, Value.RGB.G, Value.RGB.B) * 255.0f;
        set {
            Value = new AtkTimelineKeyValue {
                RGB = value.AsVector4().ToByteColor(),
            };

            GroupType = AtkTimelineKeyGroupType.RGB;
            GroupSelector = KeyFrameGroupType.TextEdge;
        }
    }

    public uint PartId {
        set {
            Value = new AtkTimelineKeyValue {
                UShort = (ushort)value,
            };

            GroupType = AtkTimelineKeyGroupType.UShort;
            GroupSelector = KeyFrameGroupType.PartId;
        }
    }

    private void UpdateNodeTint() {
        Value = new AtkTimelineKeyValue {
            NodeTint = nodeTint,
        };

        GroupType = AtkTimelineKeyGroupType.NodeTint;
        GroupSelector = KeyFrameGroupType.Tint;
    }
}
