using System.Numerics;

namespace KamiToolKit.Internal.Classes;

internal record UldTextureInfo(float PositionX = 0.0f, float PositionY = 0.0f, float Width = 0.0f, float Height = 0.0f) {
    public Vector2 TextureCoordinates => new(PositionX, PositionY);
    public Vector2 TextureSize => new(Width, Height);
}
