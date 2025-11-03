using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Addon;
using KamiToolKit.Addons.Interfaces;
using KamiToolKit.Addons.Parts;
using KamiToolKit.Nodes;
using KamiToolKit.Widgets;

namespace KamiToolKit.Addons;

/// <summary>
/// A generic window for selecting a single entry among a list of options.
/// </summary>
public class SearchAddon<T> : NativeAddon where T : IInfoNodeData {

    private SearchWidget? searchWidget;
    private ScrollingAreaNode<VerticalListNode>? listNode;

    private TextButtonNode? cancelButton;
    private TextButtonNode? confirmButton;

    private BaseSearchInfoNode<T>? selectedOption;

    protected override unsafe void OnSetup(AtkUnitBase* addon) {
        searchWidget = new SearchWidget {
            Size = ContentSize,
            Position = ContentStartPosition,
            SortingOptions = SortingOptions,
            OnSortOrderChanged = OnSortOrderUpdated,
            OnSearchUpdated = OnSearchUpdated,
            IsVisible = true,
        };
        AttachNode(searchWidget);

        listNode = new ScrollingAreaNode<VerticalListNode> {
            Position = new Vector2(ContentStartPosition.X, searchWidget.Y + searchWidget.Height + 8.0f),
            Size = new Vector2(ContentSize.X, ContentSize.Y - searchWidget.Height - 16.0f - 24.0f - 8.0f),
            IsVisible = true,
            AutoHideScrollBar = true,
            ContentHeight = 10.0f,
        };
        listNode.ContentNode.FitContents = true;
        AttachNode(listNode);

        const float buttonPadding = 20.0f;
        var contentWidth = ContentSize.X - buttonPadding * 2;
        var buttonWidth = contentWidth / 3.0f;
        
        cancelButton = new TextButtonNode {
            Size = new Vector2(buttonWidth, 24.0f),
            Position = new Vector2(ContentStartPosition.X, ContentStartPosition.Y + ContentSize.Y - 24.0f - 8.0f),
            IsVisible = true,
            String = "Cancel",
            OnClick = OnCancelClicked,
        };
        AttachNode(cancelButton);
        
        confirmButton = new TextButtonNode {
            Size = new Vector2(buttonWidth, 24.0f),
            Position = new Vector2(ContentStartPosition.X + buttonWidth * 2 + buttonPadding * 2, ContentStartPosition.Y + ContentSize.Y - 24.0f - 8.0f),
            IsVisible = true,
            IsEnabled = false,
            String = "Confirm",
            OnClick = OnConfirmClicked,
        };
        AttachNode(confirmButton);
        
        foreach (var option in SearchOptions) {
            listNode.ContentNode.AddNode(BuildOptionNode(option), true);
        }

        listNode.ContentNode.RecalculateLayout();
        listNode.ContentHeight = listNode.ContentNode.Height;

        if (SortingOptions.Count > 0) {
            OnSortOrderUpdated(SortingOptions.First(), false);
        }
    }

    private void OnCancelClicked() {
        selectedOption = null;
        Close();
    }

    private void OnConfirmClicked() {
        if (selectedOption is { Option: { } option }) {
            SelectionResult?.Invoke(option);
        }
        
        selectedOption = null;
        Close();
    }

    private SearchInfoNode<T> BuildOptionNode(T option) => new() {
        Width = listNode!.ContentNode.Width,
        Height = 48.0f,
        Option = option,
        IsVisible = true,
        OnClicked = OnOptionClicked,
    };

    private void OnOptionClicked(BaseSearchInfoNode<T> clickedOption) {
        if (confirmButton is null) return;
        
        // Unselect Previous Option
        if (selectedOption is not null) {
            selectedOption.IsSelected = false;
            selectedOption = null;
        }

        // Select New Option
        selectedOption = clickedOption;
        selectedOption.IsSelected = true;

        // Enable Confirm Button
        confirmButton.IsEnabled = true;
    }

    private void OnSortOrderUpdated(string sortingString, bool reversed) => listNode?.ContentNode.ReorderNodes((x, y) => {
        if (x is not SearchInfoNode<T> left || y is not SearchInfoNode<T> right) return 0;

        var compareResult = left.Compare(right, sortingString, reversed);
        return reversed ? -compareResult : compareResult;
    });

    private void OnSearchUpdated(string searchString) {
        if (listNode is null) return;

        foreach (var node in listNode.ContentNode.GetNodes<SearchInfoNode<T>>()) {
            node.IsVisible = node.IsMatch(searchString);
        }

        if (selectedOption is not null) {
            selectedOption.IsSelected = false;
        }

        selectedOption = null;
        
        listNode.ContentNode.RecalculateLayout();
        listNode.ContentHeight = listNode.ContentNode.Height;
    }

    public required List<string> SortingOptions { get; init; }
    
    public required List<T> SearchOptions { get; set; }

    public Action<T>? SelectionResult { get; set; }
}
