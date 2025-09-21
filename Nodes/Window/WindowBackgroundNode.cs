﻿using System.Numerics;
using KamiToolKit.NodeParts;

namespace KamiToolKit.Nodes;

public class WindowBackgroundNode : NineGridNode {
    public WindowBackgroundNode(bool selectedPath, string path = "ui/uld/WindowA_Bg") {
        var basePath = $"{path}{(selectedPath ? "Selected" : "Normal")}";

        PartsList.Add(
            new Part {
                TextureCoordinates = new Vector2(0.0f, 0.0f), Size = new Vector2(16.0f, 64.0f), Id = 0, TexturePath = $"{basePath}_Corner.tex",
            }, new Part {
                TextureCoordinates = new Vector2(0.0f, 0.0f), Size = new Vector2(32.0f, 64.0f), Id = 1, TexturePath = $"{basePath}_H.tex",
            }, new Part {
                TextureCoordinates = new Vector2(16.0f, 0.0f), Size = new Vector2(16.0f, 64.0f), Id = 2, TexturePath = $"{basePath}_Corner.tex",
            }, new Part {
                TextureCoordinates = new Vector2(0.0f, 0.0f), Size = new Vector2(16.0f, 32.0f), Id = 3, TexturePath = $"{basePath}_V.tex",
            }, new Part {
                TextureCoordinates = new Vector2(0.0f, 0.0f), Size = new Vector2(32.0f, 32.0f), Id = 4, TexturePath = $"{basePath}_HV.tex",
            }, new Part {
                TextureCoordinates = new Vector2(16.0f, 0.0f), Size = new Vector2(16.0f, 32.0f), Id = 5, TexturePath = $"{basePath}_V.tex",
            }, new Part {
                TextureCoordinates = new Vector2(0.0f, 64.0f), Size = new Vector2(16.0f, 32.0f), Id = 6, TexturePath = $"{basePath}_Corner.tex",
            }, new Part {
                TextureCoordinates = new Vector2(0.0f, 64.0f), Size = new Vector2(32.0f, 32.0f), Id = 7, TexturePath = $"{basePath}_H.tex",
            }, new Part {
                TextureCoordinates = new Vector2(16.0f, 64.0f), Size = new Vector2(16.0f, 32.0f), Id = 8, TexturePath = $"{basePath}_Corner.tex",
            }
        );
    }
}
