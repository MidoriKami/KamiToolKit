using System;

namespace KamiToolKit.Classes;

public enum OverlayLayer {
    BackLayer,
    MiddleLayer,
    FrontLayer,
}

public static class OverlayLayerExtensions {
    public static string GetDescription(this OverlayLayer layer) => layer switch {
        OverlayLayer.BackLayer => "KTK_Overlay_Back",
        OverlayLayer.MiddleLayer => "KTK_Overlay_Middle",
        OverlayLayer.FrontLayer => "KTK_Overlay_Front",
        _ => throw new Exception("Out of Range"),
    };

    public static int GetOverlayLayer(this OverlayLayer layer) => layer switch {
        OverlayLayer.BackLayer => 1,
        OverlayLayer.MiddleLayer => 3,
        OverlayLayer.FrontLayer => 7,
        _ => 1,
    };

    public static OverlayLayer GetOverlayLayer(this uint layer) => layer switch {
        1 - 1 => OverlayLayer.BackLayer,
        3 - 1 => OverlayLayer.MiddleLayer,
        7 - 1 => OverlayLayer.FrontLayer,
        _ => throw new Exception("Unknown depth layer: " + layer),
    };
}
