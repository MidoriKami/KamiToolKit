using System;

namespace KamiToolKit.Classes;

public enum OverlayLayer {
    /// <summary>
    /// Layer that is the back most, this is below nameplates, but above the world itself.
    /// </summary>
    Background,
    
    /// <summary>
    /// Above nameplate layer
    /// </summary>
    BehindUserInterface,
    
    /// <summary>
    /// Above most windows but below certain popup windows like battle text
    /// </summary>
    AboveUserInterface,
    
    /// <summary>
    /// Above everything, use with caution
    /// </summary>
    Foreground,
}

public static class OverlayLayerExtensions {
    public static string GetDescription(this OverlayLayer layer) => layer switch {
        OverlayLayer.Background => "KTK_Overlay_Back",
        OverlayLayer.BehindUserInterface => "KTK_Overlay_Middle",
        OverlayLayer.AboveUserInterface => "KTK_Overlay_Higher",
        OverlayLayer.Foreground => "KTK_Overlay_Front",
        _ => throw new Exception("Out of Range"),
    };

    public static int GetOverlayLayer(this OverlayLayer layer) => layer switch {
        OverlayLayer.Background => 1,
        OverlayLayer.BehindUserInterface => 3,
        OverlayLayer.AboveUserInterface => 7,
        OverlayLayer.Foreground => 13,
        _ => 1,
    };
    
    // Note: The game does not have a layer zero, but offsets the desired layer by one.
    public static OverlayLayer GetOverlayLayer(this uint layer) => (layer - 1) switch {
        1 => OverlayLayer.Background,
        3 => OverlayLayer.BehindUserInterface,
        7 => OverlayLayer.AboveUserInterface,
        13 => OverlayLayer.Foreground,
        _ => throw new Exception("Unknown depth layer: " + layer),
    };
}
