using System;
using System.Numerics;
using Dalamud.Interface.Textures.TextureWraps;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit.Nodes.Parts;

public unsafe class Part : IDisposable {
    internal readonly AtkUldPart* InternalPart;
 
    public Asset Asset { get; } = new();
    
    public Part() {
        InternalPart = NativeMemoryHelper.UiAlloc<AtkUldPart>();

        InternalPart->Height = 0;
        InternalPart->Width = 0;
        InternalPart->U = 0;
        InternalPart->V = 0;

        InternalPart->UldAsset = Asset.InternalAsset;
    }

    public void Dispose() {
        Asset.Dispose();
        NativeMemoryHelper.UiFree(InternalPart);
    }

    public float Width {
        get => InternalPart->Width;
        set => InternalPart->Width = (ushort) value;
    }

    public float Height {
        get => InternalPart->Height;
        set => InternalPart->Height = (ushort) value;
    }

    public Vector2 Size {
        get => new(Width, Height);
        set {
            Width = value.X;
            Height = value.Y;
        }
    }

    public float U {
        get => InternalPart->U;
        set => InternalPart->U = (ushort) value;
    }

    public float V {
        get => InternalPart->V;
        set => InternalPart->V = (ushort) value;
    }

    public Vector2 TextureCoordinates {
        get => new(U, V);
        set {
            U = value.X;
            V = value.Y;
        }
    }

    public uint Id {
        get => Asset.Id;
        set => Asset.Id = value;
    }

    public void LoadTexture(string path)
        => Asset.InternalAsset->AtkTexture.LoadTexture(path);

    public void UnloadTexture()
        => Asset.InternalAsset->AtkTexture.ReleaseTexture();

    public void LoadIcon(uint iconId)
        => Asset.InternalAsset->AtkTexture.LoadIconTexture(iconId, 0);

    public void LoadImGuiTexture(IDalamudTextureWrap textureWrap)
        => Asset.LoadImGuiTexture(textureWrap);
}