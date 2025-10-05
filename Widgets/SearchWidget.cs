using System.Collections.Generic;
using System.Numerics;
using Dalamud.Game.Text.SeStringHandling;
using KamiToolKit.Nodes;

namespace KamiToolKit.Widgets;

/// <summary>
/// Represents a search element that has a searchbar, and a dropdown for reordering elements.
/// </summary>
public unsafe class SearchWidget : SimpleComponentNode {
    public readonly TextInputNode InputNode;
    public readonly TextDropDownNode SortOrderDropDown;
    public readonly CircleButtonNode ReverseButtonNode;

    private bool reverseSort;
    private string searchText = string.Empty;
    private string sortOption = string.Empty;

    public delegate void SearchUpdated(string searchString);
    public delegate void SortUpdated(string sortingString, bool reversed);

    public SearchWidget() {
        InputNode = new TextInputNode {
            IsVisible = true,
            PlaceholderString = "Search . . .",
            SeString = searchText,
        };
        InputNode.AttachNode(this);

        SortOrderDropDown = new TextDropDownNode {
            MaxListOptions = 0,
            Options = [],
            IsVisible = false,
            SelectedOption = sortOption == string.Empty ? null : sortOption,
        };
        SortOrderDropDown.AttachNode(this);

        ReverseButtonNode = new CircleButtonNode {            
            Icon = ButtonIcon.Sort,
            OnClick = OnReverseButtonClicked,
            Tooltip = "Reverse Sort Direction",
            IsVisible = false,
        };
        ReverseButtonNode.AttachNode(this);

        InternalResNode->SetHeight(38);
    }

    public required SortUpdated OnSortOrderChanged {
        get;
        set {
            field = value;
            SortOrderDropDown.OnOptionSelected = DropDownChanged;
        }
    }

    private void OnReverseButtonClicked() {
        reverseSort = !reverseSort;
        OnSortOrderChanged(sortOption, reverseSort);
    }

    private void DropDownChanged(string newOption) {
        sortOption = newOption;
        OnSortOrderChanged(newOption, reverseSort);
    }

    public required SearchUpdated OnSearchUpdated {
        get;
        set {
            field = value;
            InputNode.OnInputReceived = SearchTextChanged;
        }
    }

    private void SearchTextChanged(SeString newSearchString) {
        searchText = newSearchString.ToString();
        OnSearchUpdated(searchText);
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        InputNode.Size = new Vector2(Width - 10.0f, 28.0f);
        InputNode.Position = new Vector2(5.0f, 5.0f);

        ReverseButtonNode.Size = new Vector2(28.0f, 28.0f);
        ReverseButtonNode.Position = new Vector2(Width - 5.0f - ReverseButtonNode.Width, InputNode.Height + 8.0f);

        SortOrderDropDown.Size = new Vector2(Width - 5.0f - ReverseButtonNode.Width - 5.0f - 5.0f, 28.0f);
        SortOrderDropDown.Position = new Vector2(5.0f, InputNode.Height + 8.0f);
    }

    // Disallow modifying the height of this element.
    public override float Height { get => base.Height; set { } }

    public int MaxDropdownOptions {
        get => SortOrderDropDown.MaxListOptions;
        set => SortOrderDropDown.MaxListOptions = value;
    }

    public List<string> FilterOptions {
        get => SortOrderDropDown.Options ?? [];
        set {
            SortOrderDropDown.Options = value;
            SortOrderDropDown.MaxListOptions = value.Count / 2 + 1;
            SortOrderDropDown.IsVisible = value.Count > 0;
            ReverseButtonNode.IsVisible = value.Count > 0;
            
            InternalResNode->SetHeight((ushort)(value.Count > 0 ? 69 : 38));
        }
    }

    public string? SelectedOption => SortOrderDropDown.SelectedOption;
}
