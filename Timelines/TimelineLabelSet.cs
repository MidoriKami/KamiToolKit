using System;
using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Internal.Classes;

namespace KamiToolKit.Timelines;

/// <summary>
/// Managed adaptor to the native structs. Not intended for external use.
/// </summary>
public unsafe class TimelineLabelSet : IDisposable {

    private List<TimelineKeyFrame> internalKeyFrames = [];

    internal AtkTimelineLabelSet* InternalLabelSet;

    /// <summary>
    /// Constructs a new <see cref="TimelineLabelSet"/>
    /// </summary>
    public TimelineLabelSet() {
        InternalLabelSet = NativeMemoryHelper.UiAlloc<AtkTimelineLabelSet>();

        InternalLabelSet->StartFrameIdx = 0;
        InternalLabelSet->EndFrameIdx = 0;
        InternalLabelSet->LabelKeyGroup.Type = AtkTimelineKeyGroupType.Label;
    }

    /// <summary>
    /// Gets or sets start frame id.
    /// </summary>
    public int StartFrameId {
        get => InternalLabelSet->StartFrameIdx;
        set => InternalLabelSet->StartFrameIdx = (ushort)value;
    }

    /// <summary>
    /// Gets or sets end frame id.
    /// </summary>
    public int EndFrameId {
        get => InternalLabelSet->EndFrameIdx;
        set => InternalLabelSet->EndFrameIdx = (ushort)value;
    }

    /// <summary>
    /// Gets or sets the keyframe label sets.
    /// </summary>
    public List<TimelineKeyFrame> Labels {
        get => internalKeyFrames;
        set {
            internalKeyFrames = value;
            Resync();
        }
    }

    /// <inheritdoc />
    public void Dispose() {
        NativeMemoryHelper.UiFree(InternalLabelSet);
        InternalLabelSet = null;
    }

    private void Resync() {
        ref var keyGroup = ref InternalLabelSet->LabelKeyGroup;

        // Free existing array, we will completely rebuild it
        if (keyGroup.KeyFrames is null) {
            NativeMemoryHelper.UiFree(keyGroup.KeyFrames, keyGroup.KeyFrameCount);
            keyGroup.KeyFrames = null;
        }

        // Allocate new array
        keyGroup.KeyFrames = NativeMemoryHelper.UiAlloc<AtkTimelineKeyFrame>(internalKeyFrames.Count);

        var index = 0;
        foreach (var keyFrame in internalKeyFrames) {
            keyGroup.KeyFrames[index] = keyFrame;
            index++;
        }

        keyGroup.KeyFrameCount = (ushort)internalKeyFrames.Count;
    }
}
