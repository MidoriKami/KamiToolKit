using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes.TimelineBuilding;
using KamiToolKit.NodeParts;
using KamiToolKit.Nodes.Image;

namespace KamiToolKit.Nodes.Icon;

public class IconIndicator : ResNode {

	internal readonly ImageNode IconNode;

	public IconIndicator(uint innerNodeId) {
		IconNode = new ImageNode {
            NodeId = innerNodeId,
            Size = new Vector2(18, 18),
            NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.Enabled | NodeFlags.EmitsEvents,
            WrapMode = 2,
            ImageNodeFlags = 0,
            DrawFlags = 0x02,
		};
        
        LoadPartsList(IconNode);
        
        IconNode.AttachNode(this);

        BuildTimeline();
    }

    protected override void Dispose(bool disposing) {
        if (disposing) {
            IconNode.Dispose();
            
            base.Dispose(disposing);
        }
    }

    private void BuildTimeline() {
        IconNode.AddTimeline(new TimelineBuilder()
            .AddFrameSetWithFrame(11, 20, 11, partId: 14)
            .AddFrameSetWithFrame(21, 30, 21, partId: 15)
            .AddFrameSetWithFrame(31, 40, 31, partId: 21)
            .AddFrameSetWithFrame(41, 50, 41, partId: 22)
            .AddFrameSetWithFrame(51, 60, 51, partId: 23)
            .AddFrameSetWithFrame(61, 70, 61, partId: 24)
            .AddFrameSetWithFrame(71, 79, 71, partId: 29)
            .AddFrameSetWithFrame(80, 89, 80, partId: 30)
            .AddFrameSetWithFrame(90, 99, 90, partId: 25)
            .AddFrameSetWithFrame(100, 109, 100, partId: 26)
            .AddFrameSetWithFrame(110, 119, 110, partId: 27)
            .AddFrameSetWithFrame(120, 129, 120, partId: 28)
            .Build());
    }

