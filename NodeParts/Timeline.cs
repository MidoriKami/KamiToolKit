using System;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit.NodeParts;

internal unsafe class Timeline : IDisposable {

	internal AtkTimeline* InternalTimeline;

	public Timeline() {
		InternalTimeline = NativeMemoryHelper.UiAlloc<AtkTimeline>();

		InternalTimeline->Resource = null; // todo: allocate this
		InternalTimeline->LabelResource = null; // this is null for buttons
		InternalTimeline->ActiveAnimation = null; // todo: assign this to first animation
		InternalTimeline->OwnerNode = null; // todo: attach to node
		InternalTimeline->FrameTime = 0.033333335f;
		InternalTimeline->ParentFrameTime = 0.033333334f;
		InternalTimeline->LabelFrameIdxDuration = 0;
		InternalTimeline->LabelEndFrameIdx = 1;
		InternalTimeline->ActiveLabelId = 0;
		InternalTimeline->Mask = AtkTimelineMask.VendorSpecific2;
		InternalTimeline->Flags = 0;
	}

	public void Dispose() {
		NativeMemoryHelper.UiFree(InternalTimeline);
		InternalTimeline = null;
	}

}