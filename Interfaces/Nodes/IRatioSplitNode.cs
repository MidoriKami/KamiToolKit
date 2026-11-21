using KamiToolKit.BaseTypes;

namespace KamiToolKit.Interfaces;

/// <summary>
/// Interface representing a RatioSplitNode
/// </summary>
public interface IRatioSplitNode : ILayoutUpdatable, ILayoutFiller {

    /// <summary>
    /// Define a set of ratios, these need to add up to approx 1.0f.
    /// </summary>
    /// <example>
    /// [0.3f, 0.7f]
    /// </example>
    float[] Ratios { get; set; }

    /// <summary>
    /// Gets or sets whether the separator nodes will be shown.
    /// </summary>
    bool ShowSeparators { get; set; }

    /// <summary>
    /// Gets or sets if this node should resize itself to fit the contents.
    /// </summary>
    bool FitContents { get; set; }

    /// <summary>
    /// Adds a node to the ratio split node.
    /// </summary>
    /// <param name="node"></param>
    void AddNode(NodeBase? node);
}
