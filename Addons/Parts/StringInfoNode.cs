using KamiToolKit.Addons.Interfaces;

namespace KamiToolKit.Addons.Parts;

public abstract class StringInfoNode : IInfoNodeData {
    public string Label { get; set; } = string.Empty;

    public string GetLabel() => Label;

    public abstract string? GetSubLabel();

    public abstract uint? GetId();

    public abstract uint? GetIconId();

    public abstract string? GetTexturePath();

    public int Compare(IInfoNodeData other, string sortingMode)
        => string.CompareOrdinal(Label, (other as StringInfoNode)?.Label);
}
