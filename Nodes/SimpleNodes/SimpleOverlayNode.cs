namespace KamiToolKit.Nodes;

public class SimpleOverlayNode : SimpleComponentNode {
    public SimpleOverlayNode() {
        CollisionNode.IsVisible = false;
        CollisionNode.ClearEventFlags();
    }
}
