using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Core.Enums;
using KamiToolKit.Core.Interfaces;
using KamiToolKit.Core.Layout;
using KamiToolKit.Core.Primitives.NodeBase;
using ComponentNodeBase = KamiToolKit.Core.Primitives.ComponentNode.ComponentNode;

namespace KamiToolKit.UI.Nodes.Layout;

/// <summary>
/// Abstract base class for layout nodes that manage a collection of child nodes.
/// Provides the shared ILayoutListNode contract: navigation properties, child management
/// methods that automatically trigger layout and navigation updates, and the layout pipeline.
///
/// Subclasses only need to provide their internal data structure and the layout algorithms.
/// </summary>
public abstract class LayoutListNode : LayoutNodeBase, ILayoutListNode, INavigationContextUpdatable {

    // ── IControllerNavigable ──────────────────────────────────────────────────────

    /// <inheritdoc/>
    public int NavIndex { get; set; }

    /// <inheritdoc/>
    public int NavLeft { get; set; }

    /// <inheritdoc/>
    public int NavRight { get; set; }

    /// <inheritdoc/>
    public int NavUp { get; set; }

    /// <inheritdoc/>
    public int NavDown { get; set; }

    // ── ILayoutNode ───────────────────────────────────────────────────────────────

    /// <inheritdoc/>
    public virtual bool ClipContents {
        get => NodeFlags.HasFlag(NodeFlags.Clip);
        set {
            if (value) {
                AddNodeFlags(NodeFlags.Clip);
            }
            else {
                RemoveNodeFlags(NodeFlags.Clip);
            }
        }
    }

    /// <inheritdoc/>
    public float ItemSpacing {
        get;
        set {
            field = value;
            UpdateLayout();
        }
    }

    /// <inheritdoc/>
    public float FirstItemSpacing {
        get;
        set {
            field = value;
            UpdateLayout();
        }
    }

    /// <inheritdoc/>
    public ICollection<NodeBase> InitialNodes {
        set => AddNode(value);
    }

    /// <inheritdoc/>
    public virtual IEnumerable<T> GetNodes<T>() where T : NodeBase
        => Nodes.OfType<T>();

    /// <inheritdoc/>
    public virtual IReadOnlyList<NodeBase> Nodes => NodeList;

    // ── Child management ──────────────────────────────────────────────────────────

    /// <summary>
    /// Gets the list of nodes that this <see cref="LayoutListNode"/> is managing.
    /// </summary>
    protected List<NodeBase> NodeList { get; } = [];

    /// <summary>
    /// Adds a single node to the list. Override this to implement the actual addition.
    /// </summary>
    protected virtual void AddNodeInternal(NodeBase node) {
        NodeList.Add(node);
        node.AttachNode(this);
        SetNavigationParentForChild(node, this);
    }

    /// <summary>
    /// Removes a single node from the list. Override this to implement the actual removal.
    /// </summary>
    protected virtual bool RemoveNodeInternal(NodeBase node) {
        if (!NodeList.Contains(node)) return false;
        NodeList.Remove(node);
        SetNavigationParentForChild(node, null);
        return true;
    }

    /// <summary>
    /// Clears all nodes from the list. Override this to implement the actual clear.
    /// </summary>
    protected virtual void ClearInternal() {
        foreach (var node in NodeList.ToArray()) {
            SetNavigationParentForChild(node, null);
            node.Dispose();
        }
        NodeList.Clear();
    }

    /// <summary>
    /// Reorders the nodes using the provided comparison. Override this to implement the actual reorder.
    /// </summary>
    protected virtual void ReorderInternal(Comparison<NodeBase> comparison) {
        NodeList.Sort(comparison);
    }

    /// <inheritdoc/>
    public virtual void AddNode(IEnumerable<NodeBase> nodes) {
        using (SuppressLayoutRecalculation()) {
            foreach (var node in nodes) {
                AddNodeInternal(node);
            }
        }
        UpdateLayout();
        UpdateNavigationFromRoot();
    }

    /// <inheritdoc/>
    public virtual void AddNode(NodeBase? node) {
        if (node is null) return;
        AddNodeInternal(node);
        UpdateLayout();
        UpdateNavigationFromRoot();
    }

    /// <inheritdoc/>
    public void RemoveNode(IEnumerable<NodeBase> items) {
        using (SuppressLayoutRecalculation()) {
            foreach (var node in items) {
                RemoveNodeInternal(node);
            }
        }
        UpdateLayout();
        UpdateNavigationFromRoot();
    }

