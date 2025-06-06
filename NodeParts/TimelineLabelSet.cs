﻿using System;
using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit.NodeParts;

public unsafe class TimelineLabelSet : IDisposable {

	internal AtkTimelineLabelSet* InternalLabelSet;
	
	private List<TimelineKeyFrame> internalKeyFrames = [];
	
	public TimelineLabelSet() {
		InternalLabelSet = NativeMemoryHelper.UiAlloc<AtkTimelineLabelSet>();

		InternalLabelSet->StartFrameIdx = 0;
		InternalLabelSet->EndFrameIdx = 0;
		InternalLabelSet->LabelKeyGroup.Type = AtkTimelineKeyGroupType.Label;
	}
	
	public void Dispose() {
		NativeMemoryHelper.UiFree(InternalLabelSet);
		InternalLabelSet = null;
	}

	public int StartFrameId {
		get => InternalLabelSet->StartFrameIdx;
		set => InternalLabelSet->StartFrameIdx = (ushort) value;
	}

	public int EndFrameId {
		get => InternalLabelSet->EndFrameIdx;
		set => InternalLabelSet->EndFrameIdx = (ushort) value;
	}

	private void Resync() {
		ref var keyGroup = ref InternalLabelSet->LabelKeyGroup;
		
		// Free existing array, we will completely rebuild it
		if (keyGroup.KeyFrames is null) {
			NativeMemoryHelper.UiFree(keyGroup.KeyFrames, keyGroup.KeyFrameCount);
			keyGroup.KeyFrames = null;
		}

		// Allocate new array
		keyGroup.KeyFrames = NativeMemoryHelper.UiAlloc<AtkTimelineKeyFrame>(internalKeyFrames.Count);

		var index = 0;
		foreach (var keyFrame in internalKeyFrames) {
			keyGroup.KeyFrames[index] = keyFrame;
			index++;
		}
		
		keyGroup.KeyFrameCount = (ushort) internalKeyFrames.Count;
	}

	public List<TimelineKeyFrame> Labels {
		get => internalKeyFrames;
		set {
			internalKeyFrames = value;
			Resync();
		}
	}
}