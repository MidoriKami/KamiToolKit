using Lumina.Data.Parsing.Uld;

namespace KamiToolKit.Nodes;

/// <summary>
/// Implementation of <see cref="TextInputNode"/> that defaults the <see cref="TextInputNode.PlaceholderString"/>
/// to the localized "Search" text input id.
/// </summary>
public class SearchInputNode : TextInputNode {
    public SearchInputNode() {
        PlaceholderStringId = 325; // "Search"
        SheetType = NodeData.SheetType.Addon;
    }
}
