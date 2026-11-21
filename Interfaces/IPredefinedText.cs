using Lumina.Data.Parsing.Uld;

namespace KamiToolKit.Interfaces;

/// <summary>
/// Interface representing nodes that can be pre-defined using a TextId and SheetType.
/// </summary>
public interface IPredefinedText {

    /// <summary>
    /// Gets or sets the text id that reads a label from the datasheets instead.
    /// </summary>
    uint TextId { get; set; }

    /// <summary>
    /// Gets or sets which datasheet should be used to resolve <see cref="TextId"/>
    /// </summary>
    NodeData.SheetType SheetType { get; set; }
}
