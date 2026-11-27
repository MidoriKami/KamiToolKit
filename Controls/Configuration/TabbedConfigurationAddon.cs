using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Controls.ConfigurationNodes;
using KamiToolKit.Core.Enums;
using KamiToolKit.Core.Interfaces;
using KamiToolKit.Core.Primitives.NativeAddon;
using KamiToolKit.Core.Primitives.NodeBase;
using KamiToolKit.UI.Nodes;
using KamiToolKit.UI.Nodes.ComponentNode;
using KamiToolKit.UI.Nodes.Layout;
using Lumina.Data.Parsing.Uld;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Controls.Configuration;

/// <summary>
/// A variant of <see cref="ConfigurationAddon{T,TU,TV}"/> that implements individual tabs.
/// </summary>
public class TabbedConfigurationAddon<T, TU, TV, TW> : NativeAddon
    where T : class
    where TV : EntryConfigurationNode<T>, new()
    where TU : ListItemNode<T>, IListItemNode, new()
    where TW : NodeBase, new() {

    /// <summary>
    /// Gets the currently selected entry.
    /// </summary>
    public T? SelectedEntry { get; private set; }

    /// <summary>
    /// List of all available options.
    /// </summary>
    public List<T> OptionsList {
        get;
        set {
            field = value;
            OptionsListNode?.Options = value;
        }
    } = [];

    /// <summary>
    /// Function that is called to get a comparable string to use for matching the input search string.
    /// </summary>
    public required Func<T, string> GetEntrySearchString { get; set; }

    /// <summary>
    /// Callback that is invoked when the add button is clicked.
    /// </summary>
    public Action? AddClicked { get; set; }

    /// <summary>
    /// Callback that is invoked when the remove button is clicked.
    /// </summary>
    /// <remarks>
    /// Contains a reference to the entry that was removed.
    /// </remarks>
    public Action<T>? RemoveClicked { get; set; }

    /// <summary>
    /// Action that is called when the <see cref="EntryConfigurationNode"/> has changed data.
    /// </summary>
    public Action? SaveConfig { get; set; }

    /// <summary>
    /// The starting nav index for the body section of nodes next to the selection list.
    /// </summary>
    protected int BodyNavIndex { get; private set; } = 150;

    /// <summary>
    /// Main layout container for this addon.
    /// </summary>
    protected LayoutListNode? LayoutContainer { get; private set; }

    /// <summary>
    /// Search input box.
    /// </summary>
    protected TextInputNode? SearchInputNode { get; private set; }

    /// <summary>
    /// Gets the remove button node.
    /// </summary>
    protected TextButtonNode? RemoveButtonNode { get; private set; }

    /// <summary>
    /// Gets the layout node used to display the options list config page.
    /// </summary>
    protected LayoutListNode? OptionsConfigurationPage { get; private set; }

    /// <summary>
    /// Gets the node used to display the general options.
    /// </summary>
    protected TW? GeneralConfigurationPage { get; private set; }

    /// <summary>
    /// Virtualized list node displaying the results in a performant way.
    /// </summary>
    protected ListNode<T, TU>? OptionsListNode { get; private set; }

    /// <summary>
    /// The node used to display configuration options for the selected value.
    /// </summary>
    protected TV? EntryConfigurationNode { get; private set; }

    /// <summary>
    /// Function that is called when the add button is clicked.
    /// </summary>
    protected virtual void OnAddClicked() {
        AddClicked?.Invoke();
    }

    /// <summary>
    /// Function that is called when the Remove button is clicked.
    /// </summary>
    /// <remarks>
    /// Requires a non-null entry to be selected as <see cref="SelectedEntry"/>
    /// </remarks>
    protected virtual void OnRemoveClicked() {
        if (SelectedEntry is not null) {
            RemoveClicked?.Invoke(SelectedEntry);
            OptionsList.Remove(SelectedEntry);

            OnEntrySelected(null);
            OptionsListNode?.Update();
        }
    }

    /// <inheritdoc />
    protected override unsafe void OnSetup(AtkUnitBase* addon, Span<AtkValue> atkValueSpan) {
        base.OnSetup(addon, atkValueSpan);

        var tabbedContainer = new TabbedContentNode {
            Position = ContentStartPosition,
            Size = ContentSize,
        };

        OptionsConfigurationPage = new HorizontalListNode {
            ItemSpacing = 4.0f,
            InitialNodes = [
                new VerticalListNode {
                    Width = ContentSize.X * 4.0f / 10.0f - 5.0f,
                    StretchWidth = true,
                    InitialNodes = [
                        SearchInputNode = new SearchInputNode {
                            Height = 26.0f,
                            OnInputReceived = OnSearchInputReceived,
                        },
                        new ResNode { Height = 8.0f },
                        OptionsListNode = new ListNode<T, TU> {
                            AllowMultipleSelection = false,
                            ItemSpacing = 2.0f,
                            Options = OptionsList,
                            OnOptionSelected = OnEntrySelected,
                        },
                        new ResNode { Height = 8.0f },
                        new HorizontalFlexNode {
                            Width = ContentSize.X,
                            Height = 26.0f,
                            AlignmentFlags = FlexFlags.FitHeight | FlexFlags.FitWidth,
                            ItemSpacing = 6.0f,
                            InitialNodes = [
                                new TextButtonNode {
                                    TextId = 302, // "Add"
                                    SheetType = NodeData.SheetType.Addon,
                                    OnClick = OnAddClicked,
                                },
                                RemoveButtonNode = new TextButtonNode {
                                    TextId = 85, // "Remove"
                                    SheetType = NodeData.SheetType.Addon,
                                    OnClick = OnRemoveClicked,
                                    IsEnabled = false,
                                },
                            ],
                        },
                    ],
                },
                new VerticalLineNode { Width = 4.0f },
                EntryConfigurationNode = new TV {
                    Width = ContentSize.X * 6.0f / 10.0f - 5.0f,
                    SaveConfig = SaveConfig,
                },
            ],
        };

        GeneralConfigurationPage = new TW();
        tabbedContainer.AddTab(new TabBarEntry {
            TextId = 465, // "Options"
            SheetType = NodeData.SheetType.Addon,
            OnClick = OnOptionsTabSelected,
        }, OptionsConfigurationPage);
        tabbedContainer.AddTab(new TabBarEntry {
            TextId = 662, // "General"
            SheetType = NodeData.SheetType.Addon,
            OnClick = OnGeneralTabSelected,
        }, GeneralConfigurationPage);

        LayoutContainer = tabbedContainer;
        LayoutContainer.AttachNode(this);
    }

    /// <inheritdoc />
    protected override unsafe void OnFinalize(AtkUnitBase* addon) {
        LayoutContainer = null;
        SearchInputNode = null;
        OptionsListNode = null;
        RemoveButtonNode = null;
        GeneralConfigurationPage = null;

        base.OnFinalize(addon);
    }

    private void OnSearchInputReceived(ReadOnlySeString searchString) {
        try {
            var searchRegex = new Regex(searchString.ToString(),  RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

            OptionsListNode?.Options = OptionsList.Where(option => searchRegex.IsMatch(GetEntrySearchString(option))).ToList();
            OnEntrySelected(null);
        }
        catch (RegexParseException) { }
    }

    private void OnEntrySelected(T? entry) {
        EntryConfigurationNode?.SelectEntry(entry);
        SelectedEntry = entry;

        RemoveButtonNode?.IsEnabled = SelectedEntry is not null;
    }

    private void OnGeneralTabSelected() {
        OptionsConfigurationPage?.IsVisible = false;
        GeneralConfigurationPage?.IsVisible = true;
    }

    private void OnOptionsTabSelected() {
        OptionsConfigurationPage?.IsVisible = true;
        GeneralConfigurationPage?.IsVisible = false;
    }
}
