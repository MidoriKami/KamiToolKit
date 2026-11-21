using System;

namespace KamiToolKit.Interfaces;

/// <summary>
/// Interface representing a numeric input node.
/// </summary>
public interface INumericInputNode : IValueChangeable<int>, IBoundedRange<int> {

    /// <summary>
    /// Gets the bounded range.
    /// </summary>
    Range Range { get; set; }
}
