using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using KamiToolKit.NodeParts;

namespace KamiToolKit.Classes.TimelineBuilding;

public class TimelineBuilder {

    internal List<TimelineAnimation> Animations = [];
    internal List<TimelineLabelSet> LabelSets = [];

    public FrameSetBuilder BeginFrameSet(int startFrameId, int endFrameId)
        => new(this, startFrameId, endFrameId);

    public TimelineBuilder AddFrameSetWithFrame(
        int startFrameId, int endFrameId, int frameId, Vector2? position = null, byte? alpha = null, Vector3? addColor = null, Vector3? multiplyColor = null,
        float? rotation = null, Vector2? scale = null, Vector3? textColor = null, Vector3? textOutlineColor = null, uint? partId = null) {

        new FrameSetBuilder(this, startFrameId, endFrameId)
            .AddFrame(frameId, position, alpha, addColor, multiplyColor, rotation, scale, textColor, textOutlineColor, partId)
            .EndFrameSet();

        return this;
    }

    public KeyFrameBuilder AddFrame(int frameSetStart, int frameSetEnd, int frameIndex)
        => new(new FrameSetBuilder(this, frameSetStart, frameSetEnd), frameIndex);

    public Timeline Build() {
        var newTimeline = new Timeline();

        if (LabelSets.Count != 0) {
            newTimeline.LabelSets = LabelSets;
            newTimeline.LabelFrameIdxDuration = LabelSets.Max(label => label.EndFrameId) - 1;
            newTimeline.LabelEndFrameIdx = LabelSets.Max(label => label.EndFrameId);
        }

        if (Animations.Count != 0) {
            newTimeline.Animations = Animations;
        }

        return newTimeline;
    }
}
