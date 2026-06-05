using System;

namespace KamiToolKit.Premade.Node.ListItem;

[Obsolete("Pending Removal")]
public class StringListItemNode : LabelListItemNode<string> {
    protected override void SetNodeData(string itemData)
        => StringNode.String = itemData;
}
