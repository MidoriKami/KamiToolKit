using System.Numerics;
using KamiToolKit.Classes;

namespace KamiToolKit.Nodes.Simplified;

/// <summary>
/// A simplified implementation of a <see cref="CounterNode"/>, with default font set to Money font.
/// </summary>
public unsafe class SimpleCounterNode : CounterNode {

    /// <summary>
    /// Constructs a new <see cref="SimpleCounterNode"/>
    /// </summary>
    public SimpleCounterNode() {
        PartsList.Add(new Part {
            TexturePath = "ui/uld/Money_Number.tex",
            TextureCoordinates = Vector2.Zero,
            Size = new Vector2(22.0f, 22.0f),
        });
    }
}
