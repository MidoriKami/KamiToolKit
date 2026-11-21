using System;
using System.Linq;
using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiToolKit.Classes;

/// <summary>
/// Managed AtkUldPartsList. For storing and managing multiple AtkUldParts'
/// </summary>
public unsafe class PartsList : IDisposable {

    /// <summary>
    /// Gets or sets an individual part by index.
    /// </summary>
    /// <param name="index"></param>
    public AtkUldPart* this[int index] {
        get {
            if (InternalPartsList is null) return null;
            if (PartCount <= index) return null;

            return &InternalPartsList->Parts[index];
        }
    }

    /// <summary>
    /// Add multiple parts to this PartsList.
    /// </summary>
    /// <param name="items">The parts to add.</param>
    public void Add(params Part[] items) {
        EnsureCapacity(PartCount + (uint)items.Length);

        foreach (var part in items) {
            AddPart(part);
        }
    }

    /// <summary>
    /// Add a single part to this PartsList.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public AtkUldPart* Add(Part item) {
        EnsureCapacity(PartCount + 1);

        return AddPart(item);
    }

    /// <summary>
    /// Internally exposed pointer to the contained PartsList.
    /// </summary>
    internal AtkUldPartsList* InternalPartsList;

    /// <summary>
    /// Constructs a new <see cref="PartsList"/> instance.
    /// </summary>
    public PartsList() {
        InternalPartsList = IMemorySpace.GetUISpace()->MallocZeroed<AtkUldPartsList>();

        InternalPartsList->Parts = null;
        InternalPartsList->PartCount = 0;
        InternalPartsList->Id = 0;
    }

    /// <inheritdoc />
    public void Dispose() {
        if (!isDisposed) {
            foreach (var partIndex in Enumerable.Range(0, (int)PartCount)) {
                ref var part = ref InternalPartsList->Parts[partIndex];

                if (part.UldAsset is not null && part.UldAsset->AtkTexture.IsTextureReady()) {
                    part.UldAsset->AtkTexture.ReleaseTexture();
                    part.UldAsset->AtkTexture.KernelTexture = null;
                    part.UldAsset->AtkTexture.TextureType = 0;
                }

                IMemorySpace.Free(part.UldAsset);
                part.UldAsset = null;
            }

            if (InternalPartsList->Parts is not null) {
                IMemorySpace.Free(InternalPartsList->Parts);
                InternalPartsList->Parts = null;
            }

            IMemorySpace.Free(InternalPartsList);
            InternalPartsList = null;
            partCapacity = 0;
        }

        isDisposed = true;
    }

    private uint PartCount {
        get => InternalPartsList->PartCount;
        set => InternalPartsList->PartCount = value;
    }

    private void EnsureCapacity(uint capacity) {
        if (partCapacity >= capacity) return;

        var newCapacity = partCapacity is 0 ? 4U : partCapacity;

        while (newCapacity < capacity) {
            if (newCapacity > uint.MaxValue / 2) {
                newCapacity = capacity;
                break;
            }

            newCapacity *= 2;
        }

        var newBuffer = IMemorySpace.GetUISpace()->AllocateZeroedArray<AtkUldPart>(newCapacity);

        if (InternalPartsList->Parts is not null) {
            IMemorySpace.Copy(InternalPartsList->Parts, newBuffer, PartCount);
            IMemorySpace.Free(InternalPartsList->Parts);
        }

        InternalPartsList->Parts = newBuffer;
        partCapacity = newCapacity;
    }

    private AtkUldPart* AddPart(Part item) {

        ref var newPart = ref InternalPartsList->Parts[PartCount];

        newPart.Width = (ushort)item.Width;
        newPart.Height = (ushort)item.Height;
        newPart.U = (ushort)item.U;
        newPart.V = (ushort)item.V;

        newPart.UldAsset = IMemorySpace.GetUISpace()->MallocZeroed<AtkUldAsset>();
        newPart.UldAsset->Id = item.Id;
        newPart.UldAsset->AtkTexture.Ctor();
        newPart.LoadTexture(item.TexturePath);

        return &InternalPartsList->Parts[PartCount++];
    }

    private bool isDisposed;
    private uint partCapacity;
}
