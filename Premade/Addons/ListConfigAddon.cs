using System;
using System.Collections.Generic;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;
using KamiToolKit.Premade.Nodes;

namespace KamiToolKit.Premade.Addons;

public class ListConfigAddon<T, TU, TV> : NativeAddon where T: class where TV : ConfigNode<T>, new() where TU : ListItemNode<T>, new() {

    private ModifyListNode<T, TU>? selectionListNode;
    private VerticalLineNode? separatorLine;
    private TV? configNode;
    private TextNode? nothingSelectedTextNode;

    protected override unsafe void OnSetup(AtkUnitBase* addon) {
        selectionListNode = new ModifyListNode<T, TU> {
            Position = ContentStartPosition,
            Size = new Vector2(250.0f, ContentSize.Y),
            SortOptions = SortOptions,
            Options = Options,
            SelectionChanged = SelectionChanged,
            AddNewEntry = OnAddClicked,
            RemoveEntry = OnRemoveClicked,
            ItemComparer = ItemComparer,
            IsSearchMatch = OnSearchUpdated,
            ItemSpacing = ItemSpacing,
        };
        selectionListNode.AttachNode(this);

        separatorLine = new VerticalLineNode {
            Position = ContentStartPosition + new Vector2(250.0f + 8.0f, 0.0f),
            Size = new Vector2(4.0f, ContentSize.Y),
        };
        separatorLine.AttachNode(this);

        nothingSelectedTextNode = new TextNode {
            Position = ContentStartPosition + new Vector2(250.0f + 16.0f, 0.0f),
            Size = ContentSize - new Vector2(250.0f + 16.0f, 0.0f),
            AlignmentType = AlignmentType.Center,
            TextFlags = TextFlags.WordWrap | TextFlags.MultiLine,
            FontSize = 14,
            LineSpacing = 22,
            FontType = FontType.Axis,
            String = "Please select an option on the left",
            TextColor = ColorHelper.GetColor(1),
        };
        nothingSelectedTextNode.AttachNode(this);

        configNode = new TV {
            Position = ContentStartPosition + new Vector2(250.0f + 16.0f, 0.0f),
            Size = ContentSize - new Vector2(250.0f + 16.0f, 0.0f),
            OnConfigChanged = option => EditCompleted?.Invoke(option),
            IsVisible = false,
        };
        configNode.AttachNode(this);
    }

    public required ModifyListNode<T, TU>.ItemCompareDelegate? ItemComparer {
        get;
        init {
            field = value;
            selectionListNode?.ItemComparer = value;
        }
    }

    public required ModifyListNode<T, TU>.IsSearchMatchDelegate? IsSearchMatch {
        get;
        init {
            field = value;
            selectionListNode?.IsSearchMatch = value;
        }
    }

    private void OnAddClicked() {
        AddClicked?.Invoke(this);
        selectionListNode?.RefreshList();
    }
    
    private void OnRemoveClicked(T listItem) {
        RemoveClicked?.Invoke(this, listItem);
        SelectionChanged(null);
        selectionListNode?.RefreshList();
    }

    private void SelectionChanged(T? listItem) {
        SetConfigNodeItem(listItem);
    }

    private bool OnSearchUpdated(T obj, string searchString) {
        SelectItem(null);
        return IsSearchMatch?.Invoke(obj, searchString) ?? false;
    }
    
    private void SetConfigNodeItem(T? configItem) {
        if (configNode is null) return;
        if (nothingSelectedTextNode is null) return;

        configNode.ConfigurationOption = configItem;
        
        configNode.IsVisible = configNode.ConfigurationOption is not null;
        nothingSelectedTextNode.IsVisible = configNode.ConfigurationOption is null;
    }

    public void RefreshList()
        => selectionListNode?.RefreshList();

    public void SelectItem(T? listItem)
        => SelectionChanged(listItem);

    public List<string>? SortOptions {
        get;
        set {
            field = value;
            selectionListNode?.SortOptions = value;
        }
    } = ["Alphabetical", "Id"];

    public required List<T> Options { get;
        set {
            field = value;
            selectionListNode?.Options = value;
        } 
    } = [];

    public float ItemSpacing {
        get;
        set {
            field = value;
            selectionListNode?.ItemSpacing = value;
        }
    }

    public Action<ListConfigAddon<T, TU, TV>>? AddClicked {
        get;
        set {
            field = value;
            selectionListNode?.AddNewEntry = () => {
                value?.Invoke(this);
            };
        }
    }

    public Action<ListConfigAddon<T, TU, TV>, T>? RemoveClicked {
        get;
        set {
            field = value;
            selectionListNode?.RemoveEntry = entry => {
                value?.Invoke(this, entry);
            };
        }
    }

    public Action<T>? EditCompleted { get; set; }
}
