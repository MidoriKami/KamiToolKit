using System;
using System.Collections.Generic;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Addon;
using KamiToolKit.Addons.Interfaces;
using KamiToolKit.Addons.Parts;
using KamiToolKit.Classes;
using KamiToolKit.Nodes;

namespace KamiToolKit.Addons;

public class ListConfigAddon<T, TU> : NativeAddon where TU : ConfigNode<T>, new() where T : class, IInfoNodeData {

    private ModifyListNode<T>? selectionListNode;
    private VerticalLineNode? separatorLine;
    private TU? configNode;
    private TextNode? nothingSelectedTextNode;

    protected override unsafe void OnSetup(AtkUnitBase* addon) {
        selectionListNode = new ModifyListNode<T> {
            Position = ContentStartPosition,
            Size = new Vector2(250.0f, ContentSize.Y),
            SortOptions = SortOptions,
            SelectionOptions = Options,
            OnOptionChanged = OnOptionChanged,
            AddNewEntry = OnAddClicked,
            RemoveEntry = OnItemRemoved,
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

        configNode = new TU {
            Position = ContentStartPosition + new Vector2(250.0f + 16.0f, 0.0f),
            Size = ContentSize - new Vector2(250.0f + 16.0f, 0.0f),
            OnConfigChanged = option => {
                OnConfigChanged?.Invoke(option);
                selectionListNode.UpdateList();
            },
            IsVisible = false,
        };
        configNode.AttachNode(this);
    }

    private void OnOptionChanged(T? newOption) {
        if (configNode is null) return;

        configNode.IsVisible = newOption is not null;
        if (nothingSelectedTextNode is not null) {
            nothingSelectedTextNode.IsVisible = newOption is null;
        }

        configNode.ConfigurationOption = newOption;
    }

    /// <summary>
    /// Optional, if this is set, a sorting dropdown and reverse button will appear.
    /// </summary>
    public List<string>? SortOptions { get; init; }

    /// <summary>
    /// Note: Setting new values will not be shown until the window is reopened.
    /// </summary>
    public required List<T> Options { get; set; } = [];

    public required Action<T>? OnConfigChanged { get; init; }
    public Action<ModifyListNode<T>>? OnAddClicked { get; init; }
    public Action<T>? OnItemRemoved { get; init; }
}
