using System.Numerics;

namespace KamiToolKit.NodeParts;

public class Part {

    public float Width { get; set; }

    public float Height { get; set; }

    public Vector2 Size {
        get => new(Width, Height);
        set {
            Width = value.X;
            Height = value.Y;
        }
    }

    public float U { get; set; }

    public float V { get; set; }

    public Vector2 TextureCoordinates {
        get => new(U, V);
        set {
            U = value.X;
            V = value.Y;
        }
    }

    public uint Id { get; set; }

    public string TexturePath { get; set; } = string.Empty;
}
