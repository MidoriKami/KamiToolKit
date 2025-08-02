using System.Drawing;
using System.Numerics;
using Dalamud.Interface;
using Vector4 = System.Numerics.Vector4;

namespace KamiToolKit.Extensions;

public static class KnownColorExtensions {
    public static Vector3 Vector3(this KnownColor color) {
        var color4 = color.Vector();
        return new Vector3(color4.X, color4.Y, color4.Z);
    }

    public static Vector3 AsVector3Color(this Vector4 vector4)
        => new(vector4.X, vector4.Y, vector4.Z);
}
