namespace KamiToolKit.Premade.Nodes;

public abstract class StringInfoNode : IInfoNodeData {
    public virtual string Label { get; init; } = string.Empty;

    public string GetLabel() => Label;

    public abstract string? GetSubLabel();

    public abstract uint? GetId();

    public abstract uint? GetIconId();

    public abstract string? GetTexturePath();

    public virtual int Compare(IInfoNodeData other, string sortingMode)
        => string.CompareOrdinal(Label, (other as StringInfoNode)?.Label);
}
