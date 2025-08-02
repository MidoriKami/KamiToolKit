using System;
using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit.NodeParts;

public unsafe class TimelineResource : IDisposable {

    private readonly TimelineAnimationArray animationArray;
    private readonly TimelineLabelSetArray labelsArray;

    internal AtkTimelineResource* InternalResource;

    public TimelineResource() {
        InternalResource = NativeMemoryHelper.UiAlloc<AtkTimelineResource>();

        InternalResource->Id = 2;
        InternalResource->AnimationCount = 0;
        InternalResource->LabelSetCount = 0;

        animationArray = new TimelineAnimationArray();
        InternalResource->Animations = animationArray.InternalTimelineArray;

        labelsArray = new TimelineLabelSetArray();
        InternalResource->LabelSets = labelsArray.InternalLabelSetArray;
    }

    public List<TimelineAnimation> Animations {
        get => animationArray.Animations;
        set {
            animationArray.Animations = value;
            InternalResource->Animations = animationArray.InternalTimelineArray;
            InternalResource->AnimationCount = (ushort)animationArray.Count;
        }
    }

    public List<TimelineLabelSet> LabelSets {
        get => labelsArray.LabelSets;
        set {
            labelsArray.LabelSets = value;
            InternalResource->LabelSets = labelsArray.InternalLabelSetArray;
            InternalResource->LabelSetCount = (ushort)labelsArray.Count;
        }
    }

    public int Id {
        get => (int)InternalResource->Id;
        set => InternalResource->Id = (uint)value;
    }

    public void Dispose() {
        animationArray.Dispose();
        labelsArray.Dispose();

        NativeMemoryHelper.UiFree(InternalResource);
        InternalResource = null;
    }
}
