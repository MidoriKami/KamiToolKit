using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.BaseTypes;
using KamiToolKit.Classes;
using KamiToolKit.Nodes.Simplified;
using KamiToolKit.Timelines;
using Lumina.Text.ReadOnly;

namespace KamiToolKit.Nodes;

/// <summary>
/// A custom implementation of a TreeListCategory Node. todo: maybe rewrite?
/// </summary>
/// <remarks>
/// This implementation is meant to mimic the native games list category nodes, but it's not great.
/// </remarks>
/// <remarks>
/// Needs reworking, it sorta works but is buggy at the moment.
/// </remarks>
[Experimental("KamiToolKit_Experimental")]
public unsafe class TreeListCategoryNode : ResNode {

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public NineGridNode BackgroundNode { get; }

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public ResNode ChildContainer { get; }

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public ImageNode CollapseArrowNode { get; }

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public CollisionNode CollisionNode { get; }

    /// <summary>
    /// Not intended for public use, but it's here if you absolutely need it.
    /// </summary>
    public TextNode LabelNode { get; }

    /// <summary>
    /// Gets a readonly collection of just the HeaderNodes.
    /// </summary>
    public IReadOnlyCollection<TreeListHeaderNode> HeaderNodes => children.OfType<TreeListHeaderNode>().ToList();

    /// <summary>
    /// Gets a read only collect of all children nodes.
    /// </summary>
    public IReadOnlyCollection<NodeBase> Children => children.AsReadOnly();

    /// <summary>
    /// Gets a readonly enumerable of all contained nodes of the specified type.
    /// </summary>
    public IEnumerable<T> GetNodes<T>() where T : NodeBase => children.OfType<T>();

    /// <summary>
    /// Event that is called when the category is collapsed or uncollapsed.
    /// </summary>
    public Action<bool>? OnToggle { get; set; }

    /// <summary>
    /// Gets a reference to the parent TreeList that contains this CategoryNode.
    /// </summary>
    public TreeListNode? ParentTreeListNode { get; set; }

    /// <summary>
    /// Gets or sets whether this node is collapsed
    /// </summary>
    public bool IsCollapsed {
        get;
        set {
            field = value;
            UpdateCollapsed();
            Timeline?.PlayAnimation(IsCollapsed ? 1 : 8);
        }
    }

    /// <summary>
    /// Gets or sets the amount of space before the first child.
    /// </summary>
    public float VerticalPadding { get; set; } = 4.0f;

    /// <summary>
    /// Gets or sets the label displayed for this category.
    /// </summary>
    public ReadOnlySeString String {
        get => LabelNode.String;
        set => LabelNode.String = value;
    }

    /// <summary>
    /// Recalculates the positions of all children.
    /// </summary>
    public void RecalculateLayout() {
        ChildContainer.Height = 0.0f;

        foreach (var child in children) {
            if (!child.IsVisible) continue;

            child.Y = ChildContainer.Height;
            child.Width = ChildContainer.Width;

            ChildContainer.Height += child.Height + VerticalPadding;
            Height = ChildContainer.Height + BackgroundNode.Height;
        }

        UpdateCollapsed();
    }

    /// <summary>
    /// Add a new header node with the specified label.
    /// </summary>
    public void AddHeader(ReadOnlySeString label) {
        var newHeaderNode = new TreeListHeaderNode {
            Size = new Vector2(Width, 24.0f),
            String = label,
        };

        AddNode(newHeaderNode);
    }

    /// <summary>
    /// Add a entry node to this cagtegory.
    /// </summary>
    public void AddNode(NodeBase node) {
        node.Y = ChildContainer.Height;
        node.Width = ChildContainer.Width;

        ChildContainer.Height += node.Height + VerticalPadding;
        Height = ChildContainer.Height + BackgroundNode.Height;

        children.Add(node);
        node.AttachNode(ChildContainer);
        UpdateCollapsed();
    }

