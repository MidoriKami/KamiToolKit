using System;
using System.Collections.Generic;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using FFXIVClientStructs.Interop;
using KamiToolKit.Classes.Internal;
using KamiToolKit.Enums;

namespace KamiToolKit.Timelines;

/// <summary>
/// Managed representation of a built timeline.
/// </summary>
public unsafe class Timeline : IDisposable {

    /// <summary>
    /// Sets the timeline animations used for this Timeline.
    /// </summary>
    public List<TimelineAnimation> Animations {
        set => internalTimelineResource.Animations = value;
    }

    /// <summary>
    /// Sets the label sets used for this Timeline.
    /// </summary>
    public List<TimelineLabelSet> LabelSets {
        set => internalTimelineResource.LabelSets = value;
    }

    /// <summary>
    /// Plays the specified animation via label ID
    /// </summary>
    /// <param name="labelId">The label ID to play</param>
    /// <param name="force">Force the animation to restart even if it was already playing</param>
    public void PlayAnimation(int labelId, bool force = false)
        => PlayAnimation(AtkTimelineJumpBehavior.Start, labelId, force);

    /// <summary>
    /// Plays the specified animation via label ID, with option to force it to restart if its already running.
    /// </summary>
    public void PlayAnimation(AtkTimelineJumpBehavior behavior, int labelId, bool force = false) {
        if (InternalTimeline is null) return;

        if (InternalTimeline->ActiveLabelId != labelId || force) {
            InternalTimeline->PlayAnimation(behavior, (ushort)labelId);
        }
    }

    /// <summary>
    /// Stops any active animation by invoking labelId 0.
    /// </summary>
    public void StopAnimation() {
        if (InternalTimeline is null) return;

        InternalTimeline->PlayAnimation(AtkTimelineJumpBehavior.Start, 0);
    }

    /// <summary>
    /// Helper for updating a specific keyframes values.
    /// </summary>
    public void UpdateKeyFrame(
        int frameId, KeyFrameGroupType groupType, Vector2? position = null, byte? alpha = null, Vector3? addColor = null, Vector3? multiplyColor = null,
        float? rotation = null, Vector2? scale = null, Vector3? textColor = null, Vector3? textOutlineColor = null, uint? partId = null, AtkTimelineInterpolation? interpolation = null) {

        var keyFrame = GetKeyFrame(groupType, frameId);
        if (keyFrame is null) return;

        if (position is not null) {
            *keyFrame = new TimelineAnimationKeyFrame {
                FrameIndex = frameId, Position = position.Value, Interpolation = interpolation ?? AtkTimelineInterpolation.Linear,
            };
        }

        if (alpha is not null) {
            *keyFrame = new TimelineAnimationKeyFrame {
                FrameIndex = frameId, Alpha = alpha.Value, Interpolation = interpolation ?? AtkTimelineInterpolation.Linear,
            };
        }

        if (addColor is not null || multiplyColor is not null) {
            *keyFrame = new TimelineAnimationKeyFrame {
                FrameIndex = frameId, AddColor = addColor ?? new Vector3(0.0f, 0.0f, 0.0f), MultiplyColor = multiplyColor ?? new Vector3(100.0f, 100.0f, 100.0f), Interpolation = interpolation ?? AtkTimelineInterpolation.Linear,
            };
        }

        if (rotation is not null) {
            *keyFrame = new TimelineAnimationKeyFrame {
                FrameIndex = frameId, Rotation = rotation.Value, Interpolation = interpolation ?? AtkTimelineInterpolation.Linear,
            };
        }

        if (scale is not null) {
            *keyFrame = new TimelineAnimationKeyFrame {
                FrameIndex = frameId, Scale = scale.Value, Interpolation = interpolation ?? AtkTimelineInterpolation.Linear,
            };
        }

        if (textColor is not null) {
            *keyFrame = new TimelineAnimationKeyFrame {
                FrameIndex = frameId, TextColor = textColor.Value, Interpolation = interpolation ?? AtkTimelineInterpolation.Linear,
            };
        }

        if (textOutlineColor is not null) {
            *keyFrame = new TimelineAnimationKeyFrame {
                FrameIndex = frameId, TextEdgeColor = textOutlineColor.Value, Interpolation = interpolation ?? AtkTimelineInterpolation.Linear,
            };
        }

        if (partId is not null) {
            *keyFrame = new TimelineAnimationKeyFrame {
                FrameIndex = frameId, PartId = partId.Value, Interpolation = interpolation ?? AtkTimelineInterpolation.Linear,
            };
        }
    }

    public Timeline() {
        InternalTimeline = NativeMemoryHelper.UiAlloc<AtkTimeline>();

        internalTimelineResource = new TimelineResource();
        InternalTimeline->Resource = internalTimelineResource.InternalResource;
        InternalTimeline->LabelResource = null;
        InternalTimeline->ActiveAnimation = null;
        InternalTimeline->OwnerNode = null;
    }

    public void Dispose() {
        internalTimelineResource.Dispose();

        NativeMemoryHelper.UiFree(InternalTimeline);
        InternalTimeline = null;
    }

    private AtkTimelineKeyFrame* GetKeyFrame(KeyFrameGroupType type, int frameIndex) {
        var animation = GetAnimationForFrameId(frameIndex);
        if (animation is null) return null;

        var keyGroup = animation->KeyGroups.GetPointer((int)type);
        for (var i = 0; i < keyGroup->KeyFrameCount; i++) {
            var keyFrame = &keyGroup->KeyFrames[i];

            if (keyFrame->FrameIdx == frameIndex) {
                return keyFrame;
            }
        }

        return null;
    }

    private AtkTimelineAnimation* GetAnimationForFrameId(int frameId) {
        if (InternalTimeline is null) return null;
        if (InternalTimeline->Resource is null) return null;

        for (var index = 0; index < InternalTimeline->Resource->AnimationCount; index++) {
            var animation = &InternalTimeline->Resource->Animations[index];

            if (animation->StartFrameIdx <= frameId && frameId <= animation->EndFrameIdx)
                return animation;
        }

        return null;
    }

    private readonly TimelineResource internalTimelineResource;
    internal AtkTimeline* InternalTimeline;

    internal AtkResNode* OwnerNode {
        get => InternalTimeline->OwnerNode;
        set => InternalTimeline->OwnerNode = value;
    }

    internal float FrameTime {
        get => InternalTimeline->FrameTime;
        set => InternalTimeline->FrameTime = value;
    }

    internal float ParentFrameTime {
        get => InternalTimeline->ParentFrameTime;
        set => InternalTimeline->ParentFrameTime = value;
    }

    internal int LabelFrameIdxDuration {
        get => InternalTimeline->LabelFrameIdxDuration;
        set => InternalTimeline->LabelFrameIdxDuration = (ushort)value;
    }

    internal int LabelEndFrameIdx {
        get => InternalTimeline->LabelEndFrameIdx;
        set => InternalTimeline->LabelEndFrameIdx = (ushort)value;
    }

    internal int ActiveLabelId {
        get => InternalTimeline->ActiveLabelId;
        set => InternalTimeline->ActiveLabelId = (ushort)value;
    }

    internal AtkTimelineMask Mask {
        get => InternalTimeline->Mask;
        set => InternalTimeline->Mask = value;
    }

    internal AtkTimelineFlags Flags {
        get => InternalTimeline->Flags;
        set => InternalTimeline->Flags = value;
    }
}
