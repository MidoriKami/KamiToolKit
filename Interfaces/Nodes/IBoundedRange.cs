namespace KamiToolKit.Interfaces;

/// <summary>
/// Interface representing something with a bounded range.
/// </summary>
public interface IBoundedRange<T> {

    /// <summary>
    /// Gets or sets the min value for this range.
    /// </summary>
    T Min { get; set; }

    /// <summary>
    /// Gets or sets the max value for this range.
    /// </summary>
    T Max { get; set; }

    /// <summary>
    /// Gets or sets the step amount.
    /// </summary>
    T Step { get; set; }
}
