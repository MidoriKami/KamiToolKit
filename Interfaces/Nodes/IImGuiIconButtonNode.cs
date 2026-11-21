namespace KamiToolKit.Interfaces;

/// <summary>
/// Interface representing an ImGui Icon Button Node.
/// </summary>
public interface IImGuiIconButtonNode : IButtonNode, IImGuiTexture {

    /// <summary>
    /// Gets or sets whether the border/background of the button should be shown.
    /// </summary>
    bool ShowBackground { get; set; }
}
