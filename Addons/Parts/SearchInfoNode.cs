using KamiToolKit.Addons.Interfaces;

namespace KamiToolKit.Addons.Parts;

internal class SearchInfoNode<T> : BaseSearchInfoNode<T> where T : IInfoNodeData {
    protected override void SetOptionParams(T? value) {
        LabelTextNode.String = value?.GetLabel() ?? string.Empty;
        SubLabelTextNode.String = value?.GetSubLabel() ?? string.Empty;
        IdTextNode.String = value?.GetId()?.ToString() ?? string.Empty;

        if (value?.GetIconId() is { } iconId) {
            IconNode.IconId = iconId;
        }

        if (value?.GetTexturePath() is { Length: > 0 } texturePath) {
            IconNode.LoadTexture(texturePath);
        }
    }

    public override int Compare(BaseSearchInfoNode<T> other, string sortOption, bool reversed) {
        var result = Option.Compare(other.Option, sortOption);
        
        return reversed ? -result : result;
    }

    public override bool IsMatch(string searchString)
        => Option.ContainsSearchTerm(searchString);
}
