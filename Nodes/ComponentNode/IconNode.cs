using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.BaseTypes.ComponentNode;
using KamiToolKit.Enums;
using KamiToolKit.Timelines;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Nodes;

/// <summary>
/// Implementation of the games IconNode and associated component.
/// This is often used as a part in a <see cref="DragDropNode"/>, but is not required to be used as a part can be used by itself.
/// </summary>
public unsafe class IconNode : ComponentNode<AtkComponentIcon, AtkUldComponentDataIcon> {

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public IconExtras IconExtras { get; }

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public IconImageNode IconImage { get; }

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public IconIndicator IconIndicator1 { get; }

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public IconIndicator IconIndicator2 { get; }

    /// <summary>
    /// Gets or sets the displayed Icon via IconId.
    /// </summary>
    public uint IconId {
        get => Component->IconId;
        set => Component->LoadIcon(value);
    }

    /// <summary>
    /// Gets whether the icon is in the process of loading.
    /// </summary>
    public bool IsIconLoading
        => Component->Flags.HasFlag(IconComponentFlags.IsIconLoading);

    /// <summary>
    /// Gets or sets if the icon should be shown as disabled.
    /// </summary>
    /// <remarks>
    /// This doesn't affect the interactability of the component, just the display of the icon.
    /// </remarks>
    public bool IsIconDisabled {
        get => Component->Flags.HasFlag((IconComponentFlags) 0x2000);
        set => Component->SetIconImageDisableState(value);
    }

    /// <summary>
    /// Gets or sets the combo level of the icon.
    /// </summary>
    /// <remarks>
    /// This shows the hexagon numbers with I, II, and III respectively.
    /// Only seen with PvP abilities so far.
    /// </remarks>
    public byte ComboLevel {
        get {
            if (Component->Flags.HasFlag(IconComponentFlags.ComboLevel3))
                return 3;
            if (Component->Flags.HasFlag(IconComponentFlags.ComboLevel2))
                return 2;
            if (Component->Flags.HasFlag(IconComponentFlags.ComboLevel1))
                return 1;
            return 0;
        }
        set => Component->SetComboLevel(value is >= 1 and <= 3, (byte)(value - 1));
    }

    /// <summary>
    /// Gets or sets whether this icon represents a macro.
    /// </summary>
    public bool IsMacro {
        get => Component->Flags.HasFlag(IconComponentFlags.IsMacro);
        set => Component->SetIsMacro(value);
    }

    /// <summary>
    /// Gets or sets whether this icon represents a recipe.
    /// </summary>
    public bool IsRecipe {
        get => Component->Flags.HasFlag(IconComponentFlags.IsRecipe);
        set => Component->SetIsRecipe(value);
    }

    /// <summary>
    /// Gets whether this icon is being dragged.
    /// </summary>
    public bool IsBeingDragged
        => Component->Flags.HasFlag(IconComponentFlags.IsBeingDragged);

    /// <summary>
    /// Gets or sets the displayed string where resource costs go.
    /// </summary>
    public ReadOnlySeString ResourceCostString {
        get => IconExtras.ResourceCostString;
        set => IconExtras.ResourceCostString = value;
    }

    /// <summary>
    /// Gets or sets the numeric value displayed as a resource cost.
    /// </summary>
    public uint ResourceCostValue {
        get => IconExtras.ResourceCostValue;
        set => IconExtras.ResourceCostValue = value;
    }

    /// <summary>
    /// Gets or sets the resource cost nodes visibility.
    /// </summary>
    public bool ResourceCostVisible {
        get => IconExtras.ResourceCostVisible;
        set => IconExtras.ResourceCostVisible = value;
    }

    /// <summary>
    /// Gets or sets the value used to indicate current charges.
    /// </summary>
    public uint ChargeCount {
        get => IconExtras.ChargeCount;
        set => IconExtras.ChargeCount = value;
    }

    /// <summary>
    /// Gets or sets whether the charge count image should be shown.
    /// </summary>
    public bool ChargeCountVisible {
        get => IconExtras.ChargeCountVisible;
        set => IconExtras.ChargeCountVisible = value;
    }

    /// <summary>
    /// Gets or sets the radial fade used to indicate charge percentage.
    /// </summary>
    public float ChargePercent {
        get => IconExtras.ChargePercent;
        set => IconExtras.ChargePercent = value;
    }

    /// <summary>
    /// Gets or sets the string shown for the cooldown seconds.
    /// </summary>
    public ReadOnlySeString CooldownSecondsString {
        get => IconExtras.CooldownSecondsString;
        set => IconExtras.CooldownSecondsString = value;
    }

    /// <summary>
    /// Gets or sets the displayed cooldown time.
    /// </summary>
    public uint CooldownSeconds {
        get => IconExtras.CooldownSeconds;
        set => IconExtras.CooldownSeconds = value;
    }

    /// <summary>
    /// Gets or sets the visibility of the cooldown text node.
    /// </summary>
    public bool CooldownSecondsVisible {
        get => IconExtras.CooldownSecondsVisible;
        set => IconExtras.CooldownSecondsVisible = value;
    }

