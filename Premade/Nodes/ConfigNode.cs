using System;
using KamiToolKit.Nodes;

namespace KamiToolKit.Premade.Nodes;

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
