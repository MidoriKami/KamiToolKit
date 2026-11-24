namespace KamiToolKit.Core.Layout;

/// <summary>
/// Defines an interface that allows the inheritor to have its layout updated.
/// </summary>
public interface ILayoutUpdatable {

    /// <summary>
    /// Updates the contained layout, and controller navigation values if applicable.
    /// </summary>
    void UpdateLayout();
}
