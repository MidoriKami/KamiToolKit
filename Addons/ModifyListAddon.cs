using System;
using System.Collections.Generic;
using System.Numerics;
using Dalamud.Game.Text.SeStringHandling;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Addon;
using KamiToolKit.Addons.Parts;
using KamiToolKit.Nodes;

namespace KamiToolKit.Addons;

public class ModifyListAddon<T> : NativeAddon {
    private TextInputNode? searchInputNode;
    private ScrollingAreaNode<VerticalListNode>? listNode;

    private TextButtonNode? addButton;
    private TextButtonNode? removeButton;

    private SearchInfoNode<T>? selectedOption;
    public required List<T> Options { get; set; }
    
    public required Action AddNewEntry { get; init; }
    public required Action<T> RemoveEntry { get; init; }

    public delegate OptionInfo<T> GetOptionInfoDelegate(T option);
    public required GetOptionInfoDelegate GetOptionInfo { get; init; }

    protected override unsafe void OnSetup(AtkUnitBase* addon) {
        selectedOption = null;

        searchInputNode = new TextInputNode {
            Position = ContentStartPosition + new Vector2(5.0f, 5.0f),
            Size = new Vector2(ContentSize.X - 10.0f, 28.0f),
            PlaceholderString = "Search . . .",
            OnInputReceived = OnInputReceived,
            IsVisible = true,
        };
        AttachNode(searchInputNode);

        listNode = new ScrollingAreaNode<VerticalListNode> {
            Position = new Vector2(ContentStartPosition.X, searchInputNode.Y + searchInputNode.Height + 8.0f),
            Size = new Vector2(ContentSize.X, ContentSize.Y - searchInputNode.Height - 16.0f - 24.0f - 8.0f),
            ContentHeight = 100.0f,
            IsVisible = true,
            AutoHideScrollBar = true,
        };
        listNode.ContentNode.FitContents = true;
        AttachNode(listNode);

        const float buttonPadding = 20.0f;
        var contentWidth = ContentSize.X - buttonPadding * 2;
        var buttonWidth = contentWidth / 3.0f;

        addButton = new TextButtonNode {
            Size = new Vector2(buttonWidth, 24.0f),
            Position = new Vector2(ContentStartPosition.X, ContentStartPosition.Y + ContentSize.Y - 24.0f - 8.0f),
            IsVisible = true,
            String = "Add",
            OnClick = OnAddClicked,
        };
        AttachNode(addButton);

        removeButton = new TextButtonNode {
            Size = new Vector2(buttonWidth, 24.0f),
            Position = new Vector2(ContentStartPosition.X + buttonWidth * 2 + buttonPadding * 2, ContentStartPosition.Y + ContentSize.Y - 24.0f - 8.0f),
            IsVisible = true,
            IsEnabled = false,
            String = "Remove",
            OnClick = OnRemoveClicked,
        };
        AttachNode(removeButton);
        
        listNode.ContentNode.SyncWithListData(Options, 
            node => node.OptionInfo.Option, 
            data => BuildOptionNode(GetOptionInfo(data)));

        listNode.ContentNode.RecalculateLayout();
        listNode.ContentHeight = listNode.ContentNode.Height;
    }

    public void ResyncOptions(List<T> options) {
        Options = options;

        if (listNode is not null && IsOpen) {
            listNode.ContentNode.SyncWithListData(Options, 
                node => node.OptionInfo.Option, 
                data => BuildOptionNode(GetOptionInfo(data)));
        }
    }

    private void OnInputReceived(SeString searchString) {
        if (listNode is null) return;

        if (selectedOption is not null) {
            selectedOption.IsSelected = false;
        }

        selectedOption = null;

        foreach (var node in listNode.ContentNode.GetNodes<SearchInfoNode<T>>()) {
            node.IsVisible = node.OptionInfo.ContainsSearchTerm(searchString.ToString());
        }

        listNode.ContentNode.RecalculateLayout();
        listNode.ContentHeight = listNode.ContentNode.Height;
    }

    private void OnAddClicked() {
        AddNewEntry.Invoke();
        
        if (removeButton is not null && listNode is not null) {
            removeButton.IsEnabled = listNode.ContentNode.Nodes.Count is not 0;
        }
    }

    private void OnRemoveClicked() {
        if (selectedOption is null) return;
        if (listNode is null) return;

        RemoveEntry(selectedOption.OptionInfo.Option);

        listNode.ContentNode.RemoveNode(selectedOption);
        selectedOption = null;

        listNode.ContentNode.RecalculateLayout();
        listNode.ContentHeight = listNode.ContentNode.Height;

        if (removeButton is not null) {
            removeButton.IsEnabled = listNode.ContentNode.Nodes.Count is not 0;
        }
    }
    
    private SearchInfoNode<T> BuildOptionNode(OptionInfo<T> option) => new() {
        Width = listNode!.ContentNode.Width,
        Height = 48.0f,
        OptionInfo = option,
        IsVisible = true,
        OnClicked = OnOptionClicked,
    };
    
    private void OnOptionClicked(SearchInfoNode<T> clickedOption) {
        if (removeButton is null) return;
        
        // Unselect Previous Option
        if (selectedOption is not null) {
            selectedOption.IsSelected = false;
            selectedOption = null;
        }

        // Select New Option
        selectedOption = clickedOption;
        selectedOption.IsSelected = true;

        // Enable Confirm Button
        removeButton.IsEnabled = true;
    }
}
