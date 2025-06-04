using System.Collections.Generic;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiToolKit.NodeParts;

public class TimelineKeyFrameSet {
	
	public readonly List<TimelineKeyFrame> KeyFrames = [];
	
	public int FrameIndex { get; set; }

	public Vector2 Position {
		set => KeyFrames.Add(new TimelineKeyFrame { FrameIndex = FrameIndex, Position = value });
	}

	public byte Alpha {
		set => KeyFrames.Add(new TimelineKeyFrame { FrameIndex = FrameIndex, Alpha = value });
	}

	public NodeTint NodeTint {
		set => KeyFrames.Add(new TimelineKeyFrame { FrameIndex = FrameIndex, NodeTint = value });
	}

	private Vector3? InternalAddColor { get; set; }

	public Vector3 AddColor {
		set {
			InternalAddColor = value;
			
			KeyFrames.RemoveAll(frame => frame.GroupType is AtkTimelineKeyGroupType.NodeTint);
			KeyFrames.Add(new TimelineKeyFrame {
				FrameIndex = FrameIndex, NodeTint = new NodeTint {
					AddColor = value,
					MultiplyColor = InternalMultiplyColor ?? Vector3.Zero,
				},
			});
		}
	}
	
	private Vector3? InternalMultiplyColor { get; set; }

	public Vector3 MultiplyColor {
		set {
			InternalMultiplyColor = value;
			
			KeyFrames.RemoveAll(frame => frame.GroupType is AtkTimelineKeyGroupType.NodeTint);
			KeyFrames.Add(new TimelineKeyFrame {
				FrameIndex = FrameIndex, NodeTint = new NodeTint {
					AddColor = InternalAddColor ?? Vector3.Zero,
					MultiplyColor = value,
				},
			});
		}
	}

	public uint PartId {
		set => KeyFrames.Add(new TimelineKeyFrame { FrameIndex = FrameIndex, PartId = value });
	}

	public float Rotation {
		set => KeyFrames.Add(new TimelineKeyFrame { FrameIndex = FrameIndex, Rotation = value });
	}

	public Vector2 Scale {
		set => KeyFrames.Add(new TimelineKeyFrame { FrameIndex = FrameIndex, Scale = value });
	}

	public Vector3 TextColor {
		set => KeyFrames.Add(new TimelineKeyFrame { FrameIndex = FrameIndex, TextColor = value });
	}

	public Vector3 TextEdgeColor {
		set => KeyFrames.Add(new TimelineKeyFrame { FrameIndex = FrameIndex, TextEdgeColor = value });
	}
}