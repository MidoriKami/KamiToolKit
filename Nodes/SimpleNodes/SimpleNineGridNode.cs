using System.Numerics;
using Dalamud.Plugin.Services;
using KamiToolKit.Classes;
using Newtonsoft.Json;

namespace KamiToolKit.Nodes;

public class SimpleNineGridNode : NineGridNode {
    public SimpleNineGridNode() {
        PartsList.Add(new Part());
    }
    
    [JsonIgnore] public float U {
        get => PartsList[0].U;
        set => PartsList[0].U = (ushort) value;
    }
    
    [JsonIgnore] public float V {
        get => PartsList[0].V;
        set => PartsList[0].V = (ushort) value;
    }

    [JsonIgnore] public Vector2 TextureCoordinates {
        get => new(U, V);
        set {
            U = value.X;
            V = value.Y;
        }
    }

    [JsonIgnore] public float TextureWidth {
        get => PartsList[0].Width;
        set => PartsList[0].Width = (ushort) value;
    }
    
    [JsonIgnore] public float TextureHeight {
        get => PartsList[0].Height;
        set => PartsList[0].Height = (ushort) value;
    }
    
    [JsonIgnore] public Vector2 TextureSize {
        get => new(TextureWidth, TextureHeight);
        set {
            TextureWidth = value.X;
            TextureHeight = value.Y;
        }
    }

    [JsonIgnore] public string TexturePath {
        set => PartsList[0].LoadTexture(value);
    }
    
    public void LoadTexture(string path, ITextureSubstitutionProvider? substitutionProvider)
        => PartsList[0].LoadTexture(path, substitutionProvider);
}