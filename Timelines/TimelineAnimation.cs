using System;
using System.Collections.Generic;
using System.Linq;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Internal.Classes;

namespace KamiToolKit.Timelines;

/// <summary>
/// Managed class representing a AtkTimelineAnimation
/// </summary>
public unsafe class TimelineAnimation : IDisposable {

    /// <summary>
    /// The starting frame index for this animation.
    /// </summary>
    /// <remarks>
    /// Must be less than <see cref="EndFrameId"/>
    /// </remarks>
    public int StartFrameId {
        get => InternalAnimation->StartFrameIdx;
        set => InternalAnimation->StartFrameIdx = (ushort)value;
    }

    /// <summary>
    /// The ending frame index for this animation.
    /// </summary>
    /// <remarks>
    /// Must be greater than <see cref="StartFrameId"/>
    /// </remarks>
    public int EndFrameId {
        get => InternalAnimation->EndFrameIdx;
        set => InternalAnimation->EndFrameIdx = (ushort)value;
    }

    /// <summary>
    /// Gets or sets the keyframes used.
    /// </summary>
    /// <remarks>
    /// Use <see cref="TimelineBuilder"/> to more easily edit keyframes.
    /// </remarks>
    public List<TimelineKeyFrame> KeyFrames {
        get => internalKeyFrames;
        set {
            internalKeyFrames = value;
            Resync();
        }
    }

    /// <summary>
    /// Constructs a new instance of <see cref="TimelineAnimation"/>.
    /// </summary>
    public TimelineAnimation() {
        InternalAnimation = NativeMemoryHelper.UiAlloc<AtkTimelineAnimation>();

        InternalAnimation->StartFrameIdx = 0;
        InternalAnimation->EndFrameIdx = 0;

        foreach (ref var value in InternalAnimation->KeyGroups) {
            value.Type = AtkTimelineKeyGroupType.None;
        }
    }

    /// <inheritdoc />
    public void Dispose() {
        if (InternalAnimation is null) return;

        foreach (ref var spanGroup in InternalAnimation->KeyGroups) {
            NativeMemoryHelper.UiFree(spanGroup.KeyFrames);
            spanGroup.KeyFrames = null;
            spanGroup.KeyFrameCount = 0;
        }

        NativeMemoryHelper.UiFree(InternalAnimation);
        InternalAnimation = null;
    }

    private void Resync() {
        foreach (var keyFrameSet in internalKeyFrames.GroupBy(frame => frame.GroupSelector)) {
            ref var keyFrameGroup = ref InternalAnimation->KeyGroups[(int)keyFrameSet.Key];
            keyFrameGroup.Type = keyFrameSet.First().GroupType;

            if (keyFrameGroup.KeyFrames is not null) {
                NativeMemoryHelper.UiFree(keyFrameGroup.KeyFrames, keyFrameGroup.KeyFrameCount);
                keyFrameGroup.KeyFrames = null;
            }

            keyFrameGroup.KeyFrames = NativeMemoryHelper.UiAlloc<AtkTimelineKeyFrame>(keyFrameSet.Count());

            var index = 0;
            foreach (var keyframe in keyFrameSet) {
                keyFrameGroup.KeyFrames[index] = keyframe;
                index++;
            }

            keyFrameGroup.KeyFrameCount = (ushort)keyFrameSet.Count();
        }
    }

    internal AtkTimelineAnimation* InternalAnimation;

    private List<TimelineKeyFrame> internalKeyFrames = [];
}
