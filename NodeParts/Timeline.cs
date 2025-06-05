using System;
using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit.NodeParts;

public unsafe class Timeline : IDisposable {

	internal AtkTimeline* InternalTimeline;
	
	private readonly TimelineResource internalTimelineResource;
	// private readonly TimelineResource internalLabelSetResource;
	
	public Timeline() {
		InternalTimeline = NativeMemoryHelper.UiAlloc<AtkTimeline>();

		internalTimelineResource = new TimelineResource();
		InternalTimeline->Resource = internalTimelineResource.InternalResource;
		
		// internalLabelSetResource = new TimelineResource();
		// InternalTimeline->LabelResource = internalLabelSetResource.InternalResource;
		InternalTimeline->LabelResource = null;
		
		InternalTimeline->ActiveAnimation = null;
		InternalTimeline->OwnerNode = null;
	}

	public void Dispose() {
		internalTimelineResource.Dispose();
		// internalLabelSetResource.Dispose();
		
		NativeMemoryHelper.UiFree(InternalTimeline);
		InternalTimeline = null;
	}

	internal AtkResNode* OwnerNode {
		get => InternalTimeline->OwnerNode;
		set => InternalTimeline->OwnerNode = value;
	}

	public float FrameTime {
		get => InternalTimeline->FrameTime;
		set => InternalTimeline->FrameTime = value;
	}

	public float ParentFrameTime {
		get => InternalTimeline->ParentFrameTime;
		set => InternalTimeline->ParentFrameTime = value;
	}

	public int LabelFrameIdxDuration {
		get => InternalTimeline->LabelFrameIdxDuration;
		set => InternalTimeline->LabelFrameIdxDuration = (ushort) value;
	}

	public int LabelEndFrameIdx {
		get => InternalTimeline->LabelEndFrameIdx;
		set => InternalTimeline->LabelEndFrameIdx = (ushort) value;
	}

	public int ActiveLabelId {
		get => InternalTimeline->ActiveLabelId;
		set => InternalTimeline->ActiveLabelId = (ushort) value;
	}

	public AtkTimelineMask Mask {
		get => InternalTimeline->Mask;
		set => InternalTimeline->Mask = value;
	}

	public AtkTimelineFlags Flags {
		get => InternalTimeline->Flags;
		set => InternalTimeline->Flags = value;
	}

	public List<TimelineAnimation> Animations {
		set => internalTimelineResource.Animations = value;
	}

	public List<TimelineLabelSet> LabelSets {
		set => internalTimelineResource.LabelSets = value;
	}
}