    /// <inheritdoc/>
    public virtual void RemoveNode(NodeBase node) {
        if (!RemoveNodeInternal(node)) return;
        node.Dispose();
        UpdateLayout();
        UpdateNavigationFromRoot();
    }

    /// <inheritdoc/>
    public void AddSpacer(float size = 0.0f) {
        AddNode(new ResNode { Size = new Vector2(size, size) });
    }

    /// <inheritdoc/>
    public void Clear() {
        ClearInternal();
        UpdateLayout();
        UpdateNavigationFromRoot();
    }

    /// <inheritdoc/>
    public void ReorderNodes(Comparison<NodeBase> comparison) {
        ReorderInternal(comparison);
        UpdateLayout();
        UpdateNavigationFromRoot();
    }

    // ── Navigation ────────────────────────────────────────────────────────────────

    /// <inheritdoc/>
    public override void UpdateNavigation() {
        UpdateNavigationFromRoot();
    }

    /// <inheritdoc/>
    public override void UpdateNavigation(ControllerNavigationContext context) {
        ArgumentNullException.ThrowIfNull(context);

        CurrentNavigationContext = context;
        OnPrepareControllerNavigation(context);
        foreach (var node in Nodes) {
            var childContext = CreateChildNavigationContext(node, context);
            if (node is ComponentNodeBase componentNode) {
                childContext.TryAssignIndex(componentNode);
            }

            if (node is INavigationContextUpdatable contextUpdatable) {
                contextUpdatable.UpdateNavigation(childContext);
            }
            else if (node is INavigationUpdatable controllerNavigable) {
                controllerNavigable.UpdateNavigation();
            }
        }

        OnUpdateControllerNav();
        OnConnectControllerNav(context);
    }

    /// <summary>
    /// Gets the navigation context supplied by the owning layout for the current update.
    /// </summary>
    protected ControllerNavigationContext CurrentNavigationContext { get; private set; } = new();

    /// <summary>
    /// Creates the context passed to a child navigation group. Layouts may isolate paths that cannot compete.
    /// </summary>
    protected override ControllerNavigationContext CreateChildNavigationContext(NodeBase child, ControllerNavigationContext context)
        => context;

    /// <summary>
    /// Prepares this group's local controls before child navigation is calculated.
    /// </summary>
    protected virtual void OnPrepareControllerNavigation(ControllerNavigationContext context) { }

    /// <summary>
    /// Sets the edge targets exposed to the parent navigation group.
    /// </summary>
    protected void SetNavigationBoundary(ControllerNavigationBoundary boundary)
        => NavigationBoundary = boundary;

    /// <summary>
    /// Connects this layout's boundary paths after all child groups have updated their navigation.
    /// </summary>
    protected virtual void OnConnectControllerNav(ControllerNavigationContext context) { }

    /// <summary>
    /// Gets focusable direct children and nested navigation groups with their exposed boundaries.
    /// </summary>
    protected List<(NodeBase Node, ControllerNavigationBoundary Boundary)> GetNavigationChildGroups() {
        var groups = new List<(NodeBase Node, ControllerNavigationBoundary Boundary)>();

        foreach (var node in Nodes) {
            var boundary = GetNavigationBoundaryForNode(node);

            if (HasNavigationTargets(boundary)) {
                groups.Add((node, boundary));
            }
        }

        return groups;
    }

    /// <summary>
    /// Gets the navigation boundary exposed by a component or navigation group node.
    /// </summary>
    protected static ControllerNavigationBoundary GetNavigationBoundaryForNode(NodeBase node)
        => node switch {
            ComponentNodeBase componentNode => new ControllerNavigationBoundary([componentNode], [componentNode], [componentNode], [componentNode]),
            INavigationContextUpdatable navigationGroup => navigationGroup.NavigationBoundary,
            _ => ControllerNavigationBoundary.Empty,
        };

    /// <summary>
    /// Gets the topmost focusable targets in a group, falling back to its visible structural edge when it has no top boundary.
    /// </summary>
    protected static IReadOnlyList<ComponentNodeBase> GetNavigationEntryTargets(ControllerNavigationBoundary boundary) {
        if (boundary.Top.Count is not 0) return boundary.Top;

        var candidates = boundary.Bottom.Concat(boundary.Left).Concat(boundary.Right).Distinct().ToList();
        return GetExtremeComponents(candidates, GetTop, minimize: true);
    }

