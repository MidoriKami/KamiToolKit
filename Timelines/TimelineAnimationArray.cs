using System;
using System.Collections.Generic;
using System.Linq;
using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiToolKit.Timelines;

/// <summary>
/// Wrapper around an AtkTimelineAnimation array. Not intended for external use.
/// </summary>
public unsafe class TimelineAnimationArray : IDisposable {

    /// <summary>
    /// Gets the number of timeline animations.
    /// </summary>
    public uint Count { get; private set; }

    /// <summary>
    /// Gets or sets the timeline animations used.
    /// </summary>
    public List<TimelineAnimation> Animations {
        get => timelineAnimations;
        set {
            timelineAnimations = value;
            Resync();
        }
    }

    /// <inheritdoc />
    public void Dispose() {
        foreach (var animation in timelineAnimations) {
            animation.Dispose();
        }

        IMemorySpace.Free(InternalTimelineArray);
        InternalTimelineArray = null;
    }

    private void Resync() {
        // Free existing array, we will completely rebuild it
        if (InternalTimelineArray is not null) {
            IMemorySpace.Free(InternalTimelineArray);
            InternalTimelineArray = null;
        }

        // Allocate new array
        InternalTimelineArray = IMemorySpace.GetUISpace()->AllocateZeroedArray<AtkTimelineAnimation>(timelineAnimations.Count);

        // Copy all Animations into it
        foreach (var index in Enumerable.Range(0, timelineAnimations.Count)) {
            InternalTimelineArray[index] = *timelineAnimations[index].InternalAnimation;
        }

        Count = (uint)timelineAnimations.Count;
    }

    internal AtkTimelineAnimation* InternalTimelineArray = null;

    private List<TimelineAnimation> timelineAnimations = [];
}
