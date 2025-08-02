using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit.NodeParts;

/// <summary>
///     Wrapper around a AtkUldPartsList, manages adding and removing multiple parts more easily.
/// </summary>
public unsafe class PartsList : IList<Part>, IDisposable {

    private readonly List<Part> parts = [];

    internal AtkUldPartsList* InternalPartsList;

    private bool isDisposed;

    public PartsList() {
        InternalPartsList = NativeMemoryHelper.UiAlloc<AtkUldPartsList>();

        InternalPartsList->Parts = null;
        InternalPartsList->PartCount = 0;
        InternalPartsList->Id = 0;
    }

    public uint PartCount {
        get => InternalPartsList->PartCount;
        internal set => InternalPartsList->PartCount = value;
    }

    public uint Id {
        get => InternalPartsList->Id;
        set => InternalPartsList->Id = value;
    }

    public void Dispose() {
        if (!isDisposed) {
            foreach (var part in parts.ToList()) {
                Remove(part);
            }

            NativeMemoryHelper.UiFree(InternalPartsList);
            InternalPartsList = null;
        }

        isDisposed = true;
    }

    public IEnumerator<Part> GetEnumerator()
        => parts.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    public void Add(Part item) {
        NativeMemoryHelper.ResizeArray(ref InternalPartsList->Parts, PartCount, PartCount + 1);

        // Add new part on end of buffer
        InternalPartsList->Parts[PartCount] = *item.InternalPart;

        // Now that we have the data copied into the new array, update the pointer and free old memory
        var allocatedPart = item.InternalPart;
        item.InternalPart = &InternalPartsList->Parts[PartCount];
        NativeMemoryHelper.UiFree(allocatedPart);

        // Update Parts List
        PartCount++;
        parts.Add(item);
    }

    public void Clear() {
        NativeMemoryHelper.UiFree(InternalPartsList->Parts, PartCount);
        PartCount = 0;
    }

    public bool Contains(Part item)
        => parts.Contains(item);

    public void CopyTo(Part[] array, int arrayIndex)
        => parts.CopyTo(array, arrayIndex);

    public bool Remove(Part item) {
        if (!Contains(item)) return false;

        // Make new buffer to fit new part
        var newBuffer = NativeMemoryHelper.UiAlloc<AtkUldPart>(PartCount - 1);

        foreach (var index in Enumerable.Range(0, (int)PartCount)) {

            // If this is the item we want to remove, skip.
            if (&InternalPartsList->Parts[index] == item.InternalPart) continue;

            newBuffer[index] = InternalPartsList->Parts[index];
        }

        // Free the old buffer
        NativeMemoryHelper.UiFree(InternalPartsList->Parts, PartCount);

        // Remove the item from the stored collection
        PartCount--;
        parts.Remove(item);

        // Assign new parts list to native parts list
        InternalPartsList->Parts = newBuffer;

        return true;
    }

    public int Count => parts.Count;

    public bool IsReadOnly => false;

    public int IndexOf(Part item)
        => parts.IndexOf(item);

    public void Insert(int index, Part item)
        => throw new Exception("Inserting a part into arbitrary index is not supported.");

    public void RemoveAt(int index)
        => Remove(parts[index]);

    public Part this[int index] {
        get => parts[index];
        set {
            parts[index] = value;
            InternalPartsList->Parts[index] = *value.InternalPart;
        }
    }

    public void Add(params Part[] items) {
        foreach (var part in items) {
            Add(part);
        }
    }
}
