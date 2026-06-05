using System;
using KamiToolKit.Nodes.Simplified;

namespace KamiToolKit.Premade.Node;

[Obsolete("Pending Removal")]
public abstract class ConfigNode<T> : SimpleComponentNode {
    public T? ConfigurationOption {
        get;
        set {
            field = value;
            OptionChanged(value);
        }
    }

    protected abstract void OptionChanged(T? option);

    public Action<T>? OnConfigChanged { get; set; }
}
