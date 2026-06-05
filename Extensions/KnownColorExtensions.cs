using System.Drawing;
using System.Numerics;
using Dalamud.Interface;
using Vector4 = System.Numerics.Vector4;

namespace KamiToolKit.Extensions;

/// <summary>
/// KnownColor Extensions.
/// </summary>
public static class KnownColorExtensions {

    /// <summary>
    /// Converts a KnownColor to a Vector3 for uses where alpha is not supported.
    /// </summary>
    public static Vector3 Vector3(this KnownColor color) {
        var color4 = color.Vector();
        return new Vector3(color4.X, color4.Y, color4.Z);
    }

    /// <summary>
    /// Converts a Vector4 Color to a Vector3 color for uses where alpha is not supported.
    /// </summary>
    public static Vector3 AsVector3Color(this Vector4 vector4)
        => new(vector4.X, vector4.Y, vector4.Z);
}
