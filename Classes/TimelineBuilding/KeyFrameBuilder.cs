using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Common.Math;
using KamiToolKit.NodeParts;

namespace KamiToolKit.Classes.TimelineBuilding;

public class KeyFrameBuilder(FrameSetBuilder parent, int frame) {

    private readonly List<TimelineKeyFrame> animationKeyFrames = [];

    public KeyFrameBuilder Position(Vector2 position) {
        animationKeyFrames.Add(new TimelineAnimationKeyFrame {
            FrameIndex = frame, Position = position,
        });

        return this;
    }

    public KeyFrameBuilder Alpha(byte alpha) {
        animationKeyFrames.Add(new TimelineAnimationKeyFrame {
            FrameIndex = frame, Alpha = alpha,
        });

        return this;
    }

    public KeyFrameBuilder AddColor(Vector3 color) {
        animationKeyFrames.Add(new TimelineAnimationKeyFrame {
            FrameIndex = frame, AddColor = color,
        });

        return this;
    }

    public KeyFrameBuilder MultiplyColor(Vector3 color) {
        animationKeyFrames.Add(new TimelineAnimationKeyFrame {
            FrameIndex = frame, MultiplyColor = color,
        });

        return this;
    }

    public KeyFrameBuilder MultiplyColor(float color) {
        animationKeyFrames.Add(new TimelineAnimationKeyFrame {
            FrameIndex = frame, MultiplyColor = new Vector3(color, color, color),
        });

        return this;
    }

    public KeyFrameBuilder Rotation(float rotation) {
        animationKeyFrames.Add(new TimelineAnimationKeyFrame {
            FrameIndex = frame, Rotation = rotation,
        });

        return this;
    }

    public KeyFrameBuilder Scale(Vector2 scale) {
        animationKeyFrames.Add(new TimelineAnimationKeyFrame {
            FrameIndex = frame, Scale = scale,
        });

        return this;
    }

    public KeyFrameBuilder Scale(float scale) {
        animationKeyFrames.Add(new TimelineAnimationKeyFrame {
            FrameIndex = frame, Scale = new Vector2(scale, scale),
        });

        return this;
    }

    public KeyFrameBuilder TextColor(Vector3 textColor) {
        animationKeyFrames.Add(new TimelineAnimationKeyFrame {
            FrameIndex = frame, TextColor = textColor,
        });

        return this;
    }

    public KeyFrameBuilder TextOutlineColor(Vector3 textColor) {
        animationKeyFrames.Add(new TimelineAnimationKeyFrame {
            FrameIndex = frame, TextEdgeColor = textColor,
        });

        return this;
    }

    public KeyFrameBuilder Part(uint partId) {
        animationKeyFrames.Add(new TimelineAnimationKeyFrame {
            FrameIndex = frame, PartId = partId,
        });

        return this;
    }

    public FrameSetBuilder EndFrameBuilder() {
        parent.AddFrame(animationKeyFrames.ToArray());
        return parent;
    }
}
