using System.Linq;
using System.Numerics;

namespace KamiToolKit.Nodes;

public class LabelLayoutNode : LayoutListNode {

    public bool FillWidth { get; set; }
    
    protected override void OnRecalculateLayout() {
        if (Nodes.Count is 0) return;
        
        var labelNode = Nodes[0];
        
        var labelNodeWidth = labelNode.Width;
        labelNode.Position = new Vector2(0.0f, 0.0f);

        var position = labelNodeWidth + FirstItemSpacing;
        foreach (var node in Nodes.Skip(1)) {
            node.X = position;

            if (FillWidth) {
                node.Width = (Width - labelNodeWidth - FirstItemSpacing) / (Nodes.Count - 1); 
            }

            position += node.Width + ItemSpacing;
        }
    }
}