    /// <summary>
    /// Connects adjacent vertical groups by nearest horizontal position.
    /// </summary>
    protected void ConnectVerticalNavigationGroups(IReadOnlyList<(NodeBase Node, ControllerNavigationBoundary Boundary)> groups) {
        for (var index = 0; index < groups.Count - 1; index++) {
            ConnectNavigationBoundaryTargets(groups[index].Boundary.Bottom, groups[index + 1].Boundary.Top,
                ControllerNavigationDirection.Down, ControllerNavigationDirection.Up, vertical: true,
                reverseTargets: groups[index + 1].Boundary.TopExit);
        }
    }

    /// <summary>
    /// Connects adjacent horizontal groups by nearest vertical position.
    /// </summary>
    protected void ConnectHorizontalNavigationGroups(IReadOnlyList<(NodeBase Node, ControllerNavigationBoundary Boundary)> groups) {
        for (var index = 0; index < groups.Count - 1; index++) {
            ConnectNavigationBoundaryTargets(groups[index].Boundary.Right, groups[index + 1].Boundary.Left,
                ControllerNavigationDirection.Right, ControllerNavigationDirection.Left, vertical: false,
                reverseSources: groups[index].Boundary.RightEntryTargets);
        }
    }

    /// <summary>
    /// Publishes the outermost component targets of a set of child groups.
    /// </summary>
    protected static ControllerNavigationBoundary CreateNavigationBoundary(IReadOnlyList<(NodeBase Node, ControllerNavigationBoundary Boundary)> groups) {
        var top = groups.SelectMany(group => group.Boundary.Top).Distinct().ToList();
        var bottom = groups.SelectMany(group => group.Boundary.Bottom).Distinct().ToList();
        var left = groups.SelectMany(group => group.Boundary.Left).Distinct().ToList();
        var right = groups.SelectMany(group => group.Boundary.Right).Distinct().ToList();
        var topExit = groups.SelectMany(group => group.Boundary.TopExit).Distinct().ToList();
        var rightEdge = GetExtremeComponents(right, GetRight, minimize: false);
        var rightEntryTargets = groups
            .Where(group => group.Boundary.Right.Any(rightEdge.Contains))
            .SelectMany(group => group.Boundary.RightEntryTargets)
            .Distinct()
            .ToList();

        if (top.Count is 0 && topExit.Count is 0 && bottom.Count is 0 && left.Count is 0 && right.Count is 0) return ControllerNavigationBoundary.Empty;

        return new ControllerNavigationBoundary(
            GetExtremeComponents(top, GetTop, minimize: true),
            GetExtremeComponents(bottom, GetBottom, minimize: false),
            GetExtremeComponents(left, GetLeft, minimize: true),
            GetExtremeComponents(right, GetRight, minimize: false),
            GetExtremeComponents(topExit, GetTop, minimize: true),
            rightEntryTargets);
    }

    /// <summary>
    /// Links boundary targets to an explicitly configured target index when it resolves in this context.
    /// </summary>
    protected void LinkNavigationBoundaryToIndex(IEnumerable<ComponentNodeBase> components, ControllerNavigationDirection direction, int targetIndex) {
        if (targetIndex is 0) return;

        foreach (var component in components) {
            CurrentNavigationContext.TrySetLink(component, direction, targetIndex);
        }
    }

    /// <summary>
    /// Sets the managed navigation parent on a child whose navigation refresh can originate outside its owner.
    /// </summary>
    protected static void SetNavigationParentForChild(NodeBase child, INavigationContextUpdatable? parent) {
        if (child is INavigationParentAware navigationParentAware) {
            navigationParentAware.SetNavigationParent(parent);
        }
    }


