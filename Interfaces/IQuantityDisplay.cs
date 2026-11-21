using Lumina.Text.ReadOnly;

namespace KamiToolKit.Interfaces;

/// <summary>
/// Interface representing a quantity display.
/// </summary>
public interface IQuantityDisplay {

    /// <summary>
    /// Gets or sets the quantity value.
    /// </summary>
    /// <remarks>
    /// Value is actually stores as a string, this incurs parsing costs.
    /// </remarks>
    int Quantity { get; set; }

    /// <summary>
    /// Gets or sets the quantity text value.
    /// </summary>
    ReadOnlySeString QuantityString { get; set; }
}
