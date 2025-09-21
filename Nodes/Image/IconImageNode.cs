﻿using System.Numerics;
using KamiToolKit.NodeParts;

namespace KamiToolKit.Nodes;

/// <summary>
///     A simple image node for use with displaying game icons.
/// </summary>
/// <remarks>This node is not intended to be used with multiple <see cref="Part" />'s.</remarks>
public unsafe class IconImageNode : SimpleImageNode {

    public IconImageNode() {
        TextureSize = new Vector2(32.0f, 32.0f);
    }
    
    public uint IconId {
        get;
        set {
            field = value;
            PartsList[0]->LoadIcon(value);
        }
    }

    public uint? LoadedIconId
        => Node->GetIconId();
}
