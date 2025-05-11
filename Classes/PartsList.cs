using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiToolKit.Classes;

/// <summary>
/// Wrapper around a AtkUldPartsList, manages adding and removing multiple parts more easily.
/// </summary>
public unsafe class PartsList : IList<Part>, IDisposable {
    internal readonly AtkUldPartsList* InternalPartsList;
    private readonly List<Part> parts = [];

    private bool isDisposed;

    public PartsList() {
        InternalPartsList = NativeMemoryHelper.UiAlloc<AtkUldPartsList>();

        InternalPartsList->Parts = null;
        InternalPartsList->PartCount = 0;
        InternalPartsList->Id = 0;
    }

    public void Dispose() {
        if (!isDisposed) {
            foreach (var part in parts) {
                part.Dispose();
            }
        
            NativeMemoryHelper.UiFree(InternalPartsList);
        }

        isDisposed = true;
    }

    public uint PartCount {
        get => InternalPartsList->PartCount;
        internal set => InternalPartsList->PartCount = value;
    }

    public uint Id {
        get => InternalPartsList->Id;
        set => InternalPartsList->Id = value;
    }

    public IEnumerator<Part> GetEnumerator()
        => parts.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    public void Add(Part item) {
        // Make new buffer to fit new part
        var newBuffer = NativeMemoryHelper.UiAlloc<AtkUldPart>(PartCount + 1);
        
        // Copy old parts into new buffer
        NativeMemory.Copy(InternalPartsList->Parts, newBuffer, (nuint) (sizeof(AtkUldPart) * PartCount));
        
        // Free old buffer
        if (InternalPartsList->Parts is not null) {
            NativeMemoryHelper.UiFree(InternalPartsList->Parts, PartCount);
        }
        
        // Add new part on end of buffer
        newBuffer[PartCount] = *item.InternalPart;
        
        // Now that we have the data copied into the new array, update the pointer and free old memory
        var allocatedPart = item.InternalPart;
        item.InternalPart = &newBuffer[PartCount];
        item.IsAttached = true;
        NativeMemoryHelper.UiFree(allocatedPart);
        
        // Update Parts List
        PartCount++;
        parts.Add(item);

        // Assign new parts list to native parts list
        InternalPartsList->Parts = newBuffer;
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

        foreach (var index in Enumerable.Range(0, (int) PartCount)) {
            
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
}