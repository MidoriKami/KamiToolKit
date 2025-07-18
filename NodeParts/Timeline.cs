using System;
using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit.NodeParts;

public unsafe class Timeline : IDisposable {

	internal AtkTimeline* InternalTimeline;
	
	private readonly TimelineResource internalTimelineResource;
	
	public Timeline() {
		InternalTimeline = NativeMemoryHelper.UiAlloc<AtkTimeline>();

		internalTimelineResource = new TimelineResource();
		InternalTimeline->Resource = internalTimelineResource.InternalResource;
		
		InternalTimeline->LabelResource = null;
		
		InternalTimeline->FrameTime = 0.033333335f;
		InternalTimeline->ParentFrameTime = 0.333333334f;
		InternalTimeline->LabelFrameIdxDuration = 0;
		InternalTimeline->LabelEndFrameIdx = 1;
		InternalTimeline->ActiveLabelId = 0;
		InternalTimeline->Mask = 0;
		InternalTimeline->Flags = 0;
		InternalTimeline->ActiveAnimation = null;
		InternalTimeline->OwnerNode = null;
	}

	public void Dispose() {
		internalTimelineResource.Dispose();
		
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

	public void StartAnimation(int i) {
		InternalTimeline->PlayAnimation(AtkTimelineJumpBehavior.Start, (byte) i);
	}

	public void StopAnimation() {
		InternalTimeline->PlayAnimation(AtkTimelineJumpBehavior.Start, 0);
	}
}