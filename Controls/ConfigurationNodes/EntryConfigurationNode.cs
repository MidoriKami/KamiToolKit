using System;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.UI.Nodes;
using Lumina.Data.Parsing.Uld;

namespace KamiToolKit.Controls.ConfigurationNodes;

/// <summary>
/// Node representing an editor for the various fields or properties of the contained type.
/// </summary>
public abstract class EntryConfigurationNode<T> : ResNode where T : class {

    /// <summary>
    /// Sets the node to display information for the specified entry.
    /// </summary>
    /// <remarks>
    /// If null will hide the config options and show "Select an Item"
    /// </remarks>
    public void SelectEntry(T? entry) {
        if (entry is null) {
            ConfigurationContentNode.IsVisible = false;
            SelectAnItemTextNode.IsVisible = true;

            ClearEntryData();
        }
        else {
            ConfigurationContentNode.IsVisible = true;
            SelectAnItemTextNode.IsVisible = false;

            PopulateEntryData(entry);
        }
    }

    /// <summary>
    /// Callback adaptor to allow triggering a save when a entry is edited.
    /// </summary>
    public Action? SaveConfig { get; set; }

    /// <summary>
    /// Gets the text node that shows "Select an Item" when no item is selected.
    /// </summary>
    protected TextNode SelectAnItemTextNode { get; }

    /// <summary>
    /// Gets the content node you need to attach your options to.
    /// </summary>
    protected ResNode ConfigurationContentNode { get; }

    /// <summary>
    /// Function that is called when this configuration node is representing a different entry.
    /// </summary>
    protected abstract void PopulateEntryData(T entry);

    /// <summary>
    /// Function that is called when this configuration node is reset/cleared/unselected.
    /// </summary>
    protected virtual void ClearEntryData() { }

    /// <summary>
    /// Constructs a new <see cref="EntryConfigurationNode{T}"/>
    /// </summary>
    protected EntryConfigurationNode() {
        SelectAnItemTextNode = new TextNode {
            TextId = 1818,
            SheetType = NodeData.SheetType.Addon,
            AlignmentType = AlignmentType.Center,
        };
        SelectAnItemTextNode.AttachNode(this);

        ConfigurationContentNode = new ResNode {
            IsVisible = false,
        };
        ConfigurationContentNode.AttachNode(this);
    }

    /// <inheritdoc />
    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        SelectAnItemTextNode.Size = Size;
        ConfigurationContentNode.Size = Size;
    }
}
