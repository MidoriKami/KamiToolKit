using System.Numerics;
using FFXIVClientStructs.FFXIV.Client.Graphics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Extensions;

namespace KamiToolKit.NodeParts;

public class NodeTint {

    public Vector3 AddColor;

    public Vector3 MultiplyColor;

    public static implicit operator AtkTimelineNodeTint(NodeTint tint) => new() {
        MultiplyRGB = new ByteColor {
            R = (byte)tint.MultiplyColor.X, G = (byte)tint.MultiplyColor.Y, B = (byte)tint.MultiplyColor.Z,
        },
        AddRGBBitfield = Convert(tint.AddColor),
    };

    public static implicit operator NodeTint(AtkTimelineNodeTint tint) => new() {
        AddColor = new Vector3(tint.AddR, tint.AddG, tint.AddB), MultiplyColor = tint.MultiplyRGB.ToVector4().AsVector3(),
    };

    private static uint Convert(Vector3 color) {
        var red = (short)(color.X + 255);
        var green = (short)(color.Y + 255);
        var blue = (short)(color.Z + 255);

        return (uint)(red & 0x3FF | (green & 0xFFF) << 10 | (blue & 0x3FF) << 22);
    }
}
