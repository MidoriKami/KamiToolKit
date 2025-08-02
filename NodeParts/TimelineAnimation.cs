using System;
using System.Collections.Generic;
using System.Linq;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit.NodeParts;

public unsafe class TimelineAnimation : IDisposable {

    internal AtkTimelineAnimation* InternalAnimation;

    private List<TimelineKeyFrame> internalKeyFrames = [];

    public TimelineAnimation() {
        InternalAnimation = NativeMemoryHelper.UiAlloc<AtkTimelineAnimation>();

        InternalAnimation->StartFrameIdx = 0;
        InternalAnimation->EndFrameIdx = 0;

        foreach (ref var value in InternalAnimation->KeyGroups) {
            value.Type = AtkTimelineKeyGroupType.None;
        }
    }

    public int StartFrameId {
        get => InternalAnimation->StartFrameIdx;
        set => InternalAnimation->StartFrameIdx = (ushort)value;
    }

    public int EndFrameId {
        get => InternalAnimation->EndFrameIdx;
        set => InternalAnimation->EndFrameIdx = (ushort)value;
    }

    public List<TimelineKeyFrame> KeyFrames {
        get => internalKeyFrames;
        set {
            internalKeyFrames = value;
            Resync();
        }
    }

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
}

public enum KeyFrameGroupType {
    Position = 0,
    Rotation = 1,
    Scale = 2,
    Alpha = 3,
    Tint = 4,

    PartId = 5,
    TextColor = 5,

    TextEdge = 6,
    TextLabel = 7,
}
