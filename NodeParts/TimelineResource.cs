using System;
using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit.NodeParts;

internal unsafe class TimelineResource : IDisposable {
	internal AtkTimelineResource* InternalResource;

	private readonly List<TimelineAnimation> animations = [];
	private readonly List<TimelineLabelSet> labelSets = [];

	public TimelineResource() {
		InternalResource = NativeMemoryHelper.UiAlloc<AtkTimelineResource>();

		InternalResource->Id = 0;
		InternalResource->AnimationCount = 0;
		InternalResource->LabelSetCount = 0;
		InternalResource->Animations = null;
		InternalResource->LabelSets = null;
	}
	
	public void Dispose() {
		NativeMemoryHelper.UiFree(InternalResource);
		InternalResource = null;
	}
}