namespace KamiToolKit.Interfaces;

/// <summary>
/// Defines an interface that allows the inheritor to have its layout recalculated.
/// </summary>
public interface ILayoutUpdatable {

    /// <summary>
    /// Recalculates the contained layout, and controller navigation values if applicable.
    /// </summary>
    void RecalculateLayout();
}
