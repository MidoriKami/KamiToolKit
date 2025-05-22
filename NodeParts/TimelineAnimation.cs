using System;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit.NodeParts;

internal unsafe class TimelineAnimation : IDisposable {
	
	internal AtkTimelineAnimation* InternalTimeline;
	
	
	public TimelineAnimation() {
		InternalTimeline = NativeMemoryHelper.UiAlloc<AtkTimelineAnimation>();

		InternalTimeline->StartFrameIdx = 0;
		InternalTimeline->EndFrameIdx = 0;
	}
	
	public void Dispose() {
		NativeMemoryHelper.UiFree(InternalTimeline);
		InternalTimeline = null;
	}
}