    private void LoadPartsList(ImageNode image) {
        image.AddPart(new Part {
            Id = 0, TexturePath = "ui/uld/IconA_Frame.tex", Size = new Vector2(48.0f, 48.0f),
        });
        image.AddPart(new Part {
            Id = 1, TexturePath = "ui/uld/IconA_Frame.tex", Size = new Vector2(48.0f, 48.0f), TextureCoordinates = new Vector2(48.0f, 0.0f),
        });
        image.AddPart(new Part {
            Id = 2, TexturePath = "ui/uld/IconA_Frame.tex", Size = new Vector2(48.0f, 48.0f), TextureCoordinates = new Vector2(0.0f, 48.0f),
        });
        image.AddPart(new Part {
            Id = 3, TexturePath = "ui/uld/IconA_Frame.tex", Size = new Vector2(48.0f, 48.0f), TextureCoordinates = new Vector2(48.0f, 48.0f),
        });
        image.AddPart(new Part {
            Id = 4, TexturePath = "ui/uld/IconA_Frame.tex", Size = new Vector2(48.0f, 48.0f), TextureCoordinates = new Vector2(0.0f, 96.0f),
        });
        image.AddPart(new Part {
            Id = 5, TexturePath = "ui/uld/IconA_Frame.tex", Size = new Vector2(48.0f, 48.0f), TextureCoordinates = new Vector2(48.0f, 96.0f),
        });
        image.AddPart(new Part {
            Id = 6, TexturePath = "ui/uld/IconA_Frame.tex", Size = new Vector2(48.0f, 48.0f), TextureCoordinates = new Vector2(96.0f, 0.0f),
        });
        image.AddPart(new Part {
            Id = 7, TexturePath = "ui/uld/IconA_Frame.tex", Size = new Vector2(48.0f, 48.0f), TextureCoordinates = new Vector2(144.0f, 0.0f),
        });
        image.AddPart(new Part {
            Id = 8, TexturePath = "ui/uld/IconA_Frame.tex", Size = new Vector2(48.0f, 48.0f), TextureCoordinates = new Vector2(192.0f, 0.0f),
        });
        image.AddPart(new Part {
            Id = 9, TexturePath = "ui/uld/IconA_Frame.tex", Size = new Vector2(48.0f, 48.0f), TextureCoordinates = new Vector2(96.0f, 48.0f),
        });
        image.AddPart(new Part {
            Id = 10, TexturePath = "ui/uld/IconA_Frame.tex", Size = new Vector2(48.0f, 48.0f), TextureCoordinates = new Vector2(144.0f, 48.0f),
        });
        image.AddPart(new Part {
            Id = 11, TexturePath = "ui/uld/IconA_Frame.tex", Size = new Vector2(48.0f, 48.0f), TextureCoordinates = new Vector2(192.0f, 48.0f),
        });
        image.AddPart(new Part {
            Id = 12, TexturePath = "ui/uld/IconA_Frame.tex", Size = new Vector2(48.0f, 48.0f), TextureCoordinates = new Vector2(96.0f, 96.0f),
        });
        image.AddPart(new Part {
            Id = 13, TexturePath = "ui/uld/IconA_Frame.tex", Size = new Vector2(48.0f, 48.0f), TextureCoordinates = new Vector2(144.0f, 96.0f),
        });
        image.AddPart(new Part {
            Id = 14, TexturePath = "ui/uld/IconA_Frame.tex", Size = new Vector2(18.0f, 18.0f), TextureCoordinates = new Vector2(192.0f, 96.0f),
        });
        image.AddPart(new Part {
            Id = 15, TexturePath = "ui/uld/IconA_Frame.tex", Size = new Vector2(18.0f, 18.0f), TextureCoordinates = new Vector2(192.0f, 114.0f),
        });
        image.AddPart(new Part {
            Id = 16, TexturePath = "ui/uld/IconA_Frame.tex", Size = new Vector2(72.0f, 72.0f), TextureCoordinates = new Vector2(240.0f, 0.0f),
        });
        image.AddPart(new Part {
            Id = 17, TexturePath = "ui/uld/IconA_Frame.tex", Size = new Vector2(64.0f, 64.0f), TextureCoordinates = new Vector2(240.0f, 72.0f),
        });
        image.AddPart(new Part {
            Id = 18, TexturePath = "ui/uld/IconA_Frame.tex", Size = new Vector2(48.0f, 48.0f), TextureCoordinates = new Vector2(312.0f, 0.0f),
        });
        image.AddPart(new Part {
            Id = 19, TexturePath = "ui/uld/IconA_Frame.tex", Size = new Vector2(48.0f, 48.0f), TextureCoordinates = new Vector2(312.0f, 48.0f),
        });
        image.AddPart(new Part {
            Id = 20, TexturePath = "ui/uld/IconA_Frame.tex", Size = new Vector2(48.0f, 48.0f), TextureCoordinates = new Vector2(312.0f, 96.0f),
        });
        image.AddPart(new Part {
            Id = 21, TexturePath = "ui/uld/IconA_Frame.tex", Size = new Vector2(18.0f, 18.0f), TextureCoordinates = new Vector2(210.0f, 114.0f),
        });
        image.AddPart(new Part {
            Id = 22, TexturePath = "ui/uld/IconA_Frame.tex", Size = new Vector2(18.0f, 18.0f), TextureCoordinates = new Vector2(360.0f, 96.0f),
        });
        image.AddPart(new Part {
            Id = 23, TexturePath = "ui/uld/IconA_Frame.tex", Size = new Vector2(18.0f, 18.0f), TextureCoordinates = new Vector2(378.0f, 96.0f),
        });
        image.AddPart(new Part {
            Id = 24, TexturePath = "ui/uld/IconA_Frame.tex", Size = new Vector2(18.0f, 18.0f), TextureCoordinates = new Vector2(360.0f, 114.0f),
        });
        image.AddPart(new Part {
            Id = 25, TexturePath = "ui/uld/IconA_Frame.tex", Size = new Vector2(18.0f, 18.0f), TextureCoordinates = new Vector2(210.0f, 96.0f),
        });
        image.AddPart(new Part {
            Id = 26, TexturePath = "ui/uld/IconA_Frame.tex", Size = new Vector2(18.0f, 18.0f), TextureCoordinates = new Vector2(408.0f, 0.0f),
        });
        image.AddPart(new Part {
            Id = 27, TexturePath = "ui/uld/IconA_Frame.tex", Size = new Vector2(18.0f, 18.0f), TextureCoordinates = new Vector2(408.0f, 18.0f),
        });
        image.AddPart(new Part {
            Id = 28, TexturePath = "ui/uld/IconA_Frame.tex", Size = new Vector2(18.0f, 18.0f), TextureCoordinates = new Vector2(408.0f, 36.0f),
        });
        image.AddPart(new Part {
            Id = 29, TexturePath = "ui/uld/IconA_Frame.tex", Size = new Vector2(18.0f, 18.0f), TextureCoordinates = new Vector2(396.0f, 96.0f),
        });
        image.AddPart(new Part {
            Id = 30, TexturePath = "ui/uld/IconA_Frame.tex", Size = new Vector2(18.0f, 18.0f), TextureCoordinates = new Vector2(396.0f, 114.0f),
        });
	}
}