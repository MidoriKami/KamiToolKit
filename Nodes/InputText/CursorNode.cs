using System.Numerics;
using KamiToolKit.Classes.TimelineBuilding;

namespace KamiToolKit.Nodes;

public class CursorNode : ResNode {

    public readonly SimpleImageNode CursorImageNode;

    public CursorNode() {
        CursorImageNode = new SimpleImageNode {
            TexturePath = "ui/uld/TextInputA.tex",
            Size = new Vector2(4.0f, 24.0f),
            TextureCoordinates = new Vector2(68.0f, 0.0f),
            TextureSize = new Vector2(4.0f, 24.0f),
            IsVisible = true,
            WrapMode = 1,
            ImageNodeFlags = 0x0,
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
