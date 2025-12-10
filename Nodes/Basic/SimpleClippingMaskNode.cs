using System.Numerics;
using KamiToolKit.Classes;

namespace KamiToolKit.Nodes;

public unsafe class SimpleClippingMaskNode : ClippingMaskNode {
    public SimpleClippingMaskNode() {
        PartsList.Add(new Part());
    }

    public float U {
        get => PartsList[0]->U;
        set => PartsList[0]->U = (ushort)value;
    }

    public float V {
        get => PartsList[0]->V;
        set => PartsList[0]->V = (ushort)value;
    }

    public Vector2 TextureCoordinates {
        get => new(U, V);
        set {
            U = value.X;
            V = value.Y;
        }
    }

    public float TextureHeight {
        get => PartsList[0]->Height;
        set => PartsList[0]->Height = (ushort)value;
    }

    public float TextureWidth {
        get => PartsList[0]->Width;
        set => PartsList[0]->Width = (ushort)value;
    }

    public Vector2 TextureSize {
        get => new(TextureWidth, TextureHeight);
        set {
            TextureWidth = value.X;
            TextureHeight = value.Y;
        }
    }

    public virtual string TexturePath {
        get => PartsList[0]->GetLoadedPath();
        set => PartsList[0]->LoadTexture(value);
    }

    public Vector2 ActualTextureSize => PartsList[0]->GetActualTextureSize();

    public void LoadTexture(string path)
        => PartsList[0]->LoadTexture(path);

    public void LoadIcon(uint iconId)
        => PartsList[0]->LoadIcon(iconId);
}
