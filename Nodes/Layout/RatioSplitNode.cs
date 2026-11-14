using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using KamiToolKit.BaseTypes;
using KamiToolKit.Interfaces;

namespace KamiToolKit.Nodes;

/// <summary>
/// A two part <see cref="LayoutListNode"/> that allows you to specify the relative width of each section.
/// </summary>
/// <remarks>
/// Useful for layouts that want 1/3rd and 2/3rd of the width respectively.
/// </remarks>
public class RatioSplitNode : HorizontalListNode, IRatioSplitNode {

    /// <inheritdoc />
    public float[] Ratios { get; set; } = [];

    /// <inheritdoc />
    public bool ShowSeparators { get; set; } = true;

    /// <inheritdoc />
    public bool FitContents { get; set; } = true;

    private readonly List<VerticalLineNode> separators = [];

    /// <inheritdoc />
    public override void AddNode(NodeBase? node) {
        if (NodeList.Count is not 0) {
            var separator = new VerticalLineNode {
                Width = 4.0f,
            };
            separator.AttachNode(this);
            separators.Add(separator);
        }

        base.AddNode(node);
    }

    /// <inheritdoc />
    protected override void OnRecalculateLayout() {
        if (NodeList.Count is 0) return;
        if (Ratios.Length != NodeList.Count) return;

        var spacingCount = Math.Max(0, NodeList.Count - 1);
        var totalItemSpacing = ItemSpacing * spacingCount;
        var totalSeparatorWidth = spacingCount * 6.0f;

        var availableWidth = Math.Max(0.0f, Width - FirstItemSpacing - totalItemSpacing - totalSeparatorWidth);

        var currentX = FirstItemSpacing;

        foreach (var (index, node) in NodeList.Index()) {
            var sectionWidth = availableWidth * Ratios[index];

            node.Size = new Vector2(sectionWidth, Height);
            node.Position = new Vector2(currentX, 0.0f);

            currentX += sectionWidth + ItemSpacing;

            // We have 1 fewer separators than we have nodes.
            if (index < NodeList.Count - 1) {
                var separator = separators[index];

                currentX += ItemSpacing + 2.0f;

                separator.IsVisible = ShowSeparators;
                separator.Position = new Vector2(currentX, 0.0f);
                separator.Height = Height;

                currentX += 4.0f;
            }

            if (node is ILayoutUpdatable splitChild) {
                splitChild.RecalculateLayout();
            }
        }

        if (FitContents) {
            Height = NodeList.Max(node => node.Height);
        }
    }
}
