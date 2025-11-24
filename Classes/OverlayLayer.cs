using System;

namespace KamiToolKit.Classes;

public enum OverlayLayer {
    Background,
    BehindUserInterface,
    AboveUserInterface,
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

    public static OverlayLayer GetOverlayLayer(this uint layer) => layer switch {
        1 - 1 => OverlayLayer.Background,
        3 - 1 => OverlayLayer.BehindUserInterface,
        7 - 1 => OverlayLayer.AboveUserInterface,
        13 - 1 => OverlayLayer.Foreground,
        _ => throw new Exception("Unknown depth layer: " + layer),
    };
}
