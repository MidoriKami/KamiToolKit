using KamiToolKit.Classes.TimelineBuilding;
using KamiToolKit.NodeParts;

namespace KamiToolKit.System;

public abstract unsafe partial class NodeBase {

    public Timeline? Timeline { get; private set; }

    public void AddTimeline(Timeline timeline) {
        Timeline?.Dispose();

        Timeline = timeline;
        ResNode->Timeline = timeline.InternalTimeline;
        timeline.OwnerNode = ResNode;
    }

    public void AddTimeline(TimelineBuilder builder)
        => AddTimeline(builder.Build());
}
