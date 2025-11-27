using KamiToolKit.Classes;
using KamiToolKit.Nodes;

namespace KamiToolKit.Overlay;

public abstract class OverlayNode : SimpleOverlayNode {

    public abstract OverlayLayer OverlayLayer { get; }

    /// <summary>
    /// When true, this node will automatically hide when the game hides things like nameplates
    /// </summary>
    public virtual bool HideWithNativeUi => true;

    internal bool PreAutoHideState;

    public virtual void Update() { }
}
