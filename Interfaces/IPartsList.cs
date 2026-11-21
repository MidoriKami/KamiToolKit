using System.Collections.Generic;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;

namespace KamiToolKit.Interfaces;

/// <summary>
/// Interface representing a node with parts and partslists.
/// </summary>
public interface IPartsList {

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    PartsList PartsList { get; }

    /// <summary>
    /// Sets the collection of parts to use for this node.
    /// </summary>
    ICollection<Part> Parts {
        set => PartsList.Add([.. value]);
    }

    /// <summary>
    /// Gets or sets the PartId.
    /// </summary>
    ushort PartId { get; set; }

    /// <summary>
    /// Adds a single part.
    /// </summary>
    unsafe AtkUldPart* AddPart(Part part)
        => PartsList.Add(part);

    /// <summary>
    /// Adds multiple parts.
    /// </summary>
    void AddPart(params Part[] parts)
        => PartsList.Add(parts);

    /// <summary>
    /// Adds a part from provided params.
    /// </summary>
    unsafe void AddPart(string path, Vector2 position, Vector2 size)
        => AddPart(new Part {
            TexturePath = path,
            TextureCoordinates = position,
            Size = size,
        });
}
