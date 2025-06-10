using System.Linq;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes.TimelineBuilding;
using KamiToolKit.NodeParts;
using KamiToolKit.Nodes.Image;

namespace KamiToolKit.Nodes.Icon;

public class AntsNode : ResNode {

	protected readonly ImageNode AntsImageNode;

	public AntsNode() {
		AntsImageNode = new ImageNode {
			NodeId = 13,
			Size = new Vector2(48, 48),
			NodeFlags = NodeFlags.Enabled | NodeFlags.EmitsEvents,
			WrapMode = 1,
			ImageNodeFlags = 0,
			DrawFlags = 0x02,
		};

		var index = 6;
		
		foreach(var yIndex in Enumerable.Range(0, 3))
		foreach (var xIndex in Enumerable.Range(2, 3)) {
			var coordinate = new Vector2(xIndex * 48.0f, yIndex * 48.0f);

			if (index == 14) break;
			
			AntsImageNode.AddPart(new Part {
				TexturePath = "ui/uld/IconA_Frame.tex",
				TextureCoordinates = coordinate,
				Size = new Vector2(48.0f, 48.0f),
				Id = (uint) index++,
			});
		}
		
		AntsImageNode.AttachNode(this);

		BuildTimeline();
	}

	protected override void Dispose(bool disposing) {
		if (disposing) {
			AntsImageNode.Dispose();
			
			base.Dispose(disposing);
		}
	}
	
	private void BuildTimeline() {
		AntsImageNode.AddTimeline(new TimelineBuilder()
			.BeginFrameSet(2, 9)
			.AddFrame(2, partId: 6)
			.AddFrame(9, partId: 13)
			.EndFrameSet()
			.Build());
	}
}