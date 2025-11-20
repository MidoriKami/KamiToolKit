using System.Numerics;
using KamiToolKit.Classes;

namespace KamiToolKit.Nodes;

public unsafe class SimpleCounterNode : CounterNode {
    public SimpleCounterNode() {
        PartsList.Add(new Part {
            TexturePath = "ui/uld/Money_Number.tex", 
            TextureCoordinates = Vector2.Zero,
            Size = new Vector2(22.0f, 22.0f), 
        });
    }
}
