using System;
using System.Collections.Generic;
using System.Linq;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.NodeParts;

namespace KamiToolKit.Classes;

internal record LabelSet(int LabelId, int FrameStart, int FrameEnd);

public class LabelSetBuilder {
	private AtkTimelineMask timelineMask = (AtkTimelineMask) 0xFF;

	internal readonly List<LabelSet> LabelSets = [];

	public LabelSetBuilder SetMask(byte mask) {
		timelineMask = (AtkTimelineMask) mask;

		return this;
	}

	public LabelSetBuilder SetMask(AtkTimelineMask mask) {
		timelineMask = mask;
		return this;
	}

	public LabelSetBuilder AddLabelSet(int labelId, int start, int end) {
		LabelSets.Add(new LabelSet(labelId, start, end));
		
		return this;
	}
	
	public LabelSetBuilder AddLabelSet(int labelId, Range range) {
		LabelSets.Add(new LabelSet(labelId, range.Start.Value, range.End.Value));
		
		return this;
	}

	public Timeline Build() => new() {
		Mask = timelineMask,
		LabelEndFrameIdx = LabelSets.Max(set => set.FrameEnd),
		LabelFrameIdxDuration = LabelSets.Max(set => set.FrameEnd) - 1,
		LabelSets = [ BuildLabelSet() ],
	};

	private TimelineLabelSet BuildLabelSet() {
		var labelFrames = new List<TimelineKeyFrame>();
		
		foreach (var set in LabelSets) {
			labelFrames.Add(new TimelineLabelFrame {
				FrameIndex = set.FrameStart,
				LabelId = set.LabelId,
			});
			
			labelFrames.Add(new TimelineLabelFrame {
				FrameIndex = set.FrameEnd,
				LabelId = 0,
				JumpBehavior = AtkTimelineJumpBehavior.PlayOnce,
			});
		}

		return new TimelineLabelSet {
			StartFrameId = 1,
			EndFrameId = LabelSets.Max(labelSet => labelSet.FrameEnd),
			Labels = labelFrames,
		};
	}
}