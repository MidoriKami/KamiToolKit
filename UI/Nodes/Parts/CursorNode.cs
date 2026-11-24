using System.Numerics;
using KamiToolKit.Core.Enums;
using KamiToolKit.Services.Timelines;
using KamiToolKit.UI.Nodes.ComponentNode;
using KamiToolKit.UI.Nodes.Simplified;

namespace KamiToolKit.UI.Nodes.Parts;

/// <summary>
/// Node part used for <see cref="TextInputNode"/>, not intended for external use.
/// </summary>
public class CursorNode : ResNode {

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public SimpleImageNode CursorImageNode { get; }

    /// <summary>
    /// Constructs a new <see cref="CursorNode"/>
    /// </summary>
    public CursorNode() {
        CursorImageNode = new SimpleImageNode {
            NodeId = 3,
            TexturePath = "ui/uld/TextInputA.tex",
            Size = new Vector2(4.0f, 24.0f),
            TextureCoordinates = new Vector2(68.0f, 0.0f),
            TextureSize = new Vector2(4.0f, 24.0f),
            WrapMode = WrapMode.Tile,
        };
        CursorImageNode.AttachNode(this);

        CursorImageNode.AddTimeline(new TimelineBuilder()
            .BeginFrameSet(1, 8)
            .AddEmptyFrame(1)
            .EndFrameSet()
            .Build());

        Timeline?.PlayAnimation(101);
    }
}
