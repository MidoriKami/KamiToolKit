using System.Collections.Generic;
using System.Numerics;

namespace KamiToolKit.NodeParts;

public class TimelineKeyFrameSet {
	
	private readonly List<TimelineKeyFrame> keyFrames = [];
	
	public static implicit operator List<TimelineKeyFrame>(TimelineKeyFrameSet frames) => frames.keyFrames;
	
	public int FrameIndex { get; set; }

	public Vector2 Position {
		set => keyFrames.Add(new TimelineKeyFrame { FrameIndex = FrameIndex, Position = value });
	}

	public byte Alpha {
		set => keyFrames.Add(new TimelineKeyFrame { FrameIndex = FrameIndex, Alpha = value });
	}

	public NodeTint NodeTint {
		set => keyFrames.Add(new TimelineKeyFrame { FrameIndex = FrameIndex, NodeTint = value });
	}

	public float Rotation {
		set => keyFrames.Add(new TimelineKeyFrame { FrameIndex = FrameIndex, Rotation = value });
	}

	public Vector2 Scale {
		set => keyFrames.Add(new TimelineKeyFrame { FrameIndex = FrameIndex, Scale = value });
	}

	public Vector3 TextColor {
		set => keyFrames.Add(new TimelineKeyFrame { FrameIndex = FrameIndex, TextColor = value });
	}

	public Vector3 TextEdgeColor {
		set => keyFrames.Add(new TimelineKeyFrame { FrameIndex = FrameIndex, TextEdgeColor = value });
	}
}