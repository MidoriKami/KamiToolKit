using System;
using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit.NodeParts;

public unsafe class TimelineResource : IDisposable {
	internal AtkTimelineResource* InternalResource;
	
	private readonly TimelineAnimationArray animationArray;
	
	public TimelineResource() {
		InternalResource = NativeMemoryHelper.UiAlloc<AtkTimelineResource>();

		InternalResource->Id = 1;
		InternalResource->AnimationCount = 0;
		InternalResource->LabelSetCount = 0;
		
		animationArray = new TimelineAnimationArray();
		InternalResource->Animations = animationArray.InternalTimelineArray; // Should be null here, as we don't make any animations by default.
		
		InternalResource->LabelSets = null;
	}
	
	public void Dispose() {
		animationArray.Dispose();
		
		NativeMemoryHelper.UiFree(InternalResource);
		InternalResource = null;
	}

	public List<TimelineAnimation> Animations {
		get => animationArray.Animations;
		set {
			animationArray.Animations = value;
			InternalResource->Animations = animationArray.InternalTimelineArray;
			InternalResource->AnimationCount = (ushort) animationArray.Count;
		}
	}
}