    public TreeListCategoryNode() {
        CollisionNode = new CollisionNode {
            Height = 28.0f,
        };
        CollisionNode.AttachNode(this);

        BackgroundNode = new SimpleNineGridNode {
            TexturePath = "ui/uld/ListItemB.tex",
            TextureSize = new Vector2(48.0f, 28.0f),
            TextureCoordinates = new Vector2(0.0f, 24.0f),
            Height = 28.0f,
            TopOffset = 10.0f,
            LeftOffset = 12.0f,
            RightOffset = 12.0f,
            BottomOffset = 12.0f,
        };
        BackgroundNode.AttachNode(this);

        CollapseArrowNode = new ImageNode {
            Position = new Vector2(0.0f, 1.0f),
            Size = new Vector2(24.0f, 24.0f),
            PartId = 1,
        };

        CollapseArrowNode.AddPart(new Part {
            TexturePath = "ui/uld/ListItemB.tex",
            TextureCoordinates = new Vector2(0.0f, 0.0f),
            Size = new Vector2(24.0f, 24.0f),
            Id = 0,
        });

        CollapseArrowNode.AddPart(new Part {
            TexturePath = "ui/uld/ListItemB.tex",
            TextureCoordinates = new Vector2(24.0f, 0.0f),
            Size = new Vector2(24.0f, 24.0f),
            Id = 1,
        });
        CollapseArrowNode.AttachNode(this);

        LabelNode = new TextNode {
            Position = new Vector2(23.0f, 0.0f),
            FontType = FontType.Axis,
            FontSize = 14,
            Height = 28.0f,
            AlignmentType = AlignmentType.Left,
            TextColor = ColorHelper.GetColor(50),
            TextOutlineColor = ColorHelper.GetColor(7),
        };
        LabelNode.AttachNode(this);

        ChildContainer = new ResNode {
            Position = new Vector2(0.0f, 24.0f + VerticalPadding),
        };
        ChildContainer.AttachNode(this);

        BuildTimelines();

        CollisionNode.ShowClickableCursor = true;
        CollisionNode.AddEvent(AtkEventType.MouseOver, () => Timeline?.PlayAnimation(IsCollapsed ? 2 : 9));
        CollisionNode.AddEvent(AtkEventType.MouseOut, () => Timeline?.PlayAnimation(IsCollapsed ? 1 : 8));
        CollisionNode.AddEvent(AtkEventType.MouseClick, () => {
            IsCollapsed = !IsCollapsed;
            UpdateCollapsed();
            OnToggle?.Invoke(!IsCollapsed);
        });
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        BackgroundNode.Width = Width;
        CollapseArrowNode.Width = 24.0f;
        LabelNode.Width = Width - 23.0f;
        ChildContainer.Width = Width;
        CollisionNode.Width = Width;

        foreach (var node in children) {
            node.Width = Width;
        }
    }

    private void UpdateCollapsed() {
        Timeline?.PlayAnimation(IsCollapsed ? 1 : 8);
        ChildContainer.IsVisible = !IsCollapsed;
        Height = IsCollapsed ? BackgroundNode.Height : ChildContainer.Height + BackgroundNode.Height;
        ParentTreeListNode?.RefreshLayout();
    }

