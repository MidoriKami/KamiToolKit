using System.Linq;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes.TimelineBuilding;
using KamiToolKit.NodeParts;
using KamiToolKit.Nodes.Image;

namespace KamiToolKit.Nodes.Icon;

public class CooldownNode : ResNode {

	protected readonly ImageNode GlossyImageFrame;
	protected readonly ImageNode CooldownImage;
	
	public CooldownNode() {
		GlossyImageFrame = new ImageNode {
			NodeId = 18,
			Size = new Vector2(48.0f, 48.0f),
			NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.EmitsEvents,
			WrapMode = 1,
			DrawFlags = 0,
			ImageNodeFlags = 0,
		};

        IconNodeTextureHelper.LoadIconAFrameTexture(GlossyImageFrame);

		GlossyImageFrame.AttachNode(this);

		CooldownImage = new ImageNode {
			NodeId = 17,
			Size = new Vector2(44.0f, 46.0f),
			Position = new Vector2(2.0f, 2.0f),
			NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.Enabled | NodeFlags.EmitsEvents,
			WrapMode = 1,
			ImageNodeFlags = 0,
			DrawFlags = 0x02,
		};

		foreach (var yIndex in Enumerable.Range(0, 9))
		foreach (var xIndex in Enumerable.Range(0, 9)) {
			var coordinate = new Vector2(xIndex * 44.0f, yIndex * 48.0f);
			CooldownImage.AddPart(new Part {
				TexturePath = "ui/uld/IconA_Recast.tex",
				TextureCoordinates = coordinate,
				Size = new Vector2(44.0f, 46.0f),
				Id = (uint) (xIndex + yIndex),
			});
		}
		
		CooldownImage.AttachNode(this);

		BuildTimelines();
	}

	protected override void Dispose(bool disposing) {
		if (disposing) {
			GlossyImageFrame.Dispose();
			CooldownImage.Dispose();
			
			base.Dispose(disposing);
		}
	}
	
	private void BuildTimelines() {
		GlossyImageFrame.AddTimeline(new TimelineBuilder()
			.AddFrameSetWithFrame(1, 10, 1, partId: 0)
			.AddFrameSetWithFrame(11, 20, 11, partId: 1)
			.AddFrameSetWithFrame(21, 30, 21, partId: 2)
			.AddFrameSetWithFrame(31, 40, 31, partId: 3)
			.AddFrameSetWithFrame(41, 50, 41, partId: 18)
			.AddFrameSetWithFrame(51, 60, 51, partId: 19)
			.AddFrameSetWithFrame(143, 165, 143, partId: 0)
			.Build());
		
		CooldownImage.AddTimeline(new TimelineBuilder()
			.BeginFrameSet(61, 142)
			.AddFrame(61, alpha: 255, partId: 1)
			.AddFrame(142, alpha: 255, partId: 79)
			.EndFrameSet()
			.BeginFrameSet(143, 165)
			.AddFrame(143, alpha: 255, partId: 80)
			.AddFrame(165, alpha: 0, partId: 79)
			.EndFrameSet()
			.Build());
	}
}