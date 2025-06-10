using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit.NodeParts;

/// <summary>
/// Wrapper around a AtkUldPartsList, manages adding and removing multiple parts more easily.
/// </summary>
public unsafe class PartsList : IList<Part>, IDisposable {
    internal AtkUldPartsList* InternalPartsList;
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
            InternalPartsList = null;
        }

        isDisposed = true;
    }

    public uint Id {
        get => InternalPartsList->Id;
        set => InternalPartsList->Id = value;
    }

    private void Resync() {
		// Free existing array, we will completely rebuild it
        if (InternalPartsList->Parts is not null) {
            NativeMemoryHelper.UiFree(InternalPartsList->Parts, InternalPartsList->PartCount);
            InternalPartsList->Parts = null;
        }

		// Allocate new array
        InternalPartsList->Parts = NativeMemoryHelper.UiAlloc<AtkUldPart>(parts.Count);

		// Copy all Parts into it
        foreach (var index in Enumerable.Range(0, parts.Count)) {
            InternalPartsList->Parts[index] = *parts[index].InternalPart;
            
            // Update stored pointer to the data so any further modifications are actually applied
            parts[index].InternalPart = &InternalPartsList->Parts[index];
        }

        InternalPartsList->PartCount = (uint) parts.Count;
        Count = parts.Count;
    }

    public IEnumerator<Part> GetEnumerator()
        => parts.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    public void Add(params Part[] items) {
        foreach (var part in items) {
            Add(part);
        }
    }
    
    public void Add(Part item) {
        parts.Add(item);
        Resync();
    }

    public void Clear() {
        parts.Clear();

        NativeMemoryHelper.UiFree(InternalPartsList->Parts, InternalPartsList->PartCount);
        InternalPartsList->Parts = null;
        InternalPartsList->PartCount = 0;
    }

    public bool Contains(Part item)
        => parts.Contains(item);

    public void CopyTo(Part[] array, int arrayIndex)
        => parts.CopyTo(array, arrayIndex);

    public bool Remove(Part item) {
        if (!Contains(item)) return false;
        parts.Remove(item);
        Resync();

        return true;
    }

    public int Count { get; set; }

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
            Resync();
        }
    }
}