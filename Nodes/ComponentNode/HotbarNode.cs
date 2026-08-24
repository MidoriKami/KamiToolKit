
namespace KamiToolKit.Nodes;

/// <summary>
/// Specialization of <see cref="DragDropNode"/> that has handy accessors for things used to represent a hotbar slot.
/// </summary>
public class HotbarNode : DragDropNode {

    /// <summary>
    /// Gets or sets the primary resource cost text.
    /// </summary>
    public uint ResourceCost {
        get;
        set {
            field = value;
            IconNode.IconExtras.ResourceCostTextNode.String = value.ToString();
        }
    }

    /// <summary>
    /// Gets or sets whether the resource cost node should be visible.
    /// </summary>
    public bool ShowResourceCost {
        get => IconNode.IconExtras.ResourceCostTextNode.IsVisible;
        set => IconNode.IconExtras.ResourceCostTextNode.IsVisible = value;
    }

    /// <summary>
    /// Gets or sets the charge count.
    /// </summary>
    public uint ChargeCount {
        get;
        set {
            field = value;
            IconNode.IconExtras.ChargeCountImageNode.PartId = value;
        }
    }

    /// <summary>
    /// Gets or sets the charge cooldown percentage.
    /// </summary>
    public float ChargePercent {
        get;
        set {
            field = value;
            IconNode.IconExtras.AlternateCooldownNode.CooldownImage.PartId = (uint) (value * 80 + 81);
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the charge count node should be shown.
    /// </summary>
    public bool ShowChargeCount {
        get;
        set {
            field = value;
            IconNode.IconExtras.AlternateCooldownNode.IsVisible = value;
            IconNode.IconExtras.ChargeCountImageNode.IsVisible = value;
        }
    }

    /// <summary>
    /// Gets or sets the cooldown percent background.
    /// </summary>
    public uint CooldownSeconds {
        get;
        set {
            field = value;
            IconNode.IconExtras.CooldownTextNode.String = value.ToString();
        }
    }

    /// <summary>
    /// Gets or sets whether the cooldown text should be shown.
    /// </summary>
    public bool ShowCooldownSeconds {
        get;
        set {
            field = value;
            IconNode.IconExtras.CooldownTextNode.IsVisible = value;
        }
    }

    /// <summary>
    /// Gets or sets the cooldown percentage.
    /// </summary>
    public float CooldownPercent {
        get;
        set {
            field = value;
            IconNode.IconExtras.CooldownNode.CooldownImage.PartId = (uint) (value * 80);
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the cooldown progress should display.
    /// </summary>
    public bool ShowCooldownPercent {
        get;
        set {
            field = value;
            IconNode.IconExtras.CooldownNode.GlossyImageFrame.IsVisible = !value;
            IconNode.IconExtras.CooldownNode.CooldownImage.IsVisible = value;
        }
    }
}
