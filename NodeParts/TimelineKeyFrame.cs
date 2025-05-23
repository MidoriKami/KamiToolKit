using System.Numerics;
using FFXIVClientStructs.FFXIV.Client.Graphics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using FFXIVClientStructs.STD;

namespace KamiToolKit.NodeParts;

public class TimelineKeyFrame {
	public KeyFrameGroupType Type { get; set; }
	
	public float SpeedStart { get; set; } = 0.0f;
	public float SpeedEnd { get; set; } = 1.0f;
	public required int FrameIndex { get; set; } = 0;
	public AtkTimelineInterpolation Interpolation { get; set; } = AtkTimelineInterpolation.Linear;
	public AtkTimelineKeyValue Value { get; set; }

	public Vector2 Position {
		get => new(Value.Float2.Item1, Value.Float2.Item2);
		set {
			Value = new AtkTimelineKeyValue {
				Float2 = new StdPair<float, float>(value.X, value.Y),
			};

			Type = KeyFrameGroupType.Position;
		}
	}

	public byte Alpha {
		get => Value.Byte;
		set {
			Value = new AtkTimelineKeyValue {
				Byte = value,
			};
			
			Type = KeyFrameGroupType.Alpha;
		}
	}

	public NodeTint NodeTint {
		get => Value.NodeTint;
		set {
			Value = new AtkTimelineKeyValue {
				NodeTint = value,
			};
			
			Type = KeyFrameGroupType.Tint;
		}
	}

	public float Rotation {
		get => Value.Float;
		set {
			Value = new AtkTimelineKeyValue {
				Float = value,
			};
			
			Type = KeyFrameGroupType.Rotation;
		}
	}

	public Vector2 Scale {
		get => new(Value.Float2.Item1, Value.Float2.Item2);
		set {
			Value = new AtkTimelineKeyValue {
				Float2 = new StdPair<float, float>(value.X, value.Y),
			};

			Type = KeyFrameGroupType.Scale;
		}
	}

	public Vector3 TextColor {
		get => new Vector3(Value.RGB.R, Value.RGB.G, Value.RGB.B) * 255.0f;
		set {
			Value = new AtkTimelineKeyValue {
				RGB = new ByteColor {
					R = (byte) ( value.X / 255.0f ),
					G = (byte) ( value.Y / 255.0f ),
					B = (byte) ( value.Z / 255.0f ),
				},
			};

			Type = KeyFrameGroupType.TextColor;
		}
	}
	
	public Vector3 TextEdgeColor {
		get => new Vector3(Value.RGB.R, Value.RGB.G, Value.RGB.B) * 255.0f;
		set {
			Value = new AtkTimelineKeyValue {
				RGB = new ByteColor {
					R = (byte) ( value.X / 255.0f ),
					G = (byte) ( value.Y / 255.0f ),
					B = (byte) ( value.Z / 255.0f ),
				},
			};

			Type = KeyFrameGroupType.TextEdge;
		}
	}
	
	public static implicit operator AtkTimelineKeyFrame(TimelineKeyFrame frame) => new() {
		Interpolation = frame.Interpolation,
		SpeedCoefficient1 = frame.SpeedStart,
		SpeedCoefficient2 = frame.SpeedEnd,
		FrameIdx = (ushort) frame.FrameIndex,
		Value = frame.Value,
	};
}