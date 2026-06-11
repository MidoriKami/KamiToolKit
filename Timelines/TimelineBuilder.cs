using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace KamiToolKit.Timelines;

/// <summary>
/// The main class used for building a custom timeline/timeline animation.
/// </summary>
public class TimelineBuilder {

    /// <summary>
    /// The main function to start building a timeline.
    /// Give this function the full range of frames you want to build timelines for.
    /// </summary>
    /// <remarks>
    /// Every 60 keyframes represents a 1second animation regardless of the games actual framerate.
    /// </remarks>
    /// <param name="startFrameId">The starting frame index.</param>
    /// <param name="endFrameId">The ending frame index.</param>
    public FrameSetBuilder BeginFrameSet(int startFrameId, int endFrameId)
        => new(this, startFrameId, endFrameId);

    /// <summary>
    /// Directly adds a full frameset with specified animation.
    /// </summary>
    public TimelineBuilder AddFrameSetWithFrame(
        int startFrameId, int endFrameId, int frameId, Vector2? position = null, byte? alpha = null, Vector3? addColor = null, Vector3? multiplyColor = null,
        float? rotation = null, Vector2? scale = null, Vector3? textColor = null, Vector3? textOutlineColor = null, uint? partId = null) {

        new FrameSetBuilder(this, startFrameId, endFrameId)
            .AddFrame(frameId, position, alpha, addColor, multiplyColor, rotation, scale, textColor, textOutlineColor, partId)
            .EndFrameSet();

        return this;
    }

    /// <summary>
    /// Begins a frameset builder for the specified ranges, but targeting a specific frame index for building the animation.
    /// </summary>
    public KeyFrameBuilder AddFrame(int frameSetStart, int frameSetEnd, int frameIndex)
        => new(new FrameSetBuilder(this, frameSetStart, frameSetEnd), frameIndex);

    /// <summary>
    /// Constructs the actual native object that will be set in the games memory to do the animations.
    /// </summary>
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

    internal List<TimelineAnimation> Animations = [];
    internal List<TimelineLabelSet> LabelSets = [];
}
