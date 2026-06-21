using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using FFXIVClientStructs.STD;
using KamiToolKit.Enums;

namespace KamiToolKit.Timelines;

/// <summary>
/// Adaptor class for easily setting keyframe internal values. Not intended for external use.
/// </summary>
public class TimelineAnimationKeyFrame : TimelineKeyFrame {

    private readonly NodeTint nodeTint = new();

    /// <summary>
    /// X/Y position that the node will be set to.
    /// </summary>
    /// <remarks>
    /// This is not an offset from original position.
    /// </remarks>
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

    /// <summary>
    /// Alpha transparency, expects 0 to 255
    /// </summary>
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

    /// <summary>
    /// Add Color
    /// </summary>
    public Vector3 AddColor {
        set {
            nodeTint.AddColor = value;
            UpdateNodeTint();
        }
    }

    /// <summary>
    /// Multiply Color
    /// </summary>
    public Vector3 MultiplyColor {
        set {
            nodeTint.MultiplyColor = value;
            UpdateNodeTint();
        }
    }

    /// <summary>
    /// Rotation in radians.
    /// </summary>
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

    /// <summary>
    /// Scale.
    /// </summary>
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

    /// <summary>
    /// Text Color.
    /// </summary>
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

    /// <summary>
    /// Text outline color.
    /// </summary>
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

    /// <summary>
    /// Part id to use.
    /// </summary>
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
