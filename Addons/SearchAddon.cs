using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Addon;
using KamiToolKit.Addons.Parts;
using KamiToolKit.Nodes;
using KamiToolKit.System;
using KamiToolKit.Widgets;

namespace KamiToolKit.Addons;

public class SearchAddon<T> : NativeAddon {

    public required List<string> SortingOptions { get; init; }

    private SearchWidget? searchWidget;
    private ScrollingAreaNode<VerticalListNode>? listNode;

    private TextButtonNode? cancelButton;
    private TextButtonNode? confirmButton;

    private SearchInfoNode<T>? selectedOption;

    protected override unsafe void OnSetup(AtkUnitBase* addon) {
        searchWidget = new SearchWidget {
            Size = ContentSize,
            Position = ContentStartPosition,
            FilterOptions = SortingOptions,
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
            listNode.ContentNode.AddNode(BuildOptionNode(GetOptionInfo(option)), true);
        }

        listNode.ContentNode.RecalculateLayout();
        listNode.ContentHeight = listNode.ContentNode.Nodes.Sum(node => node.IsVisible ? node.Height : 0.0f);

        if (SortingOptions.Count > 0) {
            OnSortOrderUpdated(SortingOptions.First(), false);
        }
    }

    private void OnCancelClicked() {
        selectedOption = null;
        Close();
    }

    private void OnConfirmClicked() {
        if (selectedOption != null) {
            SelectionResult.Invoke(selectedOption.OptionInfo.Option);
        }
        
        selectedOption = null;
        Close();
    }

    private SearchInfoNode<T> BuildOptionNode(OptionInfo<T> option) => new() {
        Width = listNode!.ContentNode.Width,
        Height = 48.0f,
        OptionInfo = option,
        IsVisible = true,
        OnClicked = OnOptionClicked,
    };

    private void OnOptionClicked(SearchInfoNode<T> clickedOption) {
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

    private void OnSortOrderUpdated(string filterString, bool reversed)
        => listNode?.ContentNode.ReorderNodes((left, right) => 
            NodeComparison(left, right, filterString, reversed));

    private int NodeComparison(NodeBase x, NodeBase y, string filterString, bool isReversed) {
        if (x is not SearchInfoNode<T> left || y is not SearchInfoNode<T> right) return 0;

        var leftValue = left.OptionInfo;
        var rightValue = right.OptionInfo;

        var comparisonResult = ItemComparison(leftValue.Option, rightValue.Option, filterString);
        
        return isReversed ? -comparisonResult : comparisonResult;
    }

    private void OnSearchUpdated(string searchString) {
        if (listNode is null) return;

        foreach (var node in listNode.ContentNode.GetNodes<SearchInfoNode<T>>()) {
            node.IsVisible = node.OptionInfo.ContainsSearchTerm(searchString);
        }

        listNode.ContentHeight = listNode.ContentNode.Nodes.Sum(node => node.IsVisible ? node.Height : 0.0f) + 10.0f;
        listNode.ContentNode.RecalculateLayout();
    }

    public required List<T> SearchOptions { get; init; }

    public delegate OptionInfo<T> GetOptionInfoDelegate(T option);
    public required GetOptionInfoDelegate GetOptionInfo { get; init; }

    public delegate int Comparison(T left, T right, string selectedOrder);
    public required Comparison ItemComparison { get; init; }

    public required Action<T> SelectionResult { get; init; }
}
