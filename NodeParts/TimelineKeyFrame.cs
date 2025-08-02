using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiToolKit.NodeParts;

public abstract class TimelineKeyFrame {

    public KeyFrameGroupType GroupSelector { get; set; }
    public AtkTimelineKeyGroupType GroupType { get; set; }

    public float SpeedStart { get; set; } = 0.0f;
    public float SpeedEnd { get; set; } = 1.0f;
    public required int FrameIndex { get; set; }
    public AtkTimelineInterpolation Interpolation { get; set; } = AtkTimelineInterpolation.Linear;
    public AtkTimelineKeyValue Value { get; set; }

    public static implicit operator AtkTimelineKeyFrame(TimelineKeyFrame frame) => new() {
        Interpolation = frame.Interpolation,
        SpeedCoefficient1 = frame.SpeedStart,
        SpeedCoefficient2 = frame.SpeedEnd,
        FrameIdx = (ushort)frame.FrameIndex,
        Value = frame.Value,
    };
}