    /// <summary>
    /// Connects directional paths between two navigation boundaries using their nearest unambiguous targets.
    /// </summary>
    protected void ConnectNavigationBoundaryTargets(
        IReadOnlyList<ComponentNodeBase> sources,
        IReadOnlyList<ComponentNodeBase> targets,
        ControllerNavigationDirection forwardDirection,
        ControllerNavigationDirection reverseDirection,
        bool vertical,
        IReadOnlyList<ComponentNodeBase>? reverseTargets = null,
        bool connectReverse = true,
        IReadOnlyList<ComponentNodeBase>? reverseSources = null) {

        if (sources.Count is 0 || targets.Count is 0) return;

        var sourceGroups = GroupByNavigationIndex(sources);
        var targetGroups = GroupByNavigationIndex(targets);
        var reverseSourceGroups = reverseSources is null ? null : GroupByNavigationIndex(reverseSources);
        var matches = new Dictionary<NavigationBoundaryTargetGroup, NavigationBoundaryTargetGroup>();

        foreach (var sourceGroup in sourceGroups) {
            var targetGroup = FindNearestGroup(sourceGroup, targetGroups, vertical, out var ambiguous);
            if (ambiguous) {
                foreach (var source in sourceGroup.Components) {
                    CurrentNavigationContext.TrySetLink(source, forwardDirection, null);
                }
                CurrentNavigationContext.ReportAmbiguousTarget($"{forwardDirection} boundary from NavIndex {sourceGroup.NavIndex}");
                continue;
            }

            if (targetGroup is null) {
                foreach (var source in sourceGroup.Components) {
                    CurrentNavigationContext.TrySetLink(source, forwardDirection, null);
                }
                continue;
            }

            var isConnected = false;
            foreach (var source in sourceGroup.Components) {
                isConnected |= CurrentNavigationContext.TrySetLink(source, forwardDirection, targetGroup.Components[0]);
            }

            if (isConnected) matches[sourceGroup] = targetGroup;
        }

        if (connectReverse) {
            foreach (var targetGroup in targetGroups) {
                var matchingSources = matches.Where(match => match.Value == targetGroup).Select(match => match.Key).ToList();
                if (reverseSourceGroups is not null) {
                    if (matchingSources.Count is 0) continue;

                    var reverseSourceGroup = FindNearestGroup(targetGroup, reverseSourceGroups, vertical, out var ambiguous);
                    if (ambiguous) {
                        foreach (var target in targetGroup.Components) {
                            CurrentNavigationContext.TrySetLink(target, reverseDirection, null);
                        }
                        CurrentNavigationContext.ReportAmbiguousTarget($"{reverseDirection} boundary to NavIndex {targetGroup.NavIndex}");
                        continue;
                    }

                    foreach (var target in targetGroup.Components) {
                        CurrentNavigationContext.TrySetLink(target, reverseDirection, reverseSourceGroup?.Components[0]);
                    }
                    continue;
                }

                if (matchingSources.Count is 1) {
                    foreach (var target in targetGroup.Components) {
                        CurrentNavigationContext.TrySetLink(target, reverseDirection, matchingSources[0].Components[0]);
                    }
                }
                else if (matchingSources.Count > 1) {
                    foreach (var target in targetGroup.Components) {
                        CurrentNavigationContext.TrySetLink(target, reverseDirection, null);
                    }
                    CurrentNavigationContext.ReportAmbiguousTarget($"{reverseDirection} boundary to NavIndex {targetGroup.NavIndex}");
                }
            }
        }

        if (reverseTargets is null) return;

        var additionalReverseTargets = GroupByNavigationIndex(reverseTargets.Where(target => !targets.Contains(target)));
        foreach (var targetGroup in additionalReverseTargets) {
            var sourceGroup = FindNearestGroup(targetGroup, sourceGroups, vertical, out var ambiguous);
            if (ambiguous) {
                foreach (var target in targetGroup.Components) {
                    CurrentNavigationContext.TrySetLink(target, reverseDirection, null);
                }
                CurrentNavigationContext.ReportAmbiguousTarget($"{reverseDirection} boundary to NavIndex {targetGroup.NavIndex}");
                continue;
            }

            foreach (var target in targetGroup.Components) {
                CurrentNavigationContext.TrySetLink(target, reverseDirection, sourceGroup?.Components[0]);
            }
        }
    }

    private static List<NavigationBoundaryTargetGroup> GroupByNavigationIndex(IEnumerable<ComponentNodeBase> components) {
        return components
            .Where(component => component.NavIndex is >= 1 and <= byte.MaxValue)
            .GroupBy(component => component.NavIndex)
            .Select(group => new NavigationBoundaryTargetGroup(group.Key, group.ToList()))
            .ToList();
    }

