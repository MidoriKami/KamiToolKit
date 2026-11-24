using System;

namespace KamiToolKit.Core.Interfaces.Nodes;

/// <summary>
/// Interface representing elements that can be selected via navigation input.
/// </summary>
public interface INavSelectable {

    /// <summary>
    /// Action that is invoked when this node is selected via Return or OK.
    /// </summary>
    Action? OnSelected { get; set; }
}
