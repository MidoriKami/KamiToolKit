using System;
using System.IO;
using System.Numerics;
using System.Threading.Tasks;
using Dalamud.Interface.Textures;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Classes.TimelineBuilding;
using KamiToolKit.NodeParts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace KamiToolKit.Nodes;

public class GifImageNode : ResNode {

    public ImageNode ImageNode;

    public GifImageNode() {
        ImageNode = new ImageNode {
            IsVisible = true,
        };

        ImageNode.AttachNode(this);
    }

    public required string FilePath {
        set {
            Task.Run(() => LoadFrames(value));
        }
    }

    public override float Width {
        get => base.Width;
        set {
            ImageNode.Width = value;
            base.Width = value;
        }
    }

    public override float Height {
        get => base.Height;
        set {
            ImageNode.Height = value;
            base.Height = value;
        }
    }

    public Vector2 GifFrameSize { get; private set; }

    public bool FitNodeToGif { get; set; }

    public Action? OnGifLoaded { get; set; }

    private async void LoadFrames(string filepath) {
        try {
            var image = await LoadAsync(filepath);
            if (image.Length <= 0) return;

            using var memoryStream = new MemoryStream(image);
            using var processedImage = Image.Load<Rgba32>(memoryStream);
            if (processedImage.Frames.Count is 0) return;

            uint currentPartId = 0;
            var frameDelay = processedImage.Frames.RootFrame.Metadata.GetGifMetadata().FrameDelay / 3.33333333f;
            var frameCount = (int)(processedImage.Frames.Count * frameDelay);
            GifFrameSize = new Vector2(processedImage.Width, processedImage.Height);

            if (FitNodeToGif) {
                Size = GifFrameSize;
            }

            foreach (var frame in processedImage.Frames) {
                var buffer = new byte[8 * frame.Width * frame.Height];

                frame.CopyPixelDataTo(buffer);

                var texture = await DalamudInterface.Instance.TextureProvider.CreateFromRawAsync(RawImageSpecification.Rgba32(frame.Width, frame.Height), buffer);

                var texturePart = new Part {
                    Size = texture.Size, Id = currentPartId++,
                };

                texturePart.LoadTexture(texture);
                ImageNode.AddPart(texturePart);
            }

            ImageNode.AddTimeline(new TimelineBuilder()
                .BeginFrameSet(1, frameCount)
                .AddFrame(0, partId: 0)
                .AddFrame(frameCount, partId: currentPartId)
                .EndFrameSet()
                .Build());

            AddTimeline(new TimelineBuilder()
                .BeginFrameSet(1, frameCount)
                .AddLabel(1, 200, AtkTimelineJumpBehavior.Start, 0)
                .AddLabel(frameCount, 0, AtkTimelineJumpBehavior.LoopForever, 200)
                .EndFrameSet()
                .Build());

            unsafe {
                InternalResNode->Timeline->PlayAnimation(AtkTimelineJumpBehavior.LoopForever, 200);
            }

            await DalamudInterface.Instance.Framework.RunOnFrameworkThread(() => {
                OnGifLoaded?.Invoke();
            });
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
