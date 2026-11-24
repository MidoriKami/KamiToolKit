using System;
using System.Collections.Generic;
using System.Numerics;
using KamiToolKit.UI.Nodes;

namespace KamiToolKit.Data.Icons;

/// <summary>
/// A node that represents a list of icon buttons, an icon preview, and an icon input text box.
/// </summary>
public class IconSelectionNode : ResNode {

    /// <summary>
    /// Action that is invoked when a different icon is selected.
    /// </summary>
    public Action<uint>? OnIconChanged { get; set; }

    /// <summary>
    /// Gets or sets the currently selected icon id.
    /// </summary>
    public required uint SelectedIcon {
        get;
        set {
            field = value;
            SelectedIconImageNode.IconId = value;
            IconIdInputNode.Value = (int) value;
        }
    }

    /// <summary>
    /// Gets the icon button list.
    /// </summary>
    public HorizontalListNode IconButtonListNode { get; }

    /// <summary>
    /// Gets the image node showing the currently selected option.
    /// </summary>
    public IconImageNode SelectedIconImageNode { get; }

    /// <summary>
    /// Gets the numeric input for inputting icon ids.
    /// </summary>
    public NumericInputNode IconIdInputNode { get; }

    /// <summary>
    /// Constructs a new <see cref="IconSelectionNode"/>
    /// </summary>
    public IconSelectionNode(List<uint> iconOptions) {
        IconButtonListNode = new HorizontalListNode();
        IconButtonListNode.AttachNode(this);

        foreach (var iconId in iconOptions) {
            IconButtonListNode.AddNode(new IconButtonNode {
                Size = new Vector2(32.0f, 32.0f),
                IconId = iconId,
                OnClick = () => {
                    if (iconId != SelectedIcon) {
                        OnIconChanged?.Invoke(iconId);
                        SelectedIcon = iconId;
                    }
                },
            });
        }

        SelectedIconImageNode = new IconImageNode {
            FitTexture = true,
        };
        SelectedIconImageNode.AttachNode(this);

        IconIdInputNode = new NumericInputNode();
        IconIdInputNode.AttachNode(this);
    }

    /// <inheritdoc />
    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        IconButtonListNode.Size = new Vector2(Width, Height / 2.0f);
        IconButtonListNode.Position = new Vector2(0.0f, 0.0f);

        foreach (var node in IconButtonListNode.Nodes) {
            node.Size = new Vector2(Height / 2.0f, Height / 2.0f);
        }

        IconButtonListNode.UpdateLayout();

        SelectedIconImageNode.Size = new Vector2(Height / 2.0f, Height / 2.0f);
        SelectedIconImageNode.Position = new Vector2(0.0f, Height / 2.0f);

        IconIdInputNode.Size = new Vector2(Width - Height / 2.0f, Height / 4.0f);
        IconIdInputNode.Position = new Vector2(SelectedIconImageNode.Bounds.Right, Height / 2.0f + IconIdInputNode.Height / 2.0f);
    }
}
