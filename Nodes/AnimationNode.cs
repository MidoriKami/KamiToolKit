using KamiToolKit.BaseTypes;
using KamiToolKit.Interfaces;
using KamiToolKit.Timelines;

namespace KamiToolKit.Nodes;

/// <summary>
/// Generic wrapper around a node used for acting as a container for an animated node.
/// </summary>
/// <typeparam name="T">The node type to be contained within this node.</typeparam>
public class AnimationNode<T> : ResNode, IAnimationNode<T> where T : NodeBase, new() {

    /// <inheritdoc/>
    public T ContentNode { get; }

    /// <inheritdoc/>
    public Timeline LabelsetTimeline {
        set => AddTimeline(value);
    }

    /// <inheritdoc/>
    public Timeline ContentTimeline {
        set => ContentNode.AddTimeline(value);
    }

    /// <summary>
    /// Constructs a new instance of <see cref="AnimationNode{T}"/>
    /// </summary>
    public AnimationNode() {
        ContentNode = new T();
        ContentNode.AttachNode(this);
    }

    /// <inheritdoc/>
    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        ContentNode.Size = Size;
    }
}
