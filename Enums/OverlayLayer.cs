using System;
using System.ComponentModel;

namespace KamiToolKit.Enums;

public enum OverlayLayer {
    /// <summary>
    /// Layer that is the back most, this is below nameplates, but above the world itself.
    /// </summary>
    [Description("KTK_Overlay_Back")]
    Background,
    
    /// <summary>
    /// Above nameplate layer
    /// </summary>
    [Description("KTK_Overlay_Middle")]
    BehindUserInterface,
    
    /// <summary>
    /// Above most windows but below certain popup windows like battle text
    /// </summary>
    [Description("KTK_Overlay_Higher")]
    AboveUserInterface,
    
    /// <summary>
    /// Above everything, use with caution
    /// </summary>
    [Description("KTK_Overlay_Front")]
    Foreground,
}

public static class OverlayLayerExtensions {
    extension(OverlayLayer layer) {
        public int DepthLayer => layer switch {
            OverlayLayer.Background => 1,
            OverlayLayer.BehindUserInterface => 3,
            OverlayLayer.AboveUserInterface => 7,
            OverlayLayer.Foreground => 13,
            _ => 1,
        };
    }

    // Note: The game does not have a layer zero, but offsets the desired layer by one.
    public static OverlayLayer GetOverlayLayer(this uint layer) => (layer + 1) switch {
        1 => OverlayLayer.Background,
        3 => OverlayLayer.BehindUserInterface,
        7 => OverlayLayer.AboveUserInterface,
        13 => OverlayLayer.Foreground,
        _ => throw new Exception("Unknown depth layer: " + layer),
    };
}
