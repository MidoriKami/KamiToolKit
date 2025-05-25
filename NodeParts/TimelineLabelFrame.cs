using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiToolKit.NodeParts;

public class TimelineLabelFrame {
	public required int FrameIndex { get; set; }

	private AtkTimelineLabel data;
	
	public AtkTimelineJumpBehavior JumpBehavior {
		get => data.JumpBehavior;
		set => data.JumpBehavior = value;
	}

	public int LabelId {
		get => data.LabelId;
		set => data.LabelId = (ushort) value;
	}

	public int JumpLabelId {
		get => data.JumpLabelId;
		set => data.JumpLabelId = (byte) value;
	}

	public static implicit operator TimelineKeyFrame(TimelineLabelFrame frame) => new() {
		FrameIndex = frame.FrameIndex,
		Label = frame.data,
	};
}