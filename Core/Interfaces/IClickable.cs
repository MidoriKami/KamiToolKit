using System;

namespace KamiToolKit.Core.Interfaces;

/// <summary>
/// Interface representing something that can be clicked on to handle a OnClick event.
/// </summary>
public interface IClickable {

    /// <summary>
    /// Action that is invoked when clicked.
    /// </summary>
    Action? OnClick { get; set; }
}
