using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit.Nodes.Parts;

public unsafe class PartsList : IList<Part>, IDisposable {
    internal readonly AtkUldPartsList* InternalPartsList;
    private readonly List<Part> parts = [];

    public PartsList() {
        InternalPartsList = NativeMemoryHelper.UiAlloc<AtkUldPartsList>();

        InternalPartsList->Parts = null;
        InternalPartsList->PartCount = 0;
        InternalPartsList->Id = 0;
    }

    public void Dispose() {
        foreach (var part in parts) {
            part.Dispose();
        }
        
        NativeMemoryHelper.UiFree(InternalPartsList);
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

    public void CopyTo(Part[] array, int arrayIndex) {
        throw new Exception("Bulk Modification of PartsList is not supported.");
    }

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

    public void Insert(int index, Part item) {
        throw new Exception("Inserting a part into arbitrary index is not supported.");
    }

    public void RemoveAt(int index) {
        var partAtIndex = parts[index];
        Remove(partAtIndex);
    }

    public Part this[int index] {
        get => parts[index];
        set => throw new Exception("Setting a part at a arbitrary index is not supported.");
    }
}