    /// <summary>
    /// Gets or sets the cooldown percent.
    /// </summary>
    public float CooldownPercent {
        get => IconExtras.CooldownPercent;
        set => IconExtras.CooldownPercent = value;
    }

    /// <summary>
    /// Gets or sets whether the cooldown percent radial should be shown, and if so also hides the glossy border.
    /// </summary>
    public bool CooldownPercentVisible {
        get => IconExtras.CooldownPercentVisible;
        set => IconExtras.CooldownPercentVisible = value;
    }

    /// <summary>
    /// Gets or sets the current cost text color.
    /// </summary>
    public CostTextColor CostTextColor {
        get => IconExtras.CostTextColor;
        set => IconExtras.CostTextColor = value;
    }

    /// <summary>
    /// Gets or sets if this icon needs to show as currently invalid.
    /// Red Text, and a Red Tinted Glossy Border.
    /// </summary>
    public bool IsInvalid {
        get => IconExtras.IsInvalid;
        set => IconExtras.IsInvalid = value;
    }

    /// <summary>
    /// Gets or sets if the macro icon should be visible.
    /// </summary>
    public bool ShowMacroIcon {
        get;
        set {
            field = value;

            if (value) {
                IconIndicator2.IconNode.PartId = 14;
            }

            IconIndicator2.IconNode.IsVisible = value;
        }
    }

    /// <summary>
    /// Gets or sets whether this icon should be faded out.
    /// </summary>
    public bool IsFaded {
        get;
        set {
            field = value;

            if (value) {
                IconImage.MultiplyColor = new Vector3(0.5f, 0.5f, 0.5f);
                IconExtras.ResourceCostTextNode.MultiplyColor = new Vector3(0.5f, 0.5f, 0.5f);
            }
            else {
                IconImage.MultiplyColor = new Vector3(1.0f, 1.0f, 1.0f);
                IconExtras.ResourceCostTextNode.MultiplyColor = new Vector3(1.0f, 1.0f, 1.0f);
            }
        }
    }

    /// <summary>
    /// Enables or disables ants for this node.
    /// </summary>
    public bool IsAnts {
        get => IconExtras.IsAnts;
        set => IconExtras.IsAnts = value;
    }

    /// <summary>
    /// Constructs a new <see cref="IconNode"/>.
    /// </summary>
    public IconNode() {
        SetInternalComponentType(ComponentType.Icon);

        IconImage = new IconImageNode {
            NodeId = 20,
            Size = new Vector2(40.0f, 40.0f),
            Position = new Vector2(2.0f, 3.0f),
            WrapMode = WrapMode.Tile,
            ImageNodeFlags = ImageNodeFlags.AutoFit,
        };
        IconImage.AttachNode(this);

        IconExtras = new IconExtras {
            NodeId = 6,
            Size = new Vector2(60, 60),
            Position = new Vector2(-2.0f, 0.0f),
        };
        IconExtras.AttachNode(this);

        IconIndicator1 = new IconIndicator(5) {
            NodeId = 4,
            Size = new Vector2(18.0f, 18.0f),
            Position = new Vector2(27.0f, 11.0f),
        };
        IconIndicator1.AttachNode(this);

        IconIndicator2 = new IconIndicator(3) {
            NodeId = 2,
            Size = new Vector2(18.0f, 18.0f),
            Position = new Vector2(27.0f, -2.0f),
        };
        IconIndicator2.AttachNode(this);

        BuildTimeline();

        Data->Nodes[0] = IconImage.NodeId;
        Data->Nodes[1] = IconExtras.CooldownNode.NodeId;
        Data->Nodes[2] = IconExtras.NodeId;
        Data->Nodes[3] = IconExtras.ResourceCostTextNode.NodeId;
        Data->Nodes[4] = IconExtras.QuantityTextNode.NodeId;
        Data->Nodes[5] = IconExtras.AntsNode.NodeId;
        Data->Nodes[6] = IconIndicator1.IconNode.NodeId;
        Data->Nodes[7] = IconIndicator2.IconNode.NodeId;

        InitializeComponentEvents();
    }

    private void BuildTimeline() {
        IconExtras.AddTimeline(new TimelineBuilder()
            .BeginFrameSet(1, 59)
            .AddLabelPair(1, 9, 1)
            .AddLabelPair(10, 19, 2)
            .AddLabelPair(20, 29, 3)
            .AddLabelPair(30, 39, 7)
            .AddLabelPair(40, 49, 6)
            .AddLabelPair(50, 59, 4)
            .EndFrameSet()
            .Build());

        var iconIndicatorTimeline = new TimelineBuilder()
            .BeginFrameSet(1, 129)
            .AddLabel(1, 17, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(11, 101, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(21, 102, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(31, 103, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(41, 104, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(51, 105, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(61, 106, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(71, 107, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(80, 108, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(90, 109, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(100, 110, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(110, 111, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(120, 112, AtkTimelineJumpBehavior.PlayOnce, 0)
            .EndFrameSet();

        IconIndicator1.AddTimeline(iconIndicatorTimeline.Build());
        IconIndicator2.AddTimeline(iconIndicatorTimeline.Build());
    }
}
