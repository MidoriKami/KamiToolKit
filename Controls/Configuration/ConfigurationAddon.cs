using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Controls.ConfigurationNodes;
using KamiToolKit.Core.Enums;
using KamiToolKit.Core.Interfaces;
using KamiToolKit.Core.Primitives.NativeAddon;
using KamiToolKit.UI.Nodes;
using KamiToolKit.UI.Nodes.ComponentNode;
using KamiToolKit.UI.Nodes.Layout;
using Lumina.Data.Parsing.Uld;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Controls.Configuration;

/// <summary>
/// Window to easily configure a list of entries.
/// </summary>
/// <typeparam name="T">The entry type of your list.</typeparam>
/// <typeparam name="TU">The <see cref="ListItemNode{T}"/>> representing the selectable list item.</typeparam>
/// <typeparam name="TV">The <see cref="EntryConfigurationNode{T}"/> representing the configuration of a specific entry.</typeparam>
public class ConfigurationAddon<T, TU, TV> : NativeAddon where TV : EntryConfigurationNode<T>, new() where T : class where TU : ListItemNode<T>, IListItemNode, new() {

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
    /// Callback that is invoked when the window has edited a config entry.
    /// </summary>
    public Action? SaveConfig { get; set; }

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

        LayoutContainer = new HorizontalListNode {
            Position = ContentStartPosition,
            Size = ContentSize,
            FitHeight = true,
            ItemSpacing = 4.0f,
            InitialNodes = [
                new VerticalListNode {
                    Width = ContentSize.X * 4.0f / 10.0f - 5.0f,
                    StretchWidth = true,
                    InitialNodes = [
                        SearchInputNode = new SearchInputNode {
                            Height = 26.0f,
                            OnInputReceived = OnSearchInputReceived,
                            NavIndex = 1,
                            NavDown = 2,
                            NavUp = 100,
                            NavRight = BodyNavIndex,
                        },
                        new ResNode { Height = 8.0f },
                        OptionsListNode = new ListNode<T, TU> {
                            Height = ContentSize.Y - 26.0f - 26.0f - 16.0f,
                            AllowMultipleSelection = false,
                            ItemSpacing = 2.0f,
                            Options = OptionsList,
                            OnOptionSelected = OnEntrySelected,
                            NavIndex = 2,
                            NavUp = 1,
                            NavDown = 100,
                            NavRight = BodyNavIndex,
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
                                    NavIndex = 100,
                                    NavUp = 2,
                                    NavDown = 1,
                                    NavRight = 101,
                                },
                                RemoveButtonNode = new TextButtonNode {
                                    TextId = 85, // "Remove"
                                    SheetType = NodeData.SheetType.Addon,
                                    OnClick = OnRemoveClicked,
                                    IsEnabled = false,
                                    NavIndex = 101,
                                    NavUp = 2,
                                    NavDown = 1,
                                    NavLeft = 100,
                                    NavRight = BodyNavIndex,
                                },
                            ],
                        },
                    ],
                },
                new VerticalLineNode { Width = 4.0f },
                EntryConfigurationNode = new TV {
                    Width =  ContentSize.X * 6.0f / 10.0f - 5.0f,
                    SaveConfig = SaveConfig,
                },
            ],
        };

        LayoutContainer.AttachNode(this);
    }

    /// <inheritdoc />
    protected override unsafe void OnUpdate(AtkUnitBase* addon) {
        base.OnUpdate(addon);

        OptionsListNode?.Update();
    }

    /// <inheritdoc />
    protected override unsafe void OnFinalize(AtkUnitBase* addon) {
        base.OnFinalize(addon);

        LayoutContainer = null;
        SearchInputNode = null;
        OptionsListNode = null;
        RemoveButtonNode = null;
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
}
