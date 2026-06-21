using System;
using System.Collections.Generic;
using System.Linq;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Internal.Classes;

namespace KamiToolKit.Timelines;

/// <summary>
/// Managed adaptor for native data. Not intended for external use.
/// </summary>
public unsafe class TimelineLabelSetArray : IDisposable {

    /// <summary>
    /// Gets the number of label sets that exist.
    /// </summary>
    public uint Count { get; private set; }

    /// <summary>
    /// Gets or sets the label sets.
    /// </summary>
    public List<TimelineLabelSet> LabelSets {
        get => labelSets;
        set {
            labelSets = value;
            Resync();
        }
    }

    /// <inheritdoc />
    public void Dispose() {
        foreach (var labelSet in labelSets) {
            labelSet.Dispose();
        }

        NativeMemoryHelper.UiFree(InternalLabelSetArray, Count);
        InternalLabelSetArray = null;
    }

    private void Resync() {
        // Free existing array, we will completely rebuild it
        if (InternalLabelSetArray is not null) {
            NativeMemoryHelper.UiFree(InternalLabelSetArray, Count);
            InternalLabelSetArray = null;
        }

        // Allocate new array
        InternalLabelSetArray = NativeMemoryHelper.UiAlloc<AtkTimelineLabelSet>(labelSets.Count);

        // Copy all Animations into it
        foreach (var index in Enumerable.Range(0, labelSets.Count)) {
            InternalLabelSetArray[index] = *labelSets[index].InternalLabelSet;
        }

        Count = (uint)labelSets.Count;
    }

    internal AtkTimelineLabelSet* InternalLabelSetArray = null;
    private List<TimelineLabelSet> labelSets = [];
}
