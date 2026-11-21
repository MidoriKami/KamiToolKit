namespace KamiToolKit.Interfaces;

/// <summary>
/// Represents a hold button node.
/// </summary>
public interface IHoldButtonNode : IButtonNode, ITextLabel {

    /// <summary>
    /// Gets or sets whether this button should act as a one-time use.
    /// If true, this button can be re-activated after clicking and holding again.
    /// </summary>
    bool UnlockAfterClick { get; set; }

    /// <summary>
    /// Resets the button params allowing it to be re-activated.
    /// </summary>
    void Reset();
}
