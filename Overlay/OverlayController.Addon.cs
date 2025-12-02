using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiToolKit.Overlay;

internal class OverlayAddon : NativeAddon {

    // This is only for OverlayAddons that are intended to be shared.
    // You should not attempt to do this for your ui elements.
    // There is a fixed hard limit on the number of addons that can be loaded at once, we have to share and not be wasteful.
    protected override unsafe void OnSetup(AtkUnitBase* addon) {
        addon->WindowNode = null;
        addon->WindowHeaderCollisionNode = null;
        addon->WindowCollisionNode = null;

        WindowNode.DetachNode();
        InternalAddon->UldManager.RemoveNodeFromObjectList(WindowNode);
        InternalAddon->UldManager.UpdateDrawNodeList();

        WindowNode.Dispose();
    }
}
