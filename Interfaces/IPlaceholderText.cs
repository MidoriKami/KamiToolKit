using Lumina.Data.Parsing.Uld;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Interfaces;

/// <summary>
/// Interface representing something with placeholder text.
/// </summary>
public interface IPlaceholderText {

    /// <summary>
    /// Gets or sets whether the placeholder string should be shown.
    /// </summary>
    bool ShowPlaceholderString { get; set; }

    /// <summary>
    /// Gets or sets the placeholder string to be shown.
    /// </summary>
    ReadOnlySeString PlaceholderString { get; set; }

    /// <summary>
    /// TextId of the placeholder string to load from <see cref="PlaceHolderStringSheetType"/>.
    /// </summary>
    uint PlaceholderStringTextId { get; set; }

    /// <summary>
    /// Gets or sets which data sheet to use to resolve <see cref="PlaceholderStringTextId"/>.
    /// </summary>
    NodeData.SheetType PlaceHolderStringSheetType { get; set; }
}
