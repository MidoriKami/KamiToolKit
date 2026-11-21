using KamiToolKit.Enums;

namespace KamiToolKit.Interfaces;

/// <summary>
/// Represents an element that can fill its parent's layout.
/// </summary>
public interface ILayoutFiller {

    /// <summary>
    /// Gets or sets how this element fills its parent.
    /// </summary>
    LayoutFill LayoutFill { get; set; }
}
