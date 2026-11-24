using KamiToolKit.Core.Enums;
using KamiToolKit.Core.Layout;

namespace KamiToolKit.Core.Interfaces.Nodes;

/// <summary>
/// Interface representing a horizontal flex node that evenly divides its area among children.
/// </summary>
public interface IHorizontalFlexNode : ILayoutUpdatable, ILayoutFiller {

    /// <summary>
    /// Gets or sets the alignment flags for adjusting children.
    /// </summary>
    FlexFlags AlignmentFlags { get; set; }
}
