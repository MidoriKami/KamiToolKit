using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiToolKit.Interfaces;

/// <summary>
/// Interface representing a collision node for handling input collision detection.
/// </summary>
public unsafe interface ICollisionNode {

    /// <summary>
    /// Gets or sets the collision type.
    /// </summary>
    CollisionType CollisionType { get; set; }

    /// <summary>
    /// Gets or sets the uses.
    /// </summary>
    /// <remarks>
    /// Unknown what this is actually doing.
    /// </remarks>
    uint Uses { get; set; }

    /// <summary>
    /// Gets or sets the linked component.
    /// </summary>
    AtkComponentBase* LinkedComponent { get; set; }
}
