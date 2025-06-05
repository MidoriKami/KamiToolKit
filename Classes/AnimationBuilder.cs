using System.Collections.Generic;
using System.Linq;
using KamiToolKit.NodeParts;

namespace KamiToolKit.Classes;

internal record Animation(int Frame, TimelineKeyFrameSet Data);

internal class AnimationBuilder {
	private readonly List<Animation> animations = [];

	public void AddAnimation(int frame, TimelineKeyFrameSet animation) {
		animation.FrameIndex = frame;

		foreach (var keyFrame in animation.KeyFrames) {
			keyFrame.FrameIndex = frame;
		}
		
		animations.Add(new Animation(frame, animation));

	}
	
	public List<TimelineAnimation> Build(LabelSetBuilder labelSetBuilder) {
		var builtAnimations = new List<TimelineAnimation>();

		foreach(var index in Enumerable.Range(0, labelSetBuilder.LabelSets.Count / 2)) {
			var startLabel = labelSetBuilder.LabelSets.ElementAt(index * 2);
			var endLabel = labelSetBuilder.LabelSets.ElementAt(index * 2 + 1);

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