    private void BuildTimelines() {
        AddTimeline(new TimelineBuilder()
            .BeginFrameSet(1, 119)
            .AddLabel(1, 1, AtkTimelineJumpBehavior.Start, 0)
            .AddLabel(9, 0, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(10, 2, AtkTimelineJumpBehavior.Start, 0)
            .AddLabel(19, 0, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(20, 3, AtkTimelineJumpBehavior.Start, 0)
            .AddLabel(29, 0, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(30, 7, AtkTimelineJumpBehavior.Start, 0)
            .AddLabel(39, 0, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(40, 6, AtkTimelineJumpBehavior.Start, 0)
            .AddLabel(49, 0, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(50, 4, AtkTimelineJumpBehavior.Start, 0)
            .AddLabel(59, 0, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(60, 8, AtkTimelineJumpBehavior.Start, 0)
            .AddLabel(69, 0, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(70, 9, AtkTimelineJumpBehavior.Start, 0)
            .AddLabel(79, 0, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(80, 10, AtkTimelineJumpBehavior.Start, 0)
            .AddLabel(89, 0, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(90, 14, AtkTimelineJumpBehavior.Start, 0)
            .AddLabel(99, 0, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(100, 13, AtkTimelineJumpBehavior.Start, 0)
            .AddLabel(109, 0, AtkTimelineJumpBehavior.PlayOnce, 0)
            .AddLabel(110, 11, AtkTimelineJumpBehavior.Start, 0)
            .AddLabel(119, 0, AtkTimelineJumpBehavior.PlayOnce, 0)
            .EndFrameSet()
            .Build()
        );

        CollapseArrowNode.AddTimeline(new TimelineBuilder()
            .BeginFrameSet(1, 9)
            .AddFrame(1, alpha: 255)
            .AddFrame(1, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
            .AddFrame(1, partId: 0)
            .EndFrameSet()
            .BeginFrameSet(10, 19)
            .AddFrame(10, alpha: 255)
            .AddFrame(12, alpha: 255)
            .AddFrame(10, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
            .AddFrame(12, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
            .AddFrame(10, partId: 0)
            .AddFrame(12, partId: 0)
            .EndFrameSet()
            .BeginFrameSet(20, 29)
            .AddFrame(20, alpha: 255)
            .AddFrame(20, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
            .AddFrame(20, partId: 0)
            .EndFrameSet()
            .BeginFrameSet(30, 39)
            .AddFrame(30, alpha: 178)
            .AddFrame(30, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(50, 50, 50))
            .AddFrame(30, partId: 0)
            .EndFrameSet()
            .BeginFrameSet(40, 49)
            .AddFrame(40, alpha: 255)
            .AddFrame(40, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
            .AddFrame(40, partId: 0)
            .EndFrameSet()
            .BeginFrameSet(50, 59)
            .AddFrame(50, alpha: 255)
            .AddFrame(52, alpha: 255)
            .AddFrame(50, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
            .AddFrame(52, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .AddFrame(50, partId: 0)
            .AddFrame(52, partId: 0)
            .EndFrameSet()
            .BeginFrameSet(60, 69)
            .AddFrame(60, alpha: 255)
            .AddFrame(60, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .AddFrame(60, partId: 1)
            .EndFrameSet()
            .BeginFrameSet(70, 79)
            .AddFrame(70, alpha: 255)
            .AddFrame(72, alpha: 255)
            .AddFrame(70, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .AddFrame(72, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
            .AddFrame(70, partId: 1)
            .AddFrame(72, partId: 1)
            .EndFrameSet()
            .BeginFrameSet(80, 89)
            .AddFrame(80, alpha: 255)
            .AddFrame(80, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
            .AddFrame(80, partId: 0)
            .EndFrameSet()
            .BeginFrameSet(90, 99)
            .AddFrame(90, alpha: 178)
            .AddFrame(90, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(50, 50, 50))
            .AddFrame(90, partId: 1)
            .EndFrameSet()
            .BeginFrameSet(100, 109)
            .AddFrame(100, alpha: 255)
            .AddFrame(100, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
            .AddFrame(100, partId: 1)
            .EndFrameSet()
            .BeginFrameSet(110, 119)
            .AddFrame(110, alpha: 255)
            .AddFrame(112, alpha: 255)
            .AddFrame(110, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
            .AddFrame(112, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .AddFrame(110, partId: 1)
            .AddFrame(112, partId: 1)
            .EndFrameSet()
            .Build()
        );

        LabelNode.AddTimeline(new TimelineBuilder()
            .BeginFrameSet(1, 9)
            .AddFrame(1, alpha: 229)
            .AddFrame(1, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(10, 19)
            .AddFrame(10, alpha: 229)
            .AddFrame(10, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(20, 29)
            .AddFrame(20, alpha: 229)
            .AddFrame(20, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(30, 39)
            .AddFrame(30, alpha: 153)
            .AddFrame(30, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(80, 80, 80))
            .EndFrameSet()
            .BeginFrameSet(40, 49)
            .AddFrame(40, alpha: 229)
            .AddFrame(40, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(50, 59)
            .AddFrame(50, alpha: 229)
            .AddFrame(50, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(60, 69)
            .AddFrame(60, alpha: 229)
            .AddFrame(60, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(70, 79)
            .AddFrame(70, alpha: 229)
            .AddFrame(70, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(80, 89)
            .AddFrame(80, alpha: 229)
            .AddFrame(80, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(90, 99)
            .AddFrame(90, alpha: 153)
            .AddFrame(90, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(80, 80, 80))
            .EndFrameSet()
            .BeginFrameSet(100, 109)
            .AddFrame(100, alpha: 229)
            .AddFrame(100, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(110, 119)
            .AddFrame(110, alpha: 229)
            .AddFrame(110, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .Build()
        );

        BackgroundNode.AddTimeline(new TimelineBuilder()
            .BeginFrameSet(1, 9)
            .AddFrame(1, alpha: 255)
            .AddFrame(1, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(10, 19)
            .AddFrame(10, alpha: 255)
            .AddFrame(12, alpha: 255)
            .AddFrame(10, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .AddFrame(12, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(20, 29)
            .AddFrame(20, alpha: 255)
            .AddFrame(20, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(30, 39)
            .AddFrame(30, alpha: 178)
            .AddFrame(30, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(50, 50, 50))
            .EndFrameSet()
            .BeginFrameSet(40, 49)
            .AddFrame(40, alpha: 255)
            .AddFrame(40, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(50, 59)
            .AddFrame(50, alpha: 255)
            .AddFrame(52, alpha: 255)
            .AddFrame(50, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
            .AddFrame(52, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(60, 69)
            .AddFrame(60, alpha: 255)
            .AddFrame(60, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(70, 79)
            .AddFrame(70, alpha: 255)
            .AddFrame(72, alpha: 255)
            .AddFrame(70, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .AddFrame(72, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(80, 89)
            .AddFrame(80, alpha: 255)
            .AddFrame(80, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(90, 99)
            .AddFrame(90, alpha: 178)
            .AddFrame(90, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(50, 50, 50))
            .EndFrameSet()
            .BeginFrameSet(100, 109)
            .AddFrame(100, alpha: 255)
            .AddFrame(100, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .BeginFrameSet(110, 119)
            .AddFrame(110, alpha: 255)
            .AddFrame(112, alpha: 255)
            .AddFrame(110, addColor: new Vector3(16, 16, 16), multiplyColor: new Vector3(100, 100, 100))
            .AddFrame(112, addColor: new Vector3(0, 0, 0), multiplyColor: new Vector3(100, 100, 100))
            .EndFrameSet()
            .Build()
        );
    }

    private readonly List<NodeBase> children = [];
}
