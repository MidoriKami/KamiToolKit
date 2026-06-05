using KamiToolKit.Timelines;

namespace KamiToolKit;

/// <summary>
/// NodeBase partial responsible for managing Timelines.
/// </summary>
public abstract unsafe partial class NodeBase {

    /// <summary>
    /// Gets this nodes timeline.
    /// </summary>
    public Timeline? Timeline { get; private set; }

    /// <summary>
    /// Adds a built timeline to this node.
    /// </summary>
    /// <remarks>
    /// Disposes the previously used timeline. <em>Potentially volatile when replacing an existing timeline</em>.
    /// </remarks>
    public void AddTimeline(Timeline timeline) {
        Timeline?.Dispose();

        Timeline = timeline;
        ResNode->Timeline = timeline.InternalTimeline;
        timeline.OwnerNode = ResNode;
    }
}
