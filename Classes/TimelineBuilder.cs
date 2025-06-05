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

		foreach(var index in Enumerable.Range(0, labelSetBuilder.LabelSets.Count / 2)) {
			var startLabel = labelSetBuilder.LabelSets.ElementAt(index);
			var endLabel = labelSetBuilder.LabelSets.ElementAt(index + 1);

			var keyFramesForFrameSet = new List<TimelineKeyFrame>();
			
			foreach (var animation in animations) {
				
				// If this animation has frames within this set
				if (animation.Frame >= startLabel.FrameId && animation.Frame <= endLabel.FrameId) {
					
					// Add each keyframe for that animation
					keyFramesForFrameSet.AddRange(animation.Data.KeyFrames);
				}
			}
			
			builtAnimations.Add(new TimelineAnimation {
				StartFrameId = startLabel.FrameId,
				EndFrameId = endLabel.FrameId,
				KeyFrames = keyFramesForFrameSet,
			});
		}
		
		return builtAnimations;
	}
}