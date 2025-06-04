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

			var labelSetBuilder = new LabelSetBuilder()
				.AddLabelSet(200, 1, processedImage.Frames.Count * 3);
			
			var timelineBuilder = new TimelineBuilder(labelSetBuilder);

			AddTimeline(new Timeline {
				Mask = (AtkTimelineMask) 0xFF,
				LabelEndFrameIdx = processedImage.Frames.Count * 3,
				LabelFrameIdxDuration = processedImage.Frames.Count * 3 - 1,
				LabelSets = [
					new TimelineLabelSet {
						StartFrameId = 1, EndFrameId = processedImage.Frames.Count * 3, Labels = [
							new TimelineLabelFrame { FrameIndex = 1,  LabelId = 200, JumpBehavior = AtkTimelineJumpBehavior.Start },
							new TimelineLabelFrame { FrameIndex = 138,  LabelId = 0, JumpBehavior = AtkTimelineJumpBehavior.LoopForever, JumpLabelId = 200},
						],
					},
				],
			});
			
			uint currentPartId = 0;
			
			foreach (var frame in processedImage.Frames) {
				// var delay = frame.Metadata.GetGifMetadata().FrameDelay / 100.0f;
				// if(delay < 0.02f) delay = 0.1f;
			
				var buffer = new byte[8 * frame.Width * frame.Height];
			
				frame.CopyPixelDataTo(buffer);
			
				var texture = await DalamudInterface.Instance.TextureProvider.CreateFromRawAsync(RawImageSpecification.Rgba32(frame.Width, frame.Height), buffer);
				timelineBuilder.AddFrame((int)(3 * currentPartId), new TimelineKeyFrameSet {
					PartId = currentPartId,
				});
				
				var texturePart = new Part {
					Size = texture.Size,
					Id = currentPartId++,
				};
				
				texturePart.LoadTexture(texture);
				
				imageNode.AddPart(texturePart);
			}
			
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