using System.Linq;
using System.Numerics;
using KamiToolKit.NodeParts;

namespace KamiToolKit.Nodes;

public static class IconNodeTextureHelper {
    public static void LoadIconAFrameTexture(ImageNode image) {
        image.AddPart(new Part {
            Id = 0, TexturePath = "ui/uld/IconA_Frame.tex", Size = new Vector2(48.0f, 48.0f),
        });
        image.AddPart(new Part {
            Id = 1, TexturePath = "ui/uld/IconA_Frame.tex", Size = new Vector2(48.0f, 48.0f), TextureCoordinates = new Vector2(48.0f, 0.0f),
        });
        image.AddPart(new Part {
            Id = 2, TexturePath = "ui/uld/IconA_Frame.tex", Size = new Vector2(48.0f, 48.0f), TextureCoordinates = new Vector2(0.0f, 48.0f),
        });
        image.AddPart(new Part {
            Id = 3, TexturePath = "ui/uld/IconA_Frame.tex", Size = new Vector2(48.0f, 48.0f), TextureCoordinates = new Vector2(48.0f, 48.0f),
        });
        image.AddPart(new Part {
            Id = 4, TexturePath = "ui/uld/IconA_Frame.tex", Size = new Vector2(48.0f, 48.0f), TextureCoordinates = new Vector2(0.0f, 96.0f),
        });
        image.AddPart(new Part {
            Id = 5, TexturePath = "ui/uld/IconA_Frame.tex", Size = new Vector2(48.0f, 48.0f), TextureCoordinates = new Vector2(48.0f, 96.0f),
        });
        image.AddPart(new Part {
            Id = 6, TexturePath = "ui/uld/IconA_Frame.tex", Size = new Vector2(48.0f, 48.0f), TextureCoordinates = new Vector2(96.0f, 0.0f),
        });
        image.AddPart(new Part {
            Id = 7, TexturePath = "ui/uld/IconA_Frame.tex", Size = new Vector2(48.0f, 48.0f), TextureCoordinates = new Vector2(144.0f, 0.0f),
        });
        image.AddPart(new Part {
            Id = 8, TexturePath = "ui/uld/IconA_Frame.tex", Size = new Vector2(48.0f, 48.0f), TextureCoordinates = new Vector2(192.0f, 0.0f),
        });
        image.AddPart(new Part {
            Id = 9, TexturePath = "ui/uld/IconA_Frame.tex", Size = new Vector2(48.0f, 48.0f), TextureCoordinates = new Vector2(96.0f, 48.0f),
        });
        image.AddPart(new Part {
            Id = 10, TexturePath = "ui/uld/IconA_Frame.tex", Size = new Vector2(48.0f, 48.0f), TextureCoordinates = new Vector2(144.0f, 48.0f),
        });
        image.AddPart(new Part {
            Id = 11, TexturePath = "ui/uld/IconA_Frame.tex", Size = new Vector2(48.0f, 48.0f), TextureCoordinates = new Vector2(192.0f, 48.0f),
        });
        image.AddPart(new Part {
            Id = 12, TexturePath = "ui/uld/IconA_Frame.tex", Size = new Vector2(48.0f, 48.0f), TextureCoordinates = new Vector2(96.0f, 96.0f),
        });
        image.AddPart(new Part {
            Id = 13, TexturePath = "ui/uld/IconA_Frame.tex", Size = new Vector2(48.0f, 48.0f), TextureCoordinates = new Vector2(144.0f, 96.0f),
        });
        image.AddPart(new Part {
            Id = 14, TexturePath = "ui/uld/IconA_Frame.tex", Size = new Vector2(18.0f, 18.0f), TextureCoordinates = new Vector2(192.0f, 96.0f),
        });
        image.AddPart(new Part {
            Id = 15, TexturePath = "ui/uld/IconA_Frame.tex", Size = new Vector2(18.0f, 18.0f), TextureCoordinates = new Vector2(192.0f, 114.0f),
        });
        image.AddPart(new Part {
            Id = 16, TexturePath = "ui/uld/IconA_Frame.tex", Size = new Vector2(72.0f, 72.0f), TextureCoordinates = new Vector2(240.0f, 0.0f),
        });
        image.AddPart(new Part {
            Id = 17, TexturePath = "ui/uld/IconA_Frame.tex", Size = new Vector2(64.0f, 64.0f), TextureCoordinates = new Vector2(240.0f, 72.0f),
        });
        image.AddPart(new Part {
            Id = 18, TexturePath = "ui/uld/IconA_Frame.tex", Size = new Vector2(48.0f, 48.0f), TextureCoordinates = new Vector2(312.0f, 0.0f),
        });
        image.AddPart(new Part {
            Id = 19, TexturePath = "ui/uld/IconA_Frame.tex", Size = new Vector2(48.0f, 48.0f), TextureCoordinates = new Vector2(312.0f, 48.0f),
        });
        image.AddPart(new Part {
            Id = 20, TexturePath = "ui/uld/IconA_Frame.tex", Size = new Vector2(48.0f, 48.0f), TextureCoordinates = new Vector2(312.0f, 96.0f),
        });
        image.AddPart(new Part {
            Id = 21, TexturePath = "ui/uld/IconA_Frame.tex", Size = new Vector2(18.0f, 18.0f), TextureCoordinates = new Vector2(210.0f, 114.0f),
        });
        image.AddPart(new Part {
            Id = 22, TexturePath = "ui/uld/IconA_Frame.tex", Size = new Vector2(18.0f, 18.0f), TextureCoordinates = new Vector2(360.0f, 96.0f),
        });
        image.AddPart(new Part {
            Id = 23, TexturePath = "ui/uld/IconA_Frame.tex", Size = new Vector2(18.0f, 18.0f), TextureCoordinates = new Vector2(378.0f, 96.0f),
        });
        image.AddPart(new Part {
            Id = 24, TexturePath = "ui/uld/IconA_Frame.tex", Size = new Vector2(18.0f, 18.0f), TextureCoordinates = new Vector2(360.0f, 114.0f),
        });
        image.AddPart(new Part {
            Id = 25, TexturePath = "ui/uld/IconA_Frame.tex", Size = new Vector2(18.0f, 18.0f), TextureCoordinates = new Vector2(210.0f, 96.0f),
        });
        image.AddPart(new Part {
            Id = 26, TexturePath = "ui/uld/IconA_Frame.tex", Size = new Vector2(18.0f, 18.0f), TextureCoordinates = new Vector2(408.0f, 0.0f),
        });
        image.AddPart(new Part {
            Id = 27, TexturePath = "ui/uld/IconA_Frame.tex", Size = new Vector2(18.0f, 18.0f), TextureCoordinates = new Vector2(408.0f, 18.0f),
        });
        image.AddPart(new Part {
            Id = 28, TexturePath = "ui/uld/IconA_Frame.tex", Size = new Vector2(18.0f, 18.0f), TextureCoordinates = new Vector2(408.0f, 36.0f),
        });
        image.AddPart(new Part {
            Id = 29, TexturePath = "ui/uld/IconA_Frame.tex", Size = new Vector2(18.0f, 18.0f), TextureCoordinates = new Vector2(396.0f, 96.0f),
        });
        image.AddPart(new Part {
            Id = 30, TexturePath = "ui/uld/IconA_Frame.tex", Size = new Vector2(18.0f, 18.0f), TextureCoordinates = new Vector2(396.0f, 114.0f),
        });
    }

