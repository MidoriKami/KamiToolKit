namespace KamiToolKit.Nodes;

public class TextButtonListNode : ButtonListNode<string> {
    protected override string GetLabelForOption(string option) => option;
}
