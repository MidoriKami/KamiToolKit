using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.BaseTypes.ComponentNode;
using KamiToolKit.Enums;
using KamiToolKit.Interfaces;
using KamiToolKit.Timelines;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Nodes;

/// <summary>
/// Implementation of the game's IconNode and associated component.
/// This is often used as a part in a <see cref="DragDropNode"/>, but is not required to be used as a part can be used by itself.
/// </summary>
public unsafe class IconNode : ComponentNode<AtkComponentIcon, AtkUldComponentDataIcon>, IIconNode {

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

    /// <inheritdoc/>
    public uint IconId {
        get => Component->IconId;
        set => Component->LoadIcon(value);
    }

    /// <inheritdoc/>
    public bool IsIconLoading
        => Component->Flags.HasFlag(IconComponentFlags.IsIconLoading);

    /// <inheritdoc/>
    public bool IsIconDisabled {
        get => Component->Flags.HasFlag(IconComponentFlags.IsDisabled);
        set => Component->SetIconImageDisableState(value);
    }

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public bool IsMacro {
        get => Component->Flags.HasFlag(IconComponentFlags.IsMacro);
        set => Component->SetIsMacro(value);
    }

    /// <inheritdoc/>
    public bool IsRecipe {
        get => Component->Flags.HasFlag(IconComponentFlags.IsRecipe);
        set => Component->SetIsRecipe(value);
    }

    /// <inheritdoc/>
    public bool IsBeingDragged
        => Component->Flags.HasFlag(IconComponentFlags.IsBeingDragged);

    /// <inheritdoc/>
    public ReadOnlySeString ResourceCostString {
        get => IconExtras.ResourceCostString;
        set => IconExtras.ResourceCostString = value;
    }

    /// <inheritdoc/>
    public uint ResourceCostValue {
        get => IconExtras.ResourceCostValue;
        set => IconExtras.ResourceCostValue = value;
    }

    /// <inheritdoc/>
    public bool ResourceCostVisible {
        get => IconExtras.ResourceCostVisible;
        set => IconExtras.ResourceCostVisible = value;
    }

    /// <inheritdoc/>
    public int ChargeCount {
        get => IconExtras.ChargeCount;
        set => IconExtras.ChargeCount = value;
    }

    /// <inheritdoc/>
    public bool ChargeCountVisible {
        get => IconExtras.ChargeCountVisible;
        set => IconExtras.ChargeCountVisible = value;
    }

    /// <inheritdoc/>
    public float ChargePercent {
        get => IconExtras.ChargePercent;
        set => IconExtras.ChargePercent = value;
    }

    /// <inheritdoc/>
    public ReadOnlySeString CooldownSecondsString {
        get => IconExtras.CooldownSecondsString;
        set => IconExtras.CooldownSecondsString = value;
    }

    /// <inheritdoc/>
    public uint CooldownSeconds {
        get => IconExtras.CooldownSeconds;
        set => IconExtras.CooldownSeconds = value;
    }

    /// <inheritdoc/>
    public bool CooldownSecondsVisible {
        get => IconExtras.CooldownSecondsVisible;
        set => IconExtras.CooldownSecondsVisible = value;
    }

    /// <inheritdoc/>
    public float CooldownPercent {
        get => IconExtras.CooldownPercent;
        set => IconExtras.CooldownPercent = value;
    }

    /// <inheritdoc/>
    public bool CooldownPercentVisible {
        get => IconExtras.CooldownPercentVisible;
        set => IconExtras.CooldownPercentVisible = value;
    }

    /// <inheritdoc/>
    public CostTextColor CostTextColor {
        get => IconExtras.CostTextColor;
        set => IconExtras.CostTextColor = value;
    }

    /// <inheritdoc/>
    public bool IsInvalid {
        get => IconExtras.IsInvalid;
        set => IconExtras.IsInvalid = value;
    }

    /// <inheritdoc/>
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

    /// <inheritdoc/>
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

    /// <inheritdoc/>
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