    private static NavigationBoundaryTargetGroup? FindNearestGroup(
        NavigationBoundaryTargetGroup source,
        IReadOnlyList<NavigationBoundaryTargetGroup> targets,
        bool vertical,
        out bool ambiguous) {

        ambiguous = false;
        var closestDistance = float.MaxValue;
        NavigationBoundaryTargetGroup? closest = null;

        foreach (var target in targets) {
            var distance = source.Components
                .SelectMany(sourceComponent => target.Components.Select(targetComponent => MathF.Abs(GetCenter(sourceComponent, vertical) - GetCenter(targetComponent, vertical))))
                .Min();
            if (distance + 0.5f < closestDistance) {
                closestDistance = distance;
                closest = target;
                ambiguous = false;
            }
            else if (MathF.Abs(distance - closestDistance) <= 0.5f) {
                ambiguous = true;
            }
        }

        return ambiguous ? null : closest;
    }

    private static bool HasNavigationTargets(ControllerNavigationBoundary boundary)
        => boundary.Top.Count is not 0 || boundary.TopExit.Count is not 0 || boundary.Bottom.Count is not 0 || boundary.Left.Count is not 0 || boundary.Right.Count is not 0;

    private static IReadOnlyList<ComponentNodeBase> GetExtremeComponents(IReadOnlyList<ComponentNodeBase> components, Func<ComponentNodeBase, float> position, bool minimize) {
        if (components.Count is 0) return [];
        var extreme = minimize ? components.Min(position) : components.Max(position);
        return components.Where(component => MathF.Abs(position(component) - extreme) <= 0.5f).ToList();
    }

    private static float GetTop(ComponentNodeBase component) => component.ScreenPosition.Y;

    private static float GetBottom(ComponentNodeBase component) => component.ScreenPosition.Y + component.Height * component.Scale.Y;

    private static float GetLeft(ComponentNodeBase component) => component.ScreenPosition.X;

    private static float GetRight(ComponentNodeBase component) => component.ScreenPosition.X + component.Width * component.Scale.X;

    private static float GetCenter(ComponentNodeBase component, bool horizontal)
        => horizontal
            ? component.ScreenPosition.X + component.Width * component.Scale.X / 2.0f
            : component.ScreenPosition.Y + component.Height * component.Scale.Y / 2.0f;

    private sealed class NavigationBoundaryTargetGroup(int navIndex, IReadOnlyList<ComponentNodeBase> components) {

        public int NavIndex { get; } = navIndex;

        public IReadOnlyList<ComponentNodeBase> Components { get; } = components;
    }

    // ── Layout pipeline ───────────────────────────────────────────────────────────

    /// <inheritdoc/>
    protected override void DoUpdateLayout() {
        // Save old sizes to check if sizes were changed later for recursing.
        var oldSizes = NodeList.Select(n => n.Size).ToArray();

        UpdateLayoutFillerNodes();
        UpdateChildrenLayout(oldSizes);
    }

    /// <summary>
    /// If any children are ILayoutFiller, resize them.
    /// </summary>
    protected virtual void UpdateLayoutFillerNodes() {
        var remainingHeight = Height - FirstItemSpacing;
        var remainingWidth = Width - FirstItemSpacing;

        foreach (var node in NodeList) {
            if (node is ILayoutFiller filler) {
                if (filler.LayoutFill.HasFlag(LayoutFill.Vertical)) {
                    node.Height = remainingHeight;
                }

                if (filler.LayoutFill.HasFlag(LayoutFill.Horizontal)) {
                    node.Width = remainingWidth;
                }
            }

            remainingHeight -= node.Height + ItemSpacing;
            remainingWidth -= node.Width + ItemSpacing;
        }
    }

    /// <summary>
    /// Updates child layout and recursively updates children whose sizes changed.
    /// </summary>
    protected virtual void UpdateChildrenLayout(Vector2[] oldSizes) {
        if (NodeList.Count is 0) return;

        OnUpdateSizes();
        OnUpdateLayout();

        foreach (var (node, oldSize) in NodeList.Zip(oldSizes)) {
            if (node is not ILayoutUpdatable subNode) continue;

            var newSize = node.Size;
            if (Vector2.Distance(oldSize, newSize) > 0.5f) {
                subNode.UpdateLayout();
            }
        }

        // Reapply sizes in case children changed to fit something.
        OnUpdateSizes();
    }

    /// <summary>
    /// Invoked when this node needs to update child node sizes.
    /// </summary>
    protected abstract void OnUpdateSizes();

    /// <summary>
    /// Invoked when this node needs to update child node positions.
    /// </summary>
    protected abstract void OnUpdateLayout();

    /// <summary>
    /// Invoked when this node needs to update controller navigation layout.
    /// </summary>
    protected abstract void OnUpdateControllerNav();
}
