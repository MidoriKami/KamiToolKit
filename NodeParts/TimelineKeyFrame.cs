using System.Numerics;
using FFXIVClientStructs.FFXIV.Client.Graphics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using FFXIVClientStructs.STD;

namespace KamiToolKit.NodeParts;

public class TimelineKeyFrame {
	public KeyFrameGroupType GroupSelector { get; set; }
	
	public AtkTimelineKeyGroupType GroupType { get; set; }
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

			GroupSelector = KeyFrameGroupType.Position;
			GroupType = AtkTimelineKeyGroupType.Float2;
		}
	}

	public byte Alpha {
		get => Value.Byte;
		set {
			Value = new AtkTimelineKeyValue {
				Byte = value,
			};
			
			GroupType = AtkTimelineKeyGroupType.Byte;
			GroupSelector = KeyFrameGroupType.Alpha;
		}
	}

	public NodeTint NodeTint {
		get => Value.NodeTint;
		set {
			Value = new AtkTimelineKeyValue {
				NodeTint = value,
			};
			
			GroupType = AtkTimelineKeyGroupType.NodeTint;
			GroupSelector = KeyFrameGroupType.Tint;
		}
	}

	public float Rotation {
		get => Value.Float;
		set {
			Value = new AtkTimelineKeyValue {
				Float = value,
			};
			
			GroupType = AtkTimelineKeyGroupType.Float;
			GroupSelector = KeyFrameGroupType.Rotation;
		}
	}

	public Vector2 Scale {
		get => new(Value.Float2.Item1, Value.Float2.Item2);
		set {
			Value = new AtkTimelineKeyValue {
				Float2 = new StdPair<float, float>(value.X, value.Y),
			};

			GroupType = AtkTimelineKeyGroupType.Float;
			GroupSelector = KeyFrameGroupType.Scale;
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

			GroupType = AtkTimelineKeyGroupType.RGB;
			GroupSelector = KeyFrameGroupType.TextColor;
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

			GroupType = AtkTimelineKeyGroupType.RGB;
			GroupSelector = KeyFrameGroupType.TextEdge;
		}
	}

	public AtkTimelineLabel Label {
		get => Value.Label;
		set {
			Value = new AtkTimelineKeyValue {
				Label = value,
			};

			GroupType = AtkTimelineKeyGroupType.Label;
			SpeedEnd = 0.0f;
			Interpolation = AtkTimelineInterpolation.None;
			GroupSelector = KeyFrameGroupType.TextLabel;
		}
	}

	public uint PartId {
		set {
			Value = new AtkTimelineKeyValue {
				UShort = (ushort) value,
			};
			
			GroupType = AtkTimelineKeyGroupType.UShort;
			GroupSelector = KeyFrameGroupType.PartId;
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