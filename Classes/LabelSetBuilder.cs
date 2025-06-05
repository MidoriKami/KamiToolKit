using System;
using System.Collections.Generic;
using System.Linq;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.NodeParts;

namespace KamiToolKit.Classes;

internal record LabelSet(int FrameId, int LabelId, AtkTimelineJumpBehavior JumpBehavior, int TargetLabel);

internal class LabelSetBuilder {
	internal readonly List<LabelSet> LabelSets = [];

	public void AddStartPlayOncePair(int labelId, Range range) {
		AddStartPlayOncePair(labelId, range.Start.Value, range.End.Value);
	}

	public void AddStartPlayOncePair(int labelId, int start, int end) {
		LabelSets.Add(new LabelSet(start, labelId, AtkTimelineJumpBehavior.Start, 0));
		LabelSets.Add(new LabelSet(end, labelId, AtkTimelineJumpBehavior.PlayOnce, 0));

	}

	public void AddLabelSet(int labelId, int frame, AtkTimelineJumpBehavior behavior, int targetId) {
		LabelSets.Add(new LabelSet(frame, labelId, behavior, targetId));

	}

	public TimelineLabelSet Build() {
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