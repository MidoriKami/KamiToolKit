using System.Collections.Generic;
using System.Linq;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.NodeParts;

namespace KamiToolKit.Classes;

internal record Animation(int Frame, TimelineKeyFrameSet Data);

public class TimelineBuilder(LabelSetBuilder labelSetBuilder) {

	private AtkTimelineMask timelineMask = AtkTimelineMask.VendorSpecific2;

	private readonly List<Animation> animations = [];
	
	public TimelineBuilder SetMask(AtkTimelineMask mask) {
		timelineMask = mask;
		return this;
	}

	public TimelineBuilder SetMask(byte mask) {
		timelineMask = (AtkTimelineMask) mask;
		return this;
	}

	public TimelineBuilder AddFrame(int frame, TimelineKeyFrameSet data) {
		data.FrameIndex = frame;

		foreach (var keyFrame in data.KeyFrames) {
			keyFrame.FrameIndex = frame;
		}
		
		animations.Add(new Animation(frame, data));
		return this;
	}

	public Timeline Build() => new() {
		Mask = timelineMask,
		Animations = BuildAnimations(),
	};

	private List<TimelineAnimation> BuildAnimations() {
		var builtAnimations = new List<TimelineAnimation>();
		
		foreach (var labelSet in labelSetBuilder.LabelSets) {
			var keyFrames = BuildKeyFramesForLabelSet(labelSet);
			if (keyFrames.Count == 0) continue;
			
			builtAnimations.Add(new TimelineAnimation {
				StartFrameId = labelSet.FrameStart,
				EndFrameId = labelSet.FrameEnd,
				KeyFrames = BuildKeyFramesForLabelSet(labelSet),
			});
		}
		
		return builtAnimations;
	}

	private List<TimelineKeyFrame> BuildKeyFramesForLabelSet(LabelSet set) => animations
		.Where(animation => animation.Frame >= set.FrameStart && animation.Frame <= set.FrameEnd)
		.SelectMany((animation, _) => animation.Data.KeyFrames)
		.ToList();
}