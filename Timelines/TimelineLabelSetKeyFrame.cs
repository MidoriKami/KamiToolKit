using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Enums;

namespace KamiToolKit.Timelines;

/// <summary>
/// Managed adaptor for native structs. Not intended for external use.
/// </summary>
public class TimelineLabelSetKeyFrame : TimelineKeyFrame {

    private AtkTimelineLabel data;

    public AtkTimelineJumpBehavior JumpBehavior {
        get => data.JumpBehavior;
        set {
            data.JumpBehavior = value;
            UpdateValue();
        }
    }

    public int LabelId {
        get => data.LabelId;
        set {
            data.LabelId = (ushort)value;
            UpdateValue();
        }
    }

    public int JumpLabelId {
        get => data.JumpLabelId;
        set {
            data.JumpLabelId = (byte)value;
            UpdateValue();
        }
    }

    private void UpdateValue() {
        Value = new AtkTimelineKeyValue {
            Label = data,
        };

        GroupType = AtkTimelineKeyGroupType.Label;
        SpeedEnd = 0.0f;
        Interpolation = AtkTimelineInterpolation.None;
        GroupSelector = KeyFrameGroupType.TextLabel;
    }
}
