using KamiToolKit.Nodes;

namespace KamiToolKit.Classes.Controllers.Overlay;

public abstract class OverlayNode : SimpleOverlayNode {

    public abstract OverlayLayer OverlayLayer { get; }

    public virtual void Update() { }
}
