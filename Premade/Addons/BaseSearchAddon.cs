using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;
using KamiToolKit.Premade.Nodes;
using KamiToolKit.Premade.Widgets;

namespace KamiToolKit.Premade.Addons;

public abstract class BaseSearchAddon<T> : NativeAddon {
    
    private SearchWidget? searchWidget;
    private ScrollingListNode? listNode;

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
        };
        searchWidget.AttachNode(this);

        listNode = new ScrollingListNode {
            Position = new Vector2(ContentStartPosition.X, searchWidget.Y + searchWidget.Height + 8.0f),
            Size = new Vector2(ContentSize.X, ContentSize.Y - searchWidget.Height - 16.0f - 24.0f - 8.0f),
            AutoHideScrollBar = true,
            FitContents = true,
        };
        listNode.AttachNode(this);

        const float buttonPadding = 20.0f;
        var contentWidth = ContentSize.X - buttonPadding * 2;
        var buttonWidth = contentWidth / 3.0f;
        
        cancelButton = new TextButtonNode {
            Size = new Vector2(buttonWidth, 24.0f),
            Position = new Vector2(ContentStartPosition.X, ContentStartPosition.Y + ContentSize.Y - 24.0f - 8.0f),
            String = "Cancel",
            OnClick = OnCancelClicked,
        };
        cancelButton.AttachNode(this);
        
        confirmButton = new TextButtonNode {
            Size = new Vector2(buttonWidth, 24.0f),
            Position = new Vector2(ContentStartPosition.X + buttonWidth * 2 + buttonPadding * 2, ContentStartPosition.Y + ContentSize.Y - 24.0f - 8.0f),
            IsEnabled = false,
            String = "Confirm",
            OnClick = OnConfirmClicked,
        };
        confirmButton.AttachNode(this);

        List<BaseSearchInfoNode<T>> newOptions = [];
        foreach (var newOptionNode in SearchOptions.Select(BuildOptionNode)) {
            newOptionNode.Size = new Vector2(listNode.ContentWidth, 48.0f);
            newOptionNode.OnClick = OnOptionClicked;
            newOptions.Add(newOptionNode);
        }
        
        listNode.AddNode(newOptions);

        listNode.RecalculateLayout();

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
            SelectionResult?.Invoke(selectedOption.Option);
        }
        
        selectedOption = null;
        Close();
    }

    protected abstract BaseSearchInfoNode<T> BuildOptionNode(T option);

    private void OnOptionClicked(SelectableNode clickedOption) {
        if (confirmButton is null) return;
        
        // Unselect Previous Option
        selectedOption?.IsHovered = false;
        selectedOption?.IsSelected = false;
        selectedOption = null;

        // Select New Option
        selectedOption = (BaseSearchInfoNode<T>) clickedOption;
        selectedOption.IsSelected = true;

        // Enable Confirm Button
        confirmButton.IsEnabled = true;
    }

    private void OnSortOrderUpdated(string sortingString, bool reversed) => listNode?.ReorderNodes((x, y) => {
        if (x is not BaseSearchInfoNode<T> left || y is not BaseSearchInfoNode<T> right) return 0;

        return left.Compare(right, sortingString, reversed);
    });

    private void OnSearchUpdated(string searchString) {
        if (listNode is null) return;

        foreach (var node in listNode.GetNodes<BaseSearchInfoNode<T>>()) {
            node.IsVisible = node.IsMatch(searchString);
        }

        selectedOption?.IsSelected = false;

        selectedOption = null;
        
        listNode.RecalculateLayout();
    }

    public required List<string> SortingOptions { get; init; }
    public required List<T> SearchOptions { get; init; }

    public Action<T>? SelectionResult { get; set; }
}
