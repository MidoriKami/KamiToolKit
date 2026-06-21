using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Enums;

namespace KamiToolKit.Timelines;

/// <summary>
/// Represents the native data of a single keyframe and an adaptor to the native struct. Not intended for external use.
/// </summary>
public abstract class TimelineKeyFrame {

    /// <summary>
    /// Sets which sub index this keyframe belongs to.
    /// </summary>
    public KeyFrameGroupType GroupSelector { get; set; }

    /// <summary>
    /// Sets which main index this keyframe belongs to.
    /// </summary>
    public AtkTimelineKeyGroupType GroupType { get; set; }

    /// <summary>
    /// Gets or sets the speed start.
    /// </summary>
    /// <remarks>
    /// Unknown what this is actually doing.
    /// </remarks>
    public float SpeedStart { get; set; } = 0.0f;

    /// <summary>
    /// Gets or sets the speed end.
    /// </summary>
    /// <remarks>
    /// Unknown what this is actually doing.
    /// </remarks>
    public float SpeedEnd { get; set; } = 1.0f;

    /// <summary>
    /// Gets or sets the frame index that this keyframe represents.
    /// </summary>
    public required int FrameIndex { get; set; }

    /// <summary>
    /// Value blending mode.
    /// </summary>
    public AtkTimelineInterpolation Interpolation { get; set; } = AtkTimelineInterpolation.Linear;

    /// <summary>
    /// The actual data this keyframe is wrapping.
    /// </summary>
    public AtkTimelineKeyValue Value { get; set; }

    /// <summary>
    /// Conversion operator for native interop.
    /// </summary>
    public static implicit operator AtkTimelineKeyFrame(TimelineKeyFrame frame) => new() {
        Interpolation = frame.Interpolation,
        SpeedCoefficient1 = frame.SpeedStart,
        SpeedCoefficient2 = frame.SpeedEnd,
        FrameIdx = (ushort)frame.FrameIndex,
        Value = frame.Value,
    };
}
