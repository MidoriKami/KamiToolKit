using KamiToolKit.BaseTypes;
using KamiToolKit.Timelines;

namespace KamiToolKit.Nodes;

/// <summary>
/// Alias for <see cref="TimelineNode{T}"/> to make it easier to fine and use this node.
/// </summary>
/// <typeparam name="T">The node type to be contained within this node.</typeparam>
public class AnimationNode<T> : TimelineNode<T> where T : NodeBase, new();

/// <summary>
/// Generic wrapper around a node used for acting as a container for a animated node.
/// </summary>
/// <typeparam name="T">The node type to be contained within this node.</typeparam>
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

    /// <summary>
    /// Constructs a new instance of <see cref="TimelineNode{T}"/>
    /// </summary>
    public TimelineNode() {
        ContentNode = new T();
        ContentNode.AttachNode(this);
    }

    /// <inheritdoc/>
    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        ContentNode.Size = Size;
    }
}
