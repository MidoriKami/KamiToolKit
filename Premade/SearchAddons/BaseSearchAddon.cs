using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;
using KamiToolKit.Premade.Widgets;

namespace KamiToolKit.Premade.SearchAddons;

public abstract class BaseSearchAddon<T, TU> : NativeAddon where TU : ListItemNode<T>, new() {
    
    private SearchWidget? searchWidget;
    private ListNode<T, TU>? listNode;

    private TextButtonNode? cancelButton;
    private TextButtonNode? confirmButton;

    private T? selectedOption;

    protected override unsafe void OnSetup(AtkUnitBase* addon) {
        searchWidget = new SearchWidget {
            Size = ContentSize,
            Position = ContentStartPosition,
            SortingOptions = SortingOptions,
            OnSortOrderChanged = OnSortOrderUpdated,
            OnSearchUpdated = OnSearchUpdated,
        };
        searchWidget.AttachNode(this);

        listNode = new ListNode<T, TU> {
            Position = new Vector2(ContentStartPosition.X, searchWidget.Y + searchWidget.Height + 8.0f),
            Size = new Vector2(ContentSize.X, ContentSize.Y - searchWidget.Height - 16.0f - 24.0f - 8.0f),
            ItemSpacing = 6.0f,
            OptionsList = SearchOptions,
            OnItemSelected = item => {
                selectedOption = item;
                confirmButton?.IsEnabled = true;
            },
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

        if (SortingOptions.Count > 0) {
            OnSortOrderUpdated(SortingOptions.First(), false);
        }
    }

    private void OnCancelClicked() {
        selectedOption = default;
        Close();
    }

    private void OnConfirmClicked() {
        if (selectedOption is not null) {
            SelectionResult?.Invoke(selectedOption);
        }
        
        selectedOption = default;
        Close();
    }

    private void OnSortOrderUpdated(string sortingString, bool reversed) {
        var resortedList = SearchOptions.ToList();
        resortedList.Sort((left, right) => Comparer(left, right, sortingString, reversed));

        listNode?.OptionsList = resortedList;
    }
    
    private void OnSearchUpdated(string searchString) {
        listNode?.OptionsList = SearchOptions.Where(item => IsMatch(item, searchString)).ToList();
    }

    protected abstract int Comparer(T left, T right, string sortingString, bool reversed);
    protected abstract bool IsMatch(T item, string searchString);

    public List<string> SortingOptions { get; init; } = [ "Alphabetical", "Id" ];

    public List<T> SearchOptions {
        get;
        set {
            field = value;
            listNode?.OptionsList = value;
        }
    } = [];

    public Action<T>? SelectionResult { get; set; }
}
