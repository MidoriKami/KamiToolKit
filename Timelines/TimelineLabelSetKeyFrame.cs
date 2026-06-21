using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Enums;

namespace KamiToolKit.Timelines;

/// <summary>
/// Managed adaptor for native structs. Not intended for external use.
/// </summary>
public class TimelineLabelSetKeyFrame : TimelineKeyFrame {

    private AtkTimelineLabel data;

    /// <summary>
    /// Gets or sets the timeline jump behavior.
    /// </summary>
    public AtkTimelineJumpBehavior JumpBehavior {
        get => data.JumpBehavior;
        set {
            data.JumpBehavior = value;
            UpdateValue();
        }
    }

    /// <summary>
    /// Gets or sets the timelines label id.
    /// </summary>
    public int LabelId {
        get => data.LabelId;
        set {
            data.LabelId = (ushort)value;
            UpdateValue();
        }
    }

    /// <summary>
    /// Gets or sets the id that will be jumped to on completion.
    /// </summary>
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
