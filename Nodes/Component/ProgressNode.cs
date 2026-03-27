using System.Numerics;
using KamiToolKit.Premade.Node.Simple;

namespace KamiToolKit.Nodes;

public abstract class ProgressNode : SimpleComponentNode {
    public abstract float Progress { get; set; }
    public abstract Vector4 BarColor { get; set; }
    public abstract Vector4 BackgroundColor { get; set; }
}
