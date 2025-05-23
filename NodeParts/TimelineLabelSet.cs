using System;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit.NodeParts;

public unsafe class TimelineLabelSet : IDisposable {

	internal AtkTimelineLabelSet* InternalLabelSet;

	public TimelineLabelSet() {
		InternalLabelSet = NativeMemoryHelper.UiAlloc<AtkTimelineLabelSet>();
		
		// Currently have nothing to base the initialization logic on.
	}
	
	public void Dispose() {
		NativeMemoryHelper.UiFree(InternalLabelSet);
		InternalLabelSet = null;
	}
}