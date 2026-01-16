using KamiToolKit.Classes;

namespace KamiToolKit.Nodes;

public abstract class ListItemNode<T> : SelectableNode {
    public abstract float ItemHeight { get; }

    public T? ItemData {
        get;
        set {
            if (value is not null) {
                if (!GenericUtil.AreEqual(field, value)) {
                    SetNodeData(value);
                }
            }

            field = value;
            
            IsVisible = value is not null;
        }
    }

    protected abstract void SetNodeData(T itemData);

    public virtual void Update() { }
}
