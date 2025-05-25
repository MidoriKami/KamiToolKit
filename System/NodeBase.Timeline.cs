using KamiToolKit.NodeParts;

namespace KamiToolKit.System;

public abstract unsafe partial class NodeBase {

	public Timeline? Timeline { get; private set; }

	public void AddTimeline(Timeline timeline) {
		if (Timeline is not null) return;
		
		Timeline = timeline;
		InternalResNode->Timeline = timeline.InternalTimeline;
		timeline.OwnerNode = InternalResNode;
	}
}