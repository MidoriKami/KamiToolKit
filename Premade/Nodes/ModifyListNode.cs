using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Utility;
using KamiToolKit.Nodes;
using KamiToolKit.Premade.Widgets;

namespace KamiToolKit.Premade.Nodes;

/// <summary>
/// A non-owning list node that supports searching, and various callbacks for easily editing a list.
/// </summary>
/// <typeparam name="T">Data type to display the data for.</typeparam>
/// <typeparam name="TU">ListItemNode derived type, for defining the result view.</typeparam>
public class ModifyListNode<T, TU> : SimpleComponentNode where TU : ListItemNode<T>, new() {
    private readonly SearchWidget searchWidget;
    private readonly ListNode<T, TU> listNode;

    private readonly TextButtonNode addButton;
    private readonly TextButtonNode editButton;
    private readonly TextButtonNode removeButton;

    public ModifyListNode() {
        searchWidget = new SearchWidget {
            OnSortOrderChanged = OnSortOrderChanged,
            OnSearchUpdated = OnSearchUpdated,
        };
        searchWidget.AttachNode(this);

        listNode = new ListNode<T, TU> {
            OptionsList = [],
            OnItemSelected = OnListItemSelected,
        };
        listNode.AttachNode(this);

        addButton = new TextButtonNode {
            String = "Add",
            OnClick = OnAddClicked,
            IsEnabled = false,
        };
        addButton.AttachNode(this);

        editButton = new TextButtonNode {
            String = "Edit",
            OnClick = OnEditClicked,
            IsEnabled = false,
        };
        editButton.AttachNode(this);

        removeButton = new TextButtonNode {
            String = "Remove",
            OnClick = OnRemoveClicked,
            IsEnabled = false,
        };
        removeButton.AttachNode(this);
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        searchWidget.Size = new Vector2(Width, 65.0f);
        searchWidget.Position = Vector2.Zero;

        listNode.Size = new Vector2(Width, Height - searchWidget.Height - 40.0f);
        listNode.Position = new Vector2(0.0f, searchWidget.Y + searchWidget.Height + 8.0f);

        const float buttonPadding = 5.0f;
        var buttonWidth = (Width - buttonPadding * 2.0f) / 3.0f;

        addButton.Size = new Vector2(buttonWidth, 24.0f);
        addButton.Position = new Vector2(0.0f, Height - 24.0f);

        editButton.Size = new Vector2(buttonWidth, 24.0f);
        editButton.Position = new Vector2(buttonWidth + buttonPadding, Height - 24.0f);

        removeButton.Size = new Vector2(buttonWidth, 24.0f);
        removeButton.Position = new Vector2(buttonWidth * 2.0f + buttonPadding * 2.0f, Height - 24.0f);
    }
    
    public List<T> Options {
        get;
        set {
            field = value;
            listNode.OptionsList = value;
        }
    } = [];

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

    public Action<T?>? SelectionChanged { get; init; }

    public Action? AddNewEntry {
        get;
        set {
            field = value; 
            addButton.IsEnabled = value is not null;
        }
    }

    public Action<T>? RemoveEntry {
        get;
        set {
            field = value; 
            removeButton.IsEnabled = value is not null && SelectedOption is not null;
        }
    }

    public Action<T>? EditEntry {
        get;
        set {
            field = value; 
            editButton.IsEnabled = value is not null && SelectedOption is not null;
        }
    }

    public delegate int ItemCompareDelegate(T left, T right, string sortingMode);
    public ItemCompareDelegate? ItemComparer { get; set; }
    
    public delegate bool IsSearchMatchDelegate(T obj, string searchString);
    public IsSearchMatchDelegate? IsSearchMatch { get; set; }

    public T? SelectedOption { get; private set; }

    public float ItemSpacing {
        get => listNode.ItemSpacing;
        set {
            listNode.ItemSpacing = value;
            OnSizeChanged();
        }
    }

    private void OnSortOrderChanged(string sortingString, bool reversed) {
        if (ItemComparer is null) return;

        var listCopy = Options.ToList();
        listCopy.Sort((left, right) => ItemComparer.Invoke(left, right, sortingString) * (reversed ? -1 : 1));
        listNode.OptionsList = listCopy;
        UpdateButtonStates();
    }
    
    private void OnSearchUpdated(string searchString) {
        if (IsSearchMatch is null) return;

        if (searchString.IsNullOrEmpty()) {
            listNode.OptionsList = Options;
        }
        else {
            listNode.OptionsList = Options.Where(item => IsSearchMatch(item, searchString)).ToList();
        }
    }
    
    private void OnListItemSelected(T? obj) {
        SelectedOption = obj;
        SelectionChanged?.Invoke(SelectedOption);

        UpdateButtonStates();
    }

    private void OnAddClicked() {
        AddNewEntry?.Invoke();
        RefreshList();
    }
    
    private void OnEditClicked() {
        if (SelectedOption is null) return;
        
        EditEntry?.Invoke(SelectedOption);
        RefreshList();
    }
    
    private void OnRemoveClicked() {
        if (SelectedOption is null) return;

        RemoveEntry?.Invoke(SelectedOption);
        RefreshList();
    }

    private void UpdateButtonStates() {
        editButton.IsEnabled = SelectedOption is not null && EditEntry is not null;
        removeButton.IsEnabled = SelectedOption is not null && RemoveEntry is not null;
    }

    /// <summary>
    /// Refreshes the displayed list data.
    /// This resets scroll position, so don't spam it.
    /// </summary>
    public void RefreshList() {
        OnSortOrderChanged(searchWidget.SortMode, searchWidget.IsReversed);
        OnSearchUpdated(searchWidget.SearchText);
        listNode.FullRebuild();
    }
}
