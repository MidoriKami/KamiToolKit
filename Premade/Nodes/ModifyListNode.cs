using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using KamiToolKit.Nodes;
using KamiToolKit.Premade.Widgets;

namespace KamiToolKit.Premade.Nodes;

public class ModifyListNode<T> : SimpleComponentNode where T : class, IInfoNodeData {
    private SearchWidget searchWidget;
    private ScrollingListNode listNode;

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

        listNode = new ScrollingListNode {
            AutoHideScrollBar = true,
            FitContents = true,
        };
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
        var buttonWidth = (Width - buttonPadding * 2.0f) / 3.0f;

        addButton.Size = new Vector2(buttonWidth, 24.0f);
        addButton.Position = new Vector2(0.0f, Height - 24.0f);

        editButton.Size = new Vector2(buttonWidth, 24.0f);
        editButton.Position = new Vector2(buttonWidth + buttonPadding, Height - 24.0f);

        removeButton.Size = new Vector2(buttonWidth, 24.0f);
        removeButton.Position = new Vector2(buttonWidth * 2.0f + buttonPadding * 2.0f, Height - 24.0f);
    }

    private void OnSortOrderChanged(string sortingString, bool reversed) {
        listNode.ReorderNodes((x, y) => {
            if (x is not SearchInfoNode<T> left || y is not SearchInfoNode<T> right) return 0;
            return left.Compare(right, sortingString, reversed);
        });
    }

    private void OnSearchUpdated(string searchString) {
        selectedOptionNode?.IsSelected = false;

        selectedOptionNode = null;
        removeButton.IsEnabled = false;
        editButton.IsEnabled = false;

        OnOptionChanged?.Invoke(null);

        foreach (var option in listNode.GetNodes<SearchInfoNode<T>>()) {
            option.IsVisible = option.IsMatch(searchString);
        }

        listNode.RecalculateLayout();
    }

    private void OnAddClicked()
        => AddNewEntry?.Invoke(this);

    public void AddOption(T option) {
        SelectionOptions.Add(option);
        UpdateList();

        var newOptionNode = listNode.GetNodes<SearchInfoNode<T>>().FirstOrDefault(node => ReferenceEquals(node.Option, option));
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

        removeButton.IsEnabled = false;
        editButton.IsEnabled = false;

        RemoveEntry?.Invoke(option);
        UpdateList();

        OnOptionChanged?.Invoke(null);
    }

    private void OnOptionClicked(SelectableNode optionNode) {
        selectedOptionNode?.IsSelected = false;

        selectedOptionNode = (BaseSearchInfoNode<T>) optionNode;
        selectedOptionNode.IsSelected = true;

        removeButton.IsEnabled = true;
        editButton.IsEnabled = true;

        OnOptionChanged?.Invoke(selectedOptionNode.Option);
    }

    public void UpdateList() {
        var previouslySelectedOption = selectedOptionNode?.Option;

        if (selectedOptionNode is not null) {
            selectedOptionNode.IsSelected = false;
            selectedOptionNode = null;
        }

        removeButton.IsEnabled = false;
        editButton.IsEnabled = false;

        listNode.SyncWithListData(
            SelectionOptions,
            node => node.Option,
            data => new SearchInfoNode<T> {
                Size = new Vector2(listNode.ContentWidth, 48.0f),
                OnClick = OnOptionClicked,
                Option = data,
            });

        listNode.RecalculateLayout();

        if (SortOptions?.Count > 0) {
            OnSortOrderChanged(SortOptions.First(), false);
        }

        if (previouslySelectedOption is not null) {
            var restoredNode = listNode
                .GetNodes<SearchInfoNode<T>>()
                .FirstOrDefault(n => ReferenceEquals(n.Option, previouslySelectedOption));

            if (restoredNode is not null) {
                selectedOptionNode = restoredNode;
                selectedOptionNode.IsSelected = true;
                removeButton.IsEnabled = true;
                editButton.IsEnabled = true;
            }
            else {
                OnOptionChanged?.Invoke(null);
            }
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
