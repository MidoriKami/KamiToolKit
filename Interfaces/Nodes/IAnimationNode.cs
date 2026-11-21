using KamiToolKit.Timelines;

namespace KamiToolKit.Interfaces;

/// <summary>
/// Interface representing a node used for simplifying usages of animations.
/// </summary>
public interface IAnimationNode<out T> {

    /// <summary>
    /// Gets the contained animated node.
    /// </summary>
    T ContentNode { get; }

    /// <summary>
    /// Sets the timeline of the parent animation container.
    /// </summary>
    /// <remarks>
    /// This needs to be the labelsets that define the animation stops and loops.
    /// </remarks>
    Timeline LabelsetTimeline { set; }

    /// <summary>
    /// Sets the timeline of the content node.
    /// </summary>
    /// <remarks>
    /// This needs to be the framesets and keyframes for the actual animation.
    /// </remarks>
    Timeline ContentTimeline { set; }
}
