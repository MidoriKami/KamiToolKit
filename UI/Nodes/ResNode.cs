using System;
using System.Collections.Generic;
using System.Linq;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Core.Interfaces;
using KamiToolKit.Core.Layout;
using KamiToolKit.Core.Primitives.NodeBase;
using ComponentNodeBase = KamiToolKit.Core.Primitives.ComponentNode.ComponentNode;

namespace KamiToolKit.UI.Nodes;

/// <summary>
/// A generic basic node.
/// </summary>
/// <remarks>
/// </remarks>
public class ResNode() : NodeBase<AtkResNode>(NodeType.Res), INavigationContextUpdatable, INavigationParentAware {

    private INavigationContextUpdatable? navigationParent;

    /// <inheritdoc/>
    public ControllerNavigationBoundary NavigationBoundary { get; protected set; } = ControllerNavigationBoundary.Empty;

    /// <inheritdoc/>
    public virtual void UpdateNavigation() => UpdateNavigationFromRoot();

    /// <inheritdoc/>
    public virtual void UpdateNavigation(ControllerNavigationContext context) {
        ArgumentNullException.ThrowIfNull(context);

        var navigationGroups = new List<(NodeBase Node, ControllerNavigationBoundary Boundary)>();
        foreach (var child in ChildNodes) {
            SetNavigationParentForChild(child, this);
            var childContext = CreateChildNavigationContext(child, context);

            if (child is ComponentNodeBase componentNode) {
                childContext.TryAssignIndex(componentNode);
            }

            if (child is INavigationContextUpdatable contextUpdatable) {
                contextUpdatable.UpdateNavigation(childContext);
            }
            else if (child is INavigationUpdatable navigationUpdatable) {
                navigationUpdatable.UpdateNavigation();
            }

            var boundary = GetNavigationBoundaryForNode(child);
            if (HasNavigationTargets(boundary)) {
                navigationGroups.Add((child, boundary));
            }
        }

        ConnectChildBoundaries(context, navigationGroups);
        NavigationBoundary = CreateNavigationBoundary(navigationGroups);
    }

    /// <summary>
    /// Creates the context passed to a managed child navigation group.
    /// </summary>
    protected virtual ControllerNavigationContext CreateChildNavigationContext(NodeBase child, ControllerNavigationContext context)
        => context;

    /// <summary>
    /// Updates navigation from the outermost managed navigation group.
    /// </summary>
    protected void UpdateNavigationFromRoot() {
        INavigationContextUpdatable root = this;
        while (root is INavigationParentAware parentAware && parentAware.NavigationParent is { } parent) {
            root = parent;
        }

        root.UpdateNavigation(new ControllerNavigationContext());
    }

    INavigationContextUpdatable? INavigationParentAware.NavigationParent => navigationParent;

    void INavigationParentAware.SetNavigationParent(INavigationContextUpdatable? parent) {
        navigationParent = parent;
    }

    private static void SetNavigationParentForChild(NodeBase child, INavigationContextUpdatable parent) {
        if (child is INavigationParentAware navigationParentAware) {
            navigationParentAware.SetNavigationParent(parent);
        }
    }

    private static ControllerNavigationBoundary GetNavigationBoundaryForNode(NodeBase node)
        => node switch {
            ComponentNodeBase componentNode => new ControllerNavigationBoundary([componentNode], [componentNode], [componentNode], [componentNode]),
            INavigationContextUpdatable navigationGroup => navigationGroup.NavigationBoundary,
            _ => ControllerNavigationBoundary.Empty,
        };

    private static ControllerNavigationBoundary CreateNavigationBoundary(IReadOnlyList<(NodeBase Node, ControllerNavigationBoundary Boundary)> groups) {
        var top = groups.SelectMany(group => group.Boundary.Top).Distinct().ToList();
        var topExit = groups.SelectMany(group => group.Boundary.TopExit).Distinct().ToList();
        var bottom = groups.SelectMany(group => group.Boundary.Bottom).Distinct().ToList();
        var left = groups.SelectMany(group => group.Boundary.Left).Distinct().ToList();
        var right = groups.SelectMany(group => group.Boundary.Right).Distinct().ToList();
        var rightEdge = GetExtremeComponents(right, component => component.ScreenPosition.X + component.Width * component.Scale.X, minimize: false);
        var rightEntryTargets = groups
            .Where(group => group.Boundary.Right.Any(rightEdge.Contains))
            .SelectMany(group => group.Boundary.RightEntryTargets)
            .Distinct()
            .ToList();

        return new ControllerNavigationBoundary(
            GetExtremeComponents(top, component => component.ScreenPosition.Y, minimize: true),
            GetExtremeComponents(bottom, component => component.ScreenPosition.Y + component.Height * component.Scale.Y, minimize: false),
            GetExtremeComponents(left, component => component.ScreenPosition.X, minimize: true),
            GetExtremeComponents(right, component => component.ScreenPosition.X + component.Width * component.Scale.X, minimize: false),
            GetExtremeComponents(topExit, component => component.ScreenPosition.Y, minimize: true),
            rightEntryTargets);
    }

