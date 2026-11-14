namespace KamiToolKit.Interfaces;

/// <summary>
/// Interface representing something that uses an icon.
/// </summary>
public interface IIcon {

    /// <summary>
    /// Gets or sets the iconId used for the displayed icon.
    /// </summary>
    uint IconId { get; set; }

    /// <summary>
    /// Gets or sets if the displayed icon should be shown as disabled.
    /// </summary>
    bool IsIconDisabled { get; set; }
}
