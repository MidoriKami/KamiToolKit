using System;
using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Core.Enums;
using KamiToolKit.Core.Interfaces;
using KamiToolKit.Core.Primitives.NativeAddon;
using KamiToolKit.UI.Nodes;
using KamiToolKit.UI.Nodes.ComponentNode;
using KamiToolKit.UI.Nodes.Layout;
using Lumina.Data.Parsing.Uld;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Controls.Search;

/// <summary>
/// Abstract class representing a search window.
/// </summary>
public class AbstractSearchAddon<T, TU> : NativeAddon where TU : ListItemNode<T>, IListItemNode, new() {

    /// <summary>
    /// List of all available options.
    /// </summary>
    public virtual List<T> OptionsList {
        get;
        set {
            field = value;
            ResultsListNode?.Options = value;
        }
    } = [];

    /// <summary>
    /// List of all selected options.
    /// </summary>
    public List<T> SelectedOptions { get; private set; } = [];

    /// <summary>
    /// When true allows multiple items to be selected.
    /// </summary>
    public bool AllowMultiselect { get; init; }

    /// <summary>
    /// Gets or sets an action to be called when the selection has been confirmed.
    /// Contains the list of selected entries.
    /// </summary>
    public Action<List<T>>? ConfirmedSelections { get; set; }

    /// <summary>
    /// Main layout container for this addon.
    /// </summary>
    protected LayoutListNode? LayoutContainer { get; private set; }

    /// <summary>
    /// Search input box.
    /// </summary>
    protected TextInputNode? SearchInputNode { get; private set; }

    /// <summary>
    /// Virtualized list node displaying the results in a performant way.
    /// </summary>
    protected ListNode<T, TU>? ResultsListNode { get; private set; }

    /// <summary>
    /// Function that is called once the user has pressed "Enter".
    /// </summary>
    protected virtual void OnSearchInputComplete(ReadOnlySeString searchString) { }

    /// <summary>
    /// Function that is called for each letter input into the search.
    /// </summary>
    protected virtual void OnSearchInputReceived(ReadOnlySeString searchString) {
        ResultsListNode?.ResetScroll();
    }

    /// <summary>
    /// Function that is called when the OK button is clicked.
    /// </summary>
    protected virtual void OnConfirmClicked() {
        SelectedOptions = ResultsListNode?.SelectedOptions ?? [];
        ConfirmedSelections?.Invoke(SelectedOptions);
        Close();
    }

    /// <summary>
    /// Function that is called when the Cancel button is clicked.
    /// </summary>
    protected virtual void OnCancelClicked() {
        SelectedOptions = [];
        Close();
    }

    /// <inheritdoc />
    protected override unsafe void OnSetup(AtkUnitBase* addon, Span<AtkValue> atkValueSpan) {
        base.OnSetup(addon, atkValueSpan);

        // Reset all selected options.
        SelectedOptions = [];

        LayoutContainer = new VerticalListNode {
            Position = ContentStartPosition,
            Size = ContentSize,
            StretchWidth = true,

            InitialNodes = [
                SearchInputNode = new SearchInputNode {
                    Height = 26.0f,
                    OnInputReceived = OnSearchInputReceived,
                    OnInputComplete = OnSearchInputComplete,
                    NavIndex = 1,
                    NavDown = 2,
                    NavUp = 100,
                },
                new ResNode { Height = 8.0f },
                ResultsListNode = new ListNode<T, TU> {
                    Height = ContentSize.Y - 26.0f - 26.0f - 16.0f,
                    AllowMultipleSelection = AllowMultiselect,
                    ItemSpacing = 2.0f,
                    Options = OptionsList,
                    NavIndex = 2,
                    NavUp = 1,
                    NavDown = 100,
                    NavLeft = 1,
                    NavRight = 100,
                },
                new ResNode { Height = 8.0f },
                new HorizontalFlexNode {
                    Width = ContentSize.X,
                    Height = 26.0f,
                    AlignmentFlags = FlexFlags.FitHeight | FlexFlags.FitWidth,
                    InitialNodes = [
                        new TextButtonNode {
                            TextId = 1, // "OK"
                            SheetType = NodeData.SheetType.Addon,
                            OnClick = OnConfirmClicked,
                            NavIndex = 100,
                            NavUp = 2,
                            NavDown = 1,
                            NavRight = 101,
                        },
                        new TextButtonNode {
                            TextId = 2, // "Cancel"
                            SheetType = NodeData.SheetType.Addon,
                            OnClick = OnCancelClicked,
                            NavIndex = 101,
                            NavUp = 2,
                            NavDown = 1,
                            NavLeft = 100,
                        },
                    ],
                },
            ],
        };

        LayoutContainer.AttachNode(this);
    }

    /// <inheritdoc />
    protected override unsafe void OnFinalize(AtkUnitBase* addon) {
        LayoutContainer = null;
        SearchInputNode = null;
        ResultsListNode = null;

        base.OnFinalize(addon);
    }
}
