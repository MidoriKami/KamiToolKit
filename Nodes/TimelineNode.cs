using KamiToolKit.BaseTypes;
using KamiToolKit.Timelines;

namespace KamiToolKit.Nodes;

/// <summary>
/// Generic wrapper around a node used for acting as a container for a animated node.
/// </summary>
public class TimelineNode<T> : ResNode where T : NodeBase, new() {

    /// <summary>
    /// Gets the contained animated node.
    /// </summary>
    public T ContentNode { get; }

    /// <summary>
    /// Sets the timeline of the parent animation container.
    /// </summary>
    /// <remarks>
    /// This needs to be the labelsets that define the animation stops and loops.
    /// </remarks>
    public Timeline LabelsetTimeline {
        set => AddTimeline(value);
    }

    /// <summary>
    /// Sets the timeline of the content node.
    /// </summary>
    /// <remarks>
    /// This needs to be the framesets and keyframes for the actual animation.
    /// </remarks>
    public Timeline ContentTimeline {
        set => ContentNode.AddTimeline(value);
    }

    public TimelineNode() {
        ContentNode = new T();
        ContentNode.AttachNode(this);
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        ContentNode.Size = Size;
    }
}
