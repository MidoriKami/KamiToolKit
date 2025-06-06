using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes.TimelineBuilding;

namespace KamiToolKit.Nodes.InputText;

public class CursorNode : ResNode {

	private readonly SimpleImageNode cursorImageNode;
	
	public CursorNode() {
		cursorImageNode = new SimpleImageNode {
			TexturePath = "ui/uld/TextInputA.tex", 
			Size = new Vector2(4.0f, 24.0f), 
			TextureCoordinates = new Vector2(68.0f, 0.0f),
			TextureSize = new Vector2(4.0f, 24.0f),
			IsVisible = true,
			WrapMode = 1,
			ImageNodeFlags = 0x0,
		};
		
		cursorImageNode.AttachNode(this);
		
		AddTimeline(new TimelineBuilder()
			.BeginFrameSet(1, 15)
			.AddLabel(1, 101, AtkTimelineJumpBehavior.Start, 0)
			.AddLabel(15, 0, AtkTimelineJumpBehavior.LoopForever, 101)
			.EndFrameSet()
			.Build());
		
		cursorImageNode.AddTimeline(new TimelineBuilder()
			.BeginFrameSet(1, 8)
			.AddEmptyFrame(1)
			.EndFrameSet()
			.Build());

		Timeline?.StartAnimation(101);
	}

	protected override void Dispose(bool disposing) {
		if (disposing) {
			cursorImageNode.Dispose();
			
			base.Dispose(disposing);
		}
	}
}