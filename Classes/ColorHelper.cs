using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiToolKit.Classes;

public static unsafe class ColorHelper {
    public static Vector4 GetColor(uint colorId)
        => ConvertToVector4(AtkStage.Instance()->AtkUIColorHolder->GetColor(true, colorId));

    private static Vector4 ConvertToVector4(uint color) {
        var a = (byte)(color >> 24);
        var b = (byte)(color >> 16);
        var g = (byte)(color >> 8);
        var r = (byte)color;

        return new Vector4(r / 255.0f, g / 255.0f, b / 255.0f, a / 255.0f);
    }
}
