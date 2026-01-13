namespace KamiToolKit.Nodes;

public abstract class ListItemNode<T> : SelectableNode {
    public abstract float ItemHeight { get; }

    public T? ItemData {
        get;
        internal set {
            field = value;
            
            if (value is not null) {
                if (!typeof(T).IsValueType || !value.Equals(default(T))) {
                    SetNodeData(value);
                }
            }

            IsVisible = value is not null;
        }
    }

    protected abstract void SetNodeData(T itemData);

    public virtual void Update() { }
}
