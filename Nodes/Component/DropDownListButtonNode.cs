namespace KamiToolKit.Nodes;

public class DropDownListButtonNode : DropDownListNode<string> {
    protected override string GetLabelForOption(string option) => option;
}
