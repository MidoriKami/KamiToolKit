using Lumina.Data.Parsing.Uld;

namespace KamiToolKit.Nodes;

/// <summary>
/// Implementation of <see cref="TextInputNode"/> that defaults the <see cref="TextInputNode.PlaceholderString"/>
/// to the localized "Search" text input id.
/// </summary>
public class SearchInputNode : TextInputNode {

    /// <summary>
    /// Constructs a new <see cref="SearchInputNode"/>.
    /// </summary>
    public unsafe SearchInputNode() {
        PlaceholderStringTextId = 325; // "Search"
        PlaceHolderStringSheetType = NodeData.SheetType.Addon;
        Component->UldManager.SetupText();
    }
}
