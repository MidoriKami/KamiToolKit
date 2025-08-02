using System;
using System.Collections.Generic;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using FFXIVClientStructs.Interop;
using KamiToolKit.Classes;

namespace KamiToolKit.NodeParts;

public unsafe class Timeline : IDisposable {

    private readonly TimelineResource internalTimelineResource;

    internal AtkTimeline* InternalTimeline;

    public Timeline() {
        InternalTimeline = NativeMemoryHelper.UiAlloc<AtkTimeline>();

        internalTimelineResource = new TimelineResource();
        InternalTimeline->Resource = internalTimelineResource.InternalResource;
        InternalTimeline->LabelResource = null;
        InternalTimeline->ActiveAnimation = null;
        InternalTimeline->OwnerNode = null;
    }

    internal AtkResNode* OwnerNode {
        get => InternalTimeline->OwnerNode;
        set => InternalTimeline->OwnerNode = value;
    }

    public float FrameTime {
        get => InternalTimeline->FrameTime;
        set => InternalTimeline->FrameTime = value;
    }

    public float ParentFrameTime {
        get => InternalTimeline->ParentFrameTime;
        set => InternalTimeline->ParentFrameTime = value;
    }

    public int LabelFrameIdxDuration {
        get => InternalTimeline->LabelFrameIdxDuration;
        set => InternalTimeline->LabelFrameIdxDuration = (ushort)value;
    }

    public int LabelEndFrameIdx {
        get => InternalTimeline->LabelEndFrameIdx;
        set => InternalTimeline->LabelEndFrameIdx = (ushort)value;
    }

    public int ActiveLabelId {
        get => InternalTimeline->ActiveLabelId;
        set => InternalTimeline->ActiveLabelId = (ushort)value;
    }

    public AtkTimelineMask Mask {
        get => InternalTimeline->Mask;
        set => InternalTimeline->Mask = value;
    }

    public AtkTimelineFlags Flags {
        get => InternalTimeline->Flags;
        set => InternalTimeline->Flags = value;
    }

    public List<TimelineAnimation> Animations {
        set => internalTimelineResource.Animations = value;
    }

    public List<TimelineLabelSet> LabelSets {
        set => internalTimelineResource.LabelSets = value;
    }

    public void Dispose() {
        internalTimelineResource.Dispose();

        NativeMemoryHelper.UiFree(InternalTimeline);
        InternalTimeline = null;
    }

    /// <summary>
    ///     Plays the specified animation via label ID
    /// </summary>
    /// <param name="labelId">The label ID to play</param>
    /// <param name="force">Force the animation to restart even if it was already playing</param>
    public void PlayAnimation(int labelId, bool force = false) {
        if (InternalTimeline->ActiveLabelId != labelId || force) {
            InternalTimeline->PlayAnimation(AtkTimelineJumpBehavior.Start, (byte)labelId);
        }
    }

    public void StopAnimation()
        => InternalTimeline->PlayAnimation(AtkTimelineJumpBehavior.Start, 0);

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
}
