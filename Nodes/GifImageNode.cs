using System;
using System.IO;
using System.Threading.Tasks;
using Dalamud.Interface.Textures;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.NodeParts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace KamiToolKit.Nodes;

public class GifImageNode : ResNode {
	
	private ImageNode imageNode;

	public required string FilePath {
		set {
			Task.Run(() => LoadFrames(value));
		}
	}

	public GifImageNode() {
		imageNode = new ImageNode {
			IsVisible = true,
		};
		
		imageNode.AttachNode(this);
	}

	protected override void Dispose(bool disposing) {
		if (disposing) {
			imageNode.Dispose();

			base.Dispose(disposing);
		}
	}
	
	public override float Width {
		get => base.Width;
		set {
			imageNode.Width = value;
			base.Width = value;
		}
	}

	public override float Height {
		get => base.Height;
		set {
			imageNode.Height = value;
			base.Height = value;
		}
	}

	private async void LoadFrames(string filepath) {
		try {
			var image = await LoadAsync(filepath);
			if (image.Length <= 0) return;
		
			using var memoryStream = new MemoryStream(image);
			using var processedImage = Image.Load<Rgba32>(memoryStream);
			if (processedImage.Frames.Count is 0) return;

			var labelSetBuilder = new LabelSetBuilder().AddLabelSet(200, 1, AtkTimelineJumpBehavior.Start, 0);
			var timelineBuilder = new TimelineBuilder(labelSetBuilder);

			uint currentPartId = 0;

			var frameDelay = processedImage.Frames.RootFrame.Metadata.GetGifMetadata().FrameDelay / 3.33333333f;
			
			foreach (var frame in processedImage.Frames) {
				var buffer = new byte[8 * frame.Width * frame.Height];
			
				frame.CopyPixelDataTo(buffer);
			
				var texture = await DalamudInterface.Instance.TextureProvider.CreateFromRawAsync(RawImageSpecification.Rgba32(frame.Width, frame.Height), buffer);
				timelineBuilder.AddFrame((int)(currentPartId * frameDelay ), new TimelineKeyFrameSet {
					PartId = currentPartId,
				});
				
				var texturePart = new Part {
					Size = texture.Size,
					Id = currentPartId++,
				};
				
				texturePart.LoadTexture(texture);
				imageNode.AddPart(texturePart);
			}
			
			timelineBuilder.AddFrame((int)(currentPartId * frameDelay ), new TimelineKeyFrameSet {
				PartId = currentPartId,
			});
			
			labelSetBuilder.AddLabelSet(0, (int)(processedImage.Frames.Count * frameDelay ), AtkTimelineJumpBehavior.LoopForever, 200);
			
			AddTimeline(labelSetBuilder.Build());
			
			imageNode.AddTimeline(timelineBuilder.Build());

			unsafe {
				InternalResNode->Timeline->PlayAnimation(AtkTimelineJumpBehavior.LoopForever, 200);
			}
		}
		catch (Exception e) {
			Log.Exception(e);
		}
	}

	private static async Task<byte[]> LoadAsync(string path) {
		byte[] data = [];
		
		if (File.Exists(path)) {
			data = await File.ReadAllBytesAsync(path);
		}
		
		return data;
	}
}