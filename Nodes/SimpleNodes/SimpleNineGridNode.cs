using System.Numerics;
using KamiToolKit.Nodes.Parts;

namespace KamiToolKit.Nodes;

public class SimpleNineGridNode : NineGridNode {
    public SimpleNineGridNode() {
        PartsList.Add(new Part());
    }
    
    public float U {
        get => PartsList[0].U;
        set => PartsList[0].U = (ushort) value;
    }
    
    public float V {
        get => PartsList[0].V;
        set => PartsList[0].V = (ushort) value;
    }

    public Vector2 TextureCoordinates {
        get => new(U, V);
        set {
            U = value.X;
            V = value.Y;
        }
    }

    public float TextureWidth {
        get => PartsList[0].Width;
        set => PartsList[0].Width = (ushort) value;
    }
    
    public float TextureHeight {
        get => PartsList[0].Height;
        set => PartsList[0].Height = (ushort) value;
    }
    
    public Vector2 TextureSize {
        get => new(TextureWidth, TextureHeight);
        set {
            TextureWidth = value.X;
            TextureHeight = value.Y;
        }
    }

    public string TexturePath {
        set => PartsList[0].LoadTexture(value);
    }
}