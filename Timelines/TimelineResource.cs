using System;
using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Internal.Classes;

namespace KamiToolKit.Timelines;

/// <summary>
/// Managed adaptor for native structs. Not intended for external use.
/// </summary>
public unsafe class TimelineResource : IDisposable {

    private readonly TimelineAnimationArray animationArray;
    private readonly TimelineLabelSetArray labelsArray;

    internal AtkTimelineResource* InternalResource;

    /// <summary>
    /// Constructs a new <see cref="TimelineResource"/>
    /// </summary>
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

    /// <summary>
    /// Gets or sets the animation keyframes.
    /// </summary>
    public List<TimelineAnimation> Animations {
        get => animationArray.Animations;
        set {
            animationArray.Animations = value;
            InternalResource->Animations = animationArray.InternalTimelineArray;
            InternalResource->AnimationCount = (ushort)animationArray.Count;
        }
    }

    /// <summary>
    /// Gets or sets the animation label sets.
    /// </summary>
    public List<TimelineLabelSet> LabelSets {
        get => labelsArray.LabelSets;
        set {
            labelsArray.LabelSets = value;
            InternalResource->LabelSets = labelsArray.InternalLabelSetArray;
            InternalResource->LabelSetCount = (ushort)labelsArray.Count;
        }
    }

    /// <summary>
    /// Gets or sets the internal resource id.
    /// </summary>
    public int Id {
        get => (int)InternalResource->Id;
        set => InternalResource->Id = (uint)value;
    }

    /// <inheritdoc />
    public void Dispose() {
        animationArray.Dispose();
        labelsArray.Dispose();

        NativeMemoryHelper.UiFree(InternalResource);
        InternalResource = null;
    }
}