    public static void LoadIconARecast2Texture(ImageNode imageNode) {
        foreach (var yIndex in Enumerable.Range(0, 9))
        foreach (var xIndex in Enumerable.Range(0, 9)) {
            var coordinate = new Vector2(xIndex * 44.0f, yIndex * 48.0f);
            imageNode.AddPart(new Part {
                TexturePath = "ui/uld/IconA_Recast2.tex", TextureCoordinates = coordinate, Size = new Vector2(44.0f, 46.0f), Id = (uint)(xIndex + yIndex),
            });
        }

        foreach (var yIndex in Enumerable.Range(9, 9))
        foreach (var xIndex in Enumerable.Range(9, 9)) {
            var coordinate = new Vector2(xIndex * 44.0f, (yIndex - 9) * 48.0f);
            imageNode.AddPart(new Part {
                TexturePath = "ui/uld/IconA_Recast2.tex", TextureCoordinates = coordinate, Size = new Vector2(44.0f, 46.0f), Id = (uint)(xIndex + yIndex),
            });
        }
    }

    public static void LoadIconARecastTexture(ImageNode imageNode) {
        foreach (var yIndex in Enumerable.Range(0, 9))
        foreach (var xIndex in Enumerable.Range(0, 9)) {
            var coordinate = new Vector2(xIndex * 44.0f, yIndex * 48.0f);
            imageNode.AddPart(new Part {
                TexturePath = "ui/uld/IconA_Recast.tex", TextureCoordinates = coordinate, Size = new Vector2(44.0f, 46.0f), Id = (uint)(xIndex + yIndex),
            });
        }
    }
}
