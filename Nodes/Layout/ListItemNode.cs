using KamiToolKit.Classes;

namespace KamiToolKit.Nodes;

public abstract class ListItemNode<T> : SelectableNode {
    public abstract float ItemHeight { get; }

    public T? ItemData {
        get;
        set {
            if (value is not null) {
                if (!GenericUtil.AreEqual(field, value)) {
                    IsSettingNodeData = true;
                    SetNodeData(value);
                    IsSettingNodeData = false;
                }
            }

            field = value;
            
            IsVisible = value is not null;
        }
    }

    /// <summary>
    /// Bool that indicates if SetNodeDate when different is being called.
    /// Used to prevent things like checkboxes from trigger a file save due to the value being changed.
    /// </summary>
    protected bool IsSettingNodeData { get; private set; }
    
    protected abstract void SetNodeData(T itemData);

    public virtual void Update() { }
}
