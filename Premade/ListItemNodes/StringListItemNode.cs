using KamiToolKit.Premade.GenericListItemNodes;

namespace KamiToolKit.Premade.ListItemNodes;

public class StringListItemNode : GenericStringListItemNode<string> {
    protected override void SetNodeData(string itemData)
        => StringNode.String = itemData;
}
