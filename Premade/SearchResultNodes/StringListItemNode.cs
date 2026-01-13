using KamiToolKit.Premade.GenericSearchListItemNodes;

namespace KamiToolKit.Premade.SearchResultNodes;

public class StringListItemNode : GenericListItemNode<string> {

    protected override uint GetIconId(string data)
        => 60072;

    protected override string GetLabelText(string data)
        => data;

    protected override string GetSubLabelText(string data)
        => data;

    protected override uint? GetId(string data)
        => null;
}
