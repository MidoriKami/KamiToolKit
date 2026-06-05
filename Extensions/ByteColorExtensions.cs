using System.Numerics;
using FFXIVClientStructs.FFXIV.Client.Graphics;

namespace KamiToolKit.Extensions;

/// <summary>
/// ByteColor extension methods.
/// </summary>
public static class ByteColorExtensions {
    /// <summary>
    /// Converts a ByteColor to a Vector4 with ranges 0.0f to 1.0f.
    /// </summary>
    public static Vector4 ToVector4(this ByteColor color)
        => new(color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, color.A / 255.0f);

    /// <summary>
    /// Converts a Vector4 with ranges 0.0f to 1.0f, to a ByteColor.
    /// </summary>
    public static ByteColor ToByteColor(this Vector4 vector) => new() {
        A = (byte)(vector.W * 255),
        R = (byte)(vector.X * 255),
        G = (byte)(vector.Y * 255),
        B = (byte)(vector.Z * 255),
    };
}
