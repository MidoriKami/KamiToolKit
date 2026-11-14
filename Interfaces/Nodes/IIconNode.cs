using KamiToolKit.Enums;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Interfaces;

/// <summary>
/// Interface representing an icon node.
/// </summary>
public interface IIconNode : IIcon {

    /// <summary>
    /// Gets whether the icon is in the process of loading.
    /// </summary>
    bool IsIconLoading { get; }

    /// <summary>
    /// Gets or sets the combo level of the icon.
    /// </summary>
    byte ComboLevel { get; set; }

    /// <summary>
    /// Gets or sets whether this icon represents a macro.
    /// </summary>
    bool IsMacro { get; set; }

    /// <summary>
    /// Gets or sets whether this icon represents a recipe.
    /// </summary>
    bool IsRecipe { get; set; }

    /// <summary>
    /// Gets whether this icon is being dragged.
    /// </summary>
    bool IsBeingDragged { get; }

    /// <summary>
    /// Gets or sets the displayed string where resource costs go.
    /// </summary>
    ReadOnlySeString ResourceCostString { get; set; }

    /// <summary>
    /// Gets or sets the numeric value displayed as a resource cost.
    /// </summary>
    uint ResourceCostValue { get; set; }

    /// <summary>
    /// Gets or sets the resource cost nodes visibility.
    /// </summary>
    bool ResourceCostVisible { get; set; }

    /// <summary>
    /// Gets or sets the value used to indicate current charges.
    /// </summary>
    int ChargeCount { get; set; }

    /// <summary>
    /// Gets or sets whether the charge count image should be shown.
    /// </summary>
    bool ChargeCountVisible { get; set; }

    /// <summary>
    /// Gets or sets the radial fade used to indicate charge percentage.
    /// </summary>
    float ChargePercent { get; set; }

    /// <summary>
    /// Gets or sets the string shown for the cooldown seconds.
    /// </summary>
    ReadOnlySeString CooldownSecondsString { get; set; }

    /// <summary>
    /// Gets or sets the displayed cooldown time.
    /// </summary>
    uint CooldownSeconds { get; set; }

    /// <summary>
    /// Gets or sets the visibility of the cooldown text node.
    /// </summary>
    bool CooldownSecondsVisible { get; set; }

    /// <summary>
    /// Gets or sets the cooldown percent.
    /// </summary>
    float CooldownPercent { get; set; }

    /// <summary>
    /// Gets or sets whether the cooldown percent radial should be shown.
    /// </summary>
    bool CooldownPercentVisible { get; set; }

    /// <summary>
    /// Gets or sets the current cost text color.
    /// </summary>
    CostTextColor CostTextColor { get; set; }

    /// <summary>
    /// Gets or sets if this icon needs to show as currently invalid.
    /// </summary>
    bool IsInvalid { get; set; }

    /// <summary>
    /// Gets or sets if the macro icon should be visible.
    /// </summary>
    bool ShowMacroIcon { get; set; }

    /// <summary>
    /// Gets or sets whether this icon should be faded out.
    /// </summary>
    bool IsFaded { get; set; }

    /// <summary>
    /// Enables or disables ants for this node.
    /// </summary>
    bool IsAnts { get; set; }
}
