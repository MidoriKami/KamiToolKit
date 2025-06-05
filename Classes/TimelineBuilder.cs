using System;
using System.Linq;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.NodeParts;

namespace KamiToolKit.Classes;

public class TimelineBuilder {

	private AtkTimelineMask timelineMask = AtkTimelineMask.VendorSpecific2;

	private LabelSetBuilder labelSetBuilder = new();
	private AnimationBuilder animationBuilder = new();

	public TimelineBuilder SetMask(AtkTimelineMask mask) {
		timelineMask = mask;
		return this;
	}

	public TimelineBuilder SetMask(byte mask) {
		timelineMask = (AtkTimelineMask) mask;
		return this;
	}

	public TimelineBuilder AddLabelSetPair(int labelId, Range range) {
		labelSetBuilder.AddStartPlayOncePair(labelId, range);
		return this;
	}

	public TimelineBuilder AddLabelSetPair(int labelId, int start, int end) {
		labelSetBuilder.AddStartPlayOncePair(labelId, start, end);
		return this;
	}

	public TimelineBuilder AddLabelSet(int labelId, int frame, AtkTimelineJumpBehavior jumpBehavior, int targetLabel) {
		labelSetBuilder.AddLabelSet(labelId, frame, jumpBehavior, targetLabel);
		return this;
	}

	public TimelineBuilder AddAnimation(int frame, TimelineKeyFrameSet animation) {
		animationBuilder.AddAnimation(frame, animation);
		return this;
	}

	public TimelineBuilder ResetAnimations() {
		animationBuilder = new AnimationBuilder();
		return this;
	}

	public TimelineBuilder ResetLabelSet() {
		labelSetBuilder = new LabelSetBuilder();
		return this;
	}

	// Set ResetAnimations to true if you need to reuse the existing label sets for another animation
	public Timeline BuildAnimations(bool resetAnimations = false) {
		var builtTimeline =  new Timeline {
			Mask = timelineMask, 
			LabelEndFrameIdx = labelSetBuilder.LabelSets.Last().FrameId, 
			LabelFrameIdxDuration = labelSetBuilder.LabelSets.Last().FrameId - 1, 
			Animations = animationBuilder.Build(labelSetBuilder),
		};

		if (resetAnimations) {
			ResetAnimations();
		}

		return builtTimeline;
	}

	public Timeline BuildLabelSets() => new() {
		Mask = timelineMask, 
		LabelEndFrameIdx = labelSetBuilder.LabelSets.Last().FrameId, 
		LabelFrameIdxDuration = labelSetBuilder.LabelSets.Last().FrameId - 1,
		LabelSets = [ labelSetBuilder.Build() ],
	};

	public Timeline BuildEverything() => new() {
		Mask = timelineMask, 
		LabelEndFrameIdx = labelSetBuilder.LabelSets.Last().FrameId, 
		LabelFrameIdxDuration = labelSetBuilder.LabelSets.Last().FrameId - 1,
		Animations = animationBuilder.Build(labelSetBuilder),
		LabelSets = [ labelSetBuilder.Build() ],
	};
}