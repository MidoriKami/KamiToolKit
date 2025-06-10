using System.Linq;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes.TimelineBuilding;
using KamiToolKit.NodeParts;
using KamiToolKit.Nodes.Image;

namespace KamiToolKit.Nodes.Icon;

public class AlternateCooldownNode : ResNode {

	protected readonly ImageNode CooldownImage;

	public AlternateCooldownNode() {
		CooldownImage = new ImageNode {
			NodeId = 15,
			Size = new Vector2(44.0f, 46.0f),
			Position = new Vector2(0.0f, 2.0f),
			Origin = new Vector2(22.0f, 23.0f),
			NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.Enabled | NodeFlags.EmitsEvents,
			WrapMode = 1,
			ImageNodeFlags = 0,
			DrawFlags = 0x102,
		};
		
		foreach (var yIndex in Enumerable.Range(0, 9))
		foreach (var xIndex in Enumerable.Range(0, 9)) {
			var coordinate = new Vector2(xIndex * 44.0f, yIndex * 48.0f);
			CooldownImage.AddPart(new Part {
				TexturePath = "ui/uld/IconA_Recast2.tex",
				TextureCoordinates = coordinate,
				Size = new Vector2(44.0f, 46.0f),
				Id = (uint) (xIndex + yIndex),
			});
		}
		
		foreach (var yIndex in Enumerable.Range(9, 9))
		foreach (var xIndex in Enumerable.Range(9, 9)) {
			var coordinate = new Vector2(xIndex * 44.0f, yIndex * 48.0f);
			CooldownImage.AddPart(new Part {
				TexturePath = "ui/uld/IconA_Recast2.tex",
				TextureCoordinates = coordinate,
				Size = new Vector2(44.0f, 46.0f),
				Id = (uint) (xIndex + yIndex),
			});
		}
		
		CooldownImage.AttachNode(this);

		BuildTimeline();
	}

	protected override void Dispose(bool disposing) {
		if (disposing) {
			CooldownImage.Dispose();
			
			base.Dispose(disposing);
		}
	}
	
	private void BuildTimeline() {
		CooldownImage.AddTimeline(new TimelineBuilder()
			.BeginFrameSet(11, 92)
			.AddFrame(11, alpha: 255, scale: new Vector2(1.0f), multiplyColor: new Vector3(100.0f), addColor: new Vector3(0.0f), partId: 1)
			.AddFrame(92, alpha: 255, scale: new Vector2(1.0f), multiplyColor: new Vector3(100.0f), addColor: new Vector3(0.0f), partId: 79)
			.EndFrameSet()
			.BeginFrameSet(93, 174)
			.AddFrame(93, alpha: 255, scale: new Vector2(1.0f), multiplyColor: new Vector3(100.0f), addColor: new Vector3(0.0f), partId: 82)
			.AddFrame(174, alpha: 255, scale: new Vector2(1.0f), multiplyColor: new Vector3(100.0f), addColor: new Vector3(0.0f), partId: 160)
			.EndFrameSet()
			.BeginFrameSet(175, 205)
			.AddFrame(175, alpha: 255, scale: new Vector2(1.0f), multiplyColor: new Vector3(100.0f), addColor: new Vector3(0.0f), partId: 80)
			.AddFrame(191, alpha: 255, scale: new Vector2(1.2f), multiplyColor: new Vector3(100.0f), addColor: new Vector3(200.0f), partId: 80)
			.AddFrame(205, alpha: 0, scale: new Vector2(1.25f), multiplyColor: new Vector3(100.0f), addColor: new Vector3(200.0f), partId: 80)
			.EndFrameSet()
			.Build());
	}
}