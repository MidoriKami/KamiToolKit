using System;
using System.Collections.Generic;
using System.Linq;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.NodeParts;

namespace KamiToolKit.Classes;

internal record LabelSet(int FrameId, int LabelId, AtkTimelineJumpBehavior JumpBehavior, int TargetLabel);

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

	public LabelSetBuilder AddStartPlayOncePair(int labelId, int start, int end) {
		LabelSets.Add(new LabelSet(start, labelId, AtkTimelineJumpBehavior.Start, 0));
		LabelSets.Add(new LabelSet(end, labelId, AtkTimelineJumpBehavior.PlayOnce, 0));
		
		return this;
	}

	public LabelSetBuilder AddLabelSet(int labelId, int frame, AtkTimelineJumpBehavior behavior, int targetId) {
		LabelSets.Add(new LabelSet(frame, labelId, behavior, targetId));
		
		return this;
	}

	public LabelSetBuilder AddStartPlayOncePair(int labelId, Range range)
		=> AddStartPlayOncePair(labelId, range.Start.Value, range.End.Value);

	public Timeline Build() => new() {
		Mask = timelineMask,
		LabelEndFrameIdx = LabelSets.Max(set => set.FrameId),
		LabelFrameIdxDuration = LabelSets.Max(set => set.FrameId) - 1,
		LabelSets = [ BuildLabelSet() ],
	};

	private TimelineLabelSet BuildLabelSet() {
		var labelFrames = new List<TimelineKeyFrame>();
		
		foreach (var set in LabelSets) {
			labelFrames.Add(new TimelineLabelFrame {
				FrameIndex = set.FrameId,
				LabelId = set.LabelId,
				JumpBehavior = set.JumpBehavior,
				JumpLabelId = set.TargetLabel,
			});
		}

		return new TimelineLabelSet {
			StartFrameId = 1,
			EndFrameId = LabelSets.Max(labelSet => labelSet.FrameId),
			Labels = labelFrames,
		};
	}
}