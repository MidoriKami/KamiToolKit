using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using KamiToolKit.Nodes;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Premade.Widgets;

/// <summary>
/// Represents a search element that has a searchbar, and a dropdown for reordering elements.
/// </summary>
public unsafe class SearchWidget : SimpleComponentNode {
    public readonly TextInputNode InputNode;
    public readonly TextDropDownNode SortOrderDropDown;
    public readonly CircleButtonNode ReverseButtonNode;

    public bool IsReversed { get; private set; }
    public string SearchText { get; private set; } = string.Empty;
    public string SortMode { get; private set; } = string.Empty;

    public delegate void SearchUpdated(string searchString);
    public delegate void SortUpdated(string sortingString, bool reversed);

    public SearchWidget() {
        InputNode = new TextInputNode {
            PlaceholderString = "Search . . .",
            SeString = SearchText,
            OnInputReceived = SearchTextChanged,
        };
        InputNode.AttachNode(this);

        SortOrderDropDown = new TextDropDownNode {
            MaxListOptions = 0,
            Options = [],
            IsVisible = false,
            SelectedOption = SortMode == string.Empty ? null : SortMode,
            OnOptionSelected = DropDownChanged,
        };
        SortOrderDropDown.AttachNode(this);

        ReverseButtonNode = new CircleButtonNode {            
            Icon = ButtonIcon.Sort,
            OnClick = OnReverseButtonClicked,
            TextTooltip = "Reverse Sort Direction",
            IsVisible = false,
        };
        ReverseButtonNode.AttachNode(this);

        ResNode->SetHeight(38);
    }

    public required SortUpdated OnSortOrderChanged { get; set; }

    private void OnReverseButtonClicked() {
        IsReversed = !IsReversed;
        OnSortOrderChanged(SortMode, IsReversed);
    }

    private void DropDownChanged(string newOption) {
        SortMode = newOption;
        OnSortOrderChanged(SortMode, IsReversed);
    }

    public required SearchUpdated OnSearchUpdated { get; set; }

    private void SearchTextChanged(ReadOnlySeString newSearchString) {
        SearchText = newSearchString.ToString();
        OnSearchUpdated(SearchText);
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

    public List<string> SortingOptions {
        get => SortOrderDropDown.Options ?? [];
        set {
            SortOrderDropDown.Options = value;
            SortOrderDropDown.MaxListOptions = value.Count / 2 + 1;
            SortOrderDropDown.IsVisible = value.Count > 0;
            ReverseButtonNode.IsVisible = value.Count > 0;
            
            ResNode->SetHeight((ushort)(value.Count > 0 ? 69 : 38));

            if (SortingOptions.Count > 0) {
                SortMode = value.First();
            }
        }
    }

    public string? SelectedOption => SortOrderDropDown.SelectedOption;
}