    private static bool HasNavigationTargets(ControllerNavigationBoundary boundary)
        => boundary.Top.Count is not 0 || boundary.Bottom.Count is not 0 || boundary.Left.Count is not 0 || boundary.Right.Count is not 0;

    private static IReadOnlyList<ComponentNodeBase> GetExtremeComponents(IReadOnlyList<ComponentNodeBase> components, Func<ComponentNodeBase, float> position, bool minimize) {
        if (components.Count is 0) return [];

        var extreme = minimize ? components.Min(position) : components.Max(position);
        return components.Where(component => MathF.Abs(position(component) - extreme) <= 0.5f).ToList();
    }

    private static void ConnectChildBoundaries(ControllerNavigationContext context, IReadOnlyList<(NodeBase Node, ControllerNavigationBoundary Boundary)> groups) {
        foreach (var (sourceNode, sourceBoundary) in groups) {
            ConnectDirection(context, sourceBoundary.Right, groups.Where(group => !ReferenceEquals(group.Node, sourceNode)).SelectMany(group => group.Boundary.Left), ControllerNavigationDirection.Right, horizontal: true, positive: true);
            ConnectDirection(context, sourceBoundary.Left, groups.Where(group => !ReferenceEquals(group.Node, sourceNode)).SelectMany(group => group.Boundary.Right), ControllerNavigationDirection.Left, horizontal: true, positive: false);
            ConnectDirection(context, sourceBoundary.Bottom, groups.Where(group => !ReferenceEquals(group.Node, sourceNode)).SelectMany(group => group.Boundary.Top), ControllerNavigationDirection.Down, horizontal: false, positive: true);
            ConnectDirection(context, sourceBoundary.TopExit, groups.Where(group => !ReferenceEquals(group.Node, sourceNode)).SelectMany(group => group.Boundary.Bottom), ControllerNavigationDirection.Up, horizontal: false, positive: false);
        }
    }

    private static void ConnectDirection(
        ControllerNavigationContext context,
        IReadOnlyList<ComponentNodeBase> sources,
        IEnumerable<ComponentNodeBase> targetComponents,
        ControllerNavigationDirection direction,
        bool horizontal,
        bool positive) {

        var targets = targetComponents
            .Where(component => component.NavIndex is >= 1 and <= byte.MaxValue)
            .GroupBy(component => component.NavIndex)
            .Select(group => group.ToList())
            .ToList();

        foreach (var sourceGroup in sources
                     .Where(component => component.NavIndex is >= 1 and <= byte.MaxValue)
                     .GroupBy(component => component.NavIndex)) {
            var sourceComponents = sourceGroup.ToList();
            var matches = new List<(List<ComponentNodeBase> Components, float Distance)>();

            foreach (var targetGroup in targets) {
                if (targetGroup[0].NavIndex == sourceGroup.Key) continue;

                var distance = sourceComponents
                    .SelectMany(source => targetGroup.Select(target => GetDirectionalDistance(source, target, horizontal, positive)))
                    .Where(value => value >= 0.0f)
                    .DefaultIfEmpty(float.MaxValue)
                    .Min();

                if (distance < float.MaxValue) matches.Add((targetGroup, distance));
            }

            if (matches.Count is 0) continue;

            var closestDistance = matches.Min(match => match.Distance);
            var closest = matches.Where(match => MathF.Abs(match.Distance - closestDistance) <= 0.5f).ToList();
            if (closest.Count is not 1) {
                foreach (var source in sourceComponents) {
                    context.TrySetLink(source, direction, null);
                }
                context.ReportAmbiguousTarget($"{direction} boundary from NavIndex {sourceGroup.Key}");
                continue;
            }

            foreach (var source in sourceComponents) {
                context.TrySetLink(source, direction, closest[0].Components[0]);
            }
        }
    }

    private static float GetDirectionalDistance(ComponentNodeBase source, ComponentNodeBase target, bool horizontal, bool positive) {
        var sourceX = source.ScreenPosition.X + source.Width * source.Scale.X / 2.0f;
        var sourceY = source.ScreenPosition.Y + source.Height * source.Scale.Y / 2.0f;
        var targetX = target.ScreenPosition.X + target.Width * target.Scale.X / 2.0f;
        var targetY = target.ScreenPosition.Y + target.Height * target.Scale.Y / 2.0f;
        var primaryDistance = horizontal ? targetX - sourceX : targetY - sourceY;

        if (positive ? primaryDistance <= 0.5f : primaryDistance >= -0.5f) return -1.0f;

        var crossDistance = horizontal ? targetY - sourceY : targetX - sourceX;
        return MathF.Sqrt(primaryDistance * primaryDistance + crossDistance * crossDistance);
    }
}
