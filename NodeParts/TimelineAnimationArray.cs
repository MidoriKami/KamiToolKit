using System;
using System.Collections.Generic;
using System.Linq;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit.NodeParts;

public unsafe class TimelineAnimationArray : IDisposable {

    internal AtkTimelineAnimation* InternalTimelineArray = null;

    private List<TimelineAnimation> timelineAnimations = [];
    public uint Count { get; private set; }

    public List<TimelineAnimation> Animations {
        get => timelineAnimations;
        set {
            timelineAnimations = value;
            Resync();
        }
    }

    public void Dispose() {
        foreach (var animation in timelineAnimations) {
            animation.Dispose();
        }

        NativeMemoryHelper.UiFree(InternalTimelineArray, Count);
        InternalTimelineArray = null;
    }

    private void Resync() {
        // Free existing array, we will completely rebuild it
        if (InternalTimelineArray is not null) {
            NativeMemoryHelper.UiFree(InternalTimelineArray, Count);
            InternalTimelineArray = null;
        }

        // Allocate new array
        InternalTimelineArray = NativeMemoryHelper.UiAlloc<AtkTimelineAnimation>(timelineAnimations.Count);

        // Copy all Animations into it
        foreach (var index in Enumerable.Range(0, timelineAnimations.Count)) {
            InternalTimelineArray[index] = *timelineAnimations[index].InternalAnimation;
        }

        Count = (uint)timelineAnimations.Count;
    }
}
