using System;

namespace KamiToolKit.Interfaces;

/// <summary>
/// Interface representing a value that can be changed.
/// </summary>
public interface IValueChangeable<T> {

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    T Value { get; set; }

    /// <summary>
    /// Gets or sets the action that is invoked when the value is updated.
    /// </summary>
    Action<T>? OnValueUpdate { get; set; }
}
