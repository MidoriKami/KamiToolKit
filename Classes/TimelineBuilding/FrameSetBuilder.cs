using System.Collections.Generic;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.NodeParts;

namespace KamiToolKit.Classes.TimelineBuilding;

public class FrameSetBuilder(TimelineBuilder parent, int startFrameId, int endFrameId) {

    private readonly List<TimelineKeyFrame> animationKeyFrames = [];
    private readonly List<TimelineKeyFrame> labelKeyFrames = [];

    public FrameSetBuilder AddFrame(params TimelineKeyFrame[] keyFrame) {
        foreach (var frame in keyFrame) {

            switch (frame.GroupType) {
                case AtkTimelineKeyGroupType.Label:
                    labelKeyFrames.Add(frame);
                    break;

                case AtkTimelineKeyGroupType.Float2:
                case AtkTimelineKeyGroupType.Float:
                case AtkTimelineKeyGroupType.Byte:
                case AtkTimelineKeyGroupType.NodeTint:
                case AtkTimelineKeyGroupType.UShort:
                case AtkTimelineKeyGroupType.RGB:
                case AtkTimelineKeyGroupType.Short:
                case AtkTimelineKeyGroupType.None:
                default:
                    animationKeyFrames.Add(frame);
                    break;
            }
        }

        return this;
    }

    public FrameSetBuilder AddEmptyFrame(int frameId) {
        animationKeyFrames.Add(new TimelineAnimationKeyFrame {
            FrameIndex = frameId, GroupType = AtkTimelineKeyGroupType.None,
        });

        return this;
    }

    public FrameSetBuilder AddFrame(
        int frameId, Vector2? position = null, byte? alpha = null, Vector3? addColor = null, Vector3? multiplyColor = null,
        float? rotation = null, Vector2? scale = null, Vector3? textColor = null, Vector3? textOutlineColor = null, uint? partId = null, AtkTimelineInterpolation? interpolation = null) {
        if (position is not null) {
            animationKeyFrames.Add(new TimelineAnimationKeyFrame {
                FrameIndex = frameId, Position = position.Value, Interpolation = interpolation ?? AtkTimelineInterpolation.Linear,
            });
        }

        if (alpha is not null) {
            animationKeyFrames.Add(new TimelineAnimationKeyFrame {
                FrameIndex = frameId, Alpha = alpha.Value, Interpolation = interpolation ?? AtkTimelineInterpolation.Linear,
            });
        }

        if (addColor is not null || multiplyColor is not null) {
            animationKeyFrames.Add(new TimelineAnimationKeyFrame {
                FrameIndex = frameId, AddColor = addColor ?? new Vector3(0.0f, 0.0f, 0.0f), MultiplyColor = multiplyColor ?? new Vector3(100.0f, 100.0f, 100.0f), Interpolation = interpolation ?? AtkTimelineInterpolation.Linear,
            });
        }

        if (rotation is not null) {
            animationKeyFrames.Add(new TimelineAnimationKeyFrame {
                FrameIndex = frameId, Rotation = rotation.Value, Interpolation = interpolation ?? AtkTimelineInterpolation.Linear,
            });
        }

        if (scale is not null) {
            animationKeyFrames.Add(new TimelineAnimationKeyFrame {
                FrameIndex = frameId, Scale = scale.Value, Interpolation = interpolation ?? AtkTimelineInterpolation.Linear,
            });
        }

        if (textColor is not null) {
            animationKeyFrames.Add(new TimelineAnimationKeyFrame {
                FrameIndex = frameId, TextColor = textColor.Value, Interpolation = interpolation ?? AtkTimelineInterpolation.Linear,
            });
        }

        if (textOutlineColor is not null) {
            animationKeyFrames.Add(new TimelineAnimationKeyFrame {
                FrameIndex = frameId, TextEdgeColor = textOutlineColor.Value, Interpolation = interpolation ?? AtkTimelineInterpolation.Linear,
            });
        }

        if (partId is not null) {
            animationKeyFrames.Add(new TimelineAnimationKeyFrame {
                FrameIndex = frameId, PartId = partId.Value, Interpolation = interpolation ?? AtkTimelineInterpolation.Linear,
            });
        }

        return this;
    }

    public FrameSetBuilder AddLabel(int frameId, int labelId, AtkTimelineJumpBehavior jumpBehavior, int labelTarget) {
        labelKeyFrames.Add(new TimelineLabelSetKeyFrame {
            FrameIndex = frameId,
            GroupType = AtkTimelineKeyGroupType.Label,
            JumpBehavior = jumpBehavior,
            LabelId = labelId,
            JumpLabelId = labelTarget,
        });

        return this;
    }

    public FrameSetBuilder AddLabelPair(int frameStart, int frameStop, int labelId) {
        labelKeyFrames.Add(new TimelineLabelSetKeyFrame {
            FrameIndex = frameStart, GroupType = AtkTimelineKeyGroupType.Label, JumpBehavior = AtkTimelineJumpBehavior.Start, LabelId = labelId,
        });

        labelKeyFrames.Add(new TimelineLabelSetKeyFrame {
            FrameIndex = frameStop,
            GroupType = AtkTimelineKeyGroupType.Label,
            JumpBehavior = AtkTimelineJumpBehavior.PlayOnce,
            LabelId = 0,
            JumpLabelId = 0,
        });

        return this;
    }

    public KeyFrameBuilder BeginFrameBuilder(int frame)
        => new(this, frame);

    public TimelineBuilder EndFrameSet() {
        if (labelKeyFrames.Count != 0) {
            parent.LabelSets.Add(new TimelineLabelSet {
                StartFrameId = startFrameId, EndFrameId = endFrameId, Labels = labelKeyFrames,
            });
        }

        if (animationKeyFrames.Count != 0) {
            parent.Animations.Add(new TimelineAnimation {
                StartFrameId = startFrameId, EndFrameId = endFrameId, KeyFrames = animationKeyFrames,
            });
        }

        return parent;
    }
}
