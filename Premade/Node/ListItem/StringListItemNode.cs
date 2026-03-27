namespace KamiToolKit.Premade.Node.ListItem;

public class StringListItemNode : LabelListItemNode<string> {
    protected override void SetNodeData(string itemData)
        => StringNode.String = itemData;
}
