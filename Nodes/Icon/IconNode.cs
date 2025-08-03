using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes.TimelineBuilding;

namespace KamiToolKit.Nodes;

public unsafe class IconNode : ComponentNode<AtkComponentIcon, AtkUldComponentDataIcon> {

    public readonly IconExtras IconExtras;
    public readonly IconImageNode IconImage;
    public readonly IconIndicator IconIndicator1;
    public readonly IconIndicator IconIndicator2;

    public IconNode() {
        SetInternalComponentType(ComponentType.Icon);

        IconImage = new IconImageNode {
            NodeId = 20,
            Size = new Vector2(40.0f, 40.0f),
            Position = new Vector2(2.0f, 3.0f),
            NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.EmitsEvents,
            WrapMode = 1,
            ImageNodeFlags = ImageNodeFlags.AutoFit,
        };

        IconImage.AttachNode(this);

        IconExtras = new IconExtras {
            NodeId = 6, Size = new Vector2(60, 60), Position = new Vector2(-2.0f, 0.0f), NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.EmitsEvents,
        };

        IconExtras.AttachNode(this);

        IconIndicator1 = new IconIndicator(5) {
            NodeId = 4, Size = new Vector2(18.0f, 18.0f), Position = new Vector2(27.0f, 11.0f), NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.EmitsEvents,
        };

        IconIndicator1.AttachNode(this);

        IconIndicator2 = new IconIndicator(3) {
            NodeId = 2, Size = new Vector2(18.0f, 18.0f), Position = new Vector2(27.0f, -2.0f), NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.EmitsEvents,
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

    public uint IconId {
        get => Component->IconId;
        set => Component->LoadIcon(value);
    }

    public bool IsIconLoading
        => Component->Flags.HasFlag(IconComponentFlags.IsIconLoading);

    public bool IsIconDisabled {
        get => Component->Flags.HasFlag(IconComponentFlags.IsDisabled);
        set => Component->SetIconImageDisableState(value);
    }

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

    public bool IsMacro {
        get => Component->Flags.HasFlag(IconComponentFlags.IsMacro);
        set => Component->SetIsMacro(value);
    }

    public bool IsRecipe {
        get => Component->Flags.HasFlag(IconComponentFlags.IsRecipe);
        set => Component->SetIsRecipe(value);
    }

    public bool IsBeingDragged
        => Component->Flags.HasFlag(IconComponentFlags.IsBeingDragged);

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
