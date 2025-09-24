using System;
using System.Linq;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit.NodeParts;

/// <summary>
///     Wrapper around a AtkUldPartsList, manages adding multiple parts more easily.
/// </summary>
public unsafe class PartsList : IDisposable {

    internal AtkUldPartsList* InternalPartsList;

    private bool isDisposed;

    public PartsList() {
        InternalPartsList = NativeMemoryHelper.UiAlloc<AtkUldPartsList>();

        InternalPartsList->Parts = null;
        InternalPartsList->PartCount = 0;
        InternalPartsList->Id = 0;
    }

    public void Dispose() {
        if (!isDisposed) {
            foreach (var partIndex in Enumerable.Range(0, (int)PartCount)) {
                ref var part = ref InternalPartsList->Parts[partIndex];

                if (part.UldAsset is not null && part.UldAsset->AtkTexture.IsTextureReady()) {
                    part.UldAsset->AtkTexture.ReleaseTexture();
                    part.UldAsset->AtkTexture.KernelTexture = null;
                    part.UldAsset->AtkTexture.TextureType = 0;
                    part.UldAsset->AtkTexture.Destroy(true);
                }
                
                NativeMemoryHelper.UiFree(part.UldAsset);
                part.UldAsset = null;
            }

            NativeMemoryHelper.UiFree(InternalPartsList);
            InternalPartsList = null;
        }

        isDisposed = true;
    }

    private uint PartCount {
        get => InternalPartsList->PartCount;
        set => InternalPartsList->PartCount = value;
    }

    public void Add(params Part[] items) {
        foreach (var part in items) {
            Add(part);
        }
    }

    public AtkUldPart* Add(Part item) {
        NativeMemoryHelper.ResizeArray(ref InternalPartsList->Parts, PartCount, PartCount + 1);

        ref var newPart = ref InternalPartsList->Parts[PartCount];

        newPart.Width = (ushort) item.Width;
        newPart.Height = (ushort) item.Height;
        newPart.U = (ushort) item.U;
        newPart.V = (ushort) item.V;
        
        newPart.UldAsset = NativeMemoryHelper.UiAlloc<AtkUldAsset>();
        newPart.UldAsset->Id = item.Id;
        newPart.UldAsset->AtkTexture.Ctor();
        newPart.LoadTexture(item.TexturePath);

        return &InternalPartsList->Parts[PartCount++];
    }

    public AtkUldPart* this[int index] {
        get {
            if (InternalPartsList is null) return null;
            if (PartCount <= index) return null;

            return &InternalPartsList->Parts[index];
        }
    }
}
