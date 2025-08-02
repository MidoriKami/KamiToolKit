using System.Numerics;
using KamiToolKit.NodeParts;

namespace KamiToolKit.Nodes;

public class SimpleCounterNode : CounterNode {
    public SimpleCounterNode() {
        PartsList[0] = new Part {
            TexturePath = "ui/uld/Money_Number.tex", Size = new Vector2(22.0f, 22.0f), TextureCoordinates = Vector2.Zero,
        };
    }
}
