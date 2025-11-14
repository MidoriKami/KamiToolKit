using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using KamiToolKit.Addons.Interfaces;
using KamiToolKit.Nodes;
using KamiToolKit.Widgets;

namespace KamiToolKit.Addons.Parts;

public class ModifyListNode<T> : SimpleComponentNode where T : class, IInfoNodeData {
    private SearchWidget searchWidget;
    private ScrollingAreaNode<VerticalListNode> listNode;

    private TextButtonNode addButton;
    private TextButtonNode editButton;
    private TextButtonNode removeButton;

    private BaseSearchInfoNode<T>? selectedOptionNode;

    public ModifyListNode() {
        searchWidget = new SearchWidget {
            OnSortOrderChanged = OnSortOrderChanged,
            OnSearchUpdated = OnSearchUpdated,
        };
        searchWidget.AttachNode(this);

        listNode = new ScrollingAreaNode<VerticalListNode> {
            ContentHeight = 100.0f,
            AutoHideScrollBar = true,
        };
        listNode.ContentNode.FitContents = true;
        listNode.AttachNode(this);

        addButton = new TextButtonNode {
            String = "Add",
            OnClick = OnAddClicked,
        };
        addButton.AttachNode(this);

        editButton = new TextButtonNode {
            IsEnabled = false,
            String = "Edit",
            OnClick = OnEditClicked,
        };
        editButton.AttachNode(this);
        
        removeButton = new TextButtonNode {
            IsEnabled = false,
            String = "Remove",
            OnClick = OnRemoveClicked,
        };
        removeButton.AttachNode(this);
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        searchWidget.Size = new Vector2(Width, 38.0f);
        searchWidget.Position = Vector2.Zero;
        
        listNode.Size = new Vector2(Width, Height - searchWidget.Height - 40.0f);
        listNode.Position = new Vector2(0.0f, searchWidget.Y + searchWidget.Height + 8.0f);

        if (AddNewEntry is null && EditEntry is null && RemoveEntry is null) {
            listNode.Height += 24.0f;
        }

        addButton.IsVisible = AddNewEntry is not null;
        editButton.IsVisible = EditEntry is not null;
        removeButton.IsVisible = RemoveEntry is not null;

        const float buttonPadding = 5.0f;
        var buttonWidth = ( Width - buttonPadding * 2.0f ) / 3.0f;

        addButton.Size = new Vector2(buttonWidth, 24.0f);
        addButton.Position = new Vector2(0.0f, Height - 24.0f);
        
        editButton.Size = new Vector2(buttonWidth, 24.0f);
        editButton.Position = new Vector2(buttonWidth + buttonPadding, Height - 24.0f);
        
        removeButton.Size = new Vector2(buttonWidth, 24.0f);
        removeButton.Position = new Vector2(buttonWidth * 2.0f + buttonPadding * 2.0f, Height - 24.0f);
    }

    private void OnSortOrderChanged(string sortingString, bool reversed) {
        listNode.ContentNode.ReorderNodes((x, y) => {
            if (x is not SearchInfoNode<T> left || y is not SearchInfoNode<T> right) return 0;

            return left.Compare(right, sortingString, reversed);
        });
    }

    private void OnSearchUpdated(string searchString) {
        if (selectedOptionNode is not null) {
            selectedOptionNode.IsSelected = false;
        }

        selectedOptionNode = null;
        OnOptionChanged?.Invoke(null);
        
        foreach (var option in listNode.ContentNode.GetNodes<SearchInfoNode<T>>()) {
            option.IsVisible = option.IsMatch(searchString);
        }

        listNode.ContentNode.RecalculateLayout();
        listNode.ContentHeight = listNode.ContentNode.Height;
    }

    private void OnAddClicked()
        => AddNewEntry?.Invoke(this);

    public void AddOption(T option) {
        SelectionOptions.Add(option);
        UpdateList();

        var newOptionNode = listNode.ContentNode.GetNodes<SearchInfoNode<T>>().FirstOrDefault(node => node.Option == option);
        if (newOptionNode is not null) {
            OnOptionClicked(newOptionNode);
        }
    }

    private void OnEditClicked() {
        if (selectedOptionNode?.Option is null) return;

        EditEntry?.Invoke(selectedOptionNode.Option);
    }

    private void OnRemoveClicked() {
        if (selectedOptionNode?.Option is null) return;

        var option = selectedOptionNode.Option;
        SelectionOptions.Remove(selectedOptionNode.Option);

        selectedOptionNode.IsSelected = false;
        selectedOptionNode = null;
        
        removeButton.IsEnabled = selectedOptionNode is not null;
        editButton.IsEnabled = selectedOptionNode is not null;

        RemoveEntry?.Invoke(option);
        UpdateList();

        OnOptionChanged?.Invoke(null);
    }

    private void OnOptionClicked(BaseSearchInfoNode<T> optionNode) {
        if (selectedOptionNode is not null) {
            selectedOptionNode.IsSelected = false;
        }

        selectedOptionNode = optionNode;
        selectedOptionNode.IsSelected = true;

        removeButton.IsEnabled = selectedOptionNode is not null;
        editButton.IsEnabled = selectedOptionNode is not null;

        OnOptionChanged?.Invoke(optionNode.Option);
    }

    public void UpdateList() {
        listNode.ContentNode.SyncWithListData(SelectionOptions, node => node.Option, data => new SearchInfoNode<T> {
            Size = new Vector2(listNode.ContentNode.Width, 48.0f),
            OnClicked = OnOptionClicked,
            Option = data,
        });

        listNode.ContentNode.RecalculateLayout();
        listNode.ContentHeight = listNode.ContentNode.Height;

        if (SortOptions?.Count > 0) {
            OnSortOrderChanged(SortOptions.First(), false);
        }

        foreach (var node in listNode.ContentNode.GetNodes<BaseSearchInfoNode<T>>()) {
            node.Size = new Vector2(listNode.ContentNode.Width, 48.0f);
            node.Refresh();
        }
    }

    public Action<ModifyListNode<T>>? AddNewEntry { 
        get;
        set {
            field = value;
            OnSizeChanged();
        }
    }

    public Action<T>? RemoveEntry { 
        get;
        set {
            field = value;
            OnSizeChanged();
        }
    }

    public Action<T>? EditEntry { 
        get;
        set {
            field = value;
            OnSizeChanged();
        }
    }
    
    public Action<T?>? OnOptionChanged { get; init; } 

    public List<string>? SortOptions {
        get => searchWidget.SortingOptions;
        set {
            searchWidget.SortingOptions = value ?? [];
            OnSizeChanged();

            if (value is not null && value.Count > 0) {
                OnSortOrderChanged(value.First(), false);
            }
        }
    }

    public List<T> SelectionOptions {
        get;
        set {
            field = value;
            UpdateList();
        }
    } = [];

    public T? SelectedOption => selectedOptionNode?.Option;
}
