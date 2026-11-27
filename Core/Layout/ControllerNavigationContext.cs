using System;
using System.Collections.Generic;
using Dalamud.Plugin.Services;
using KamiToolKit.Core.Primitives.ComponentNode;
using KamiToolKit.Internal.Classes;

namespace KamiToolKit.Core.Layout;

/// <summary>
/// Identifies a controller navigation direction.
/// </summary>
public enum ControllerNavigationDirection {
    /// <summary>
    /// The left direction.
    /// </summary>
    Left,

    /// <summary>
    /// The right direction.
    /// </summary>
    Right,

    /// <summary>
    /// The upward direction.
    /// </summary>
    Up,

    /// <summary>
    /// The downward direction.
    /// </summary>
    Down,
}

/// <summary>
/// Provides byte-safe navigation index allocation and directional link validation for one local context.
/// </summary>
public sealed class ControllerNavigationContext {

    private readonly Dictionary<ComponentNode, int> assignedIndices = [];
    private readonly Dictionary<int, ComponentNode> componentsByIndex = [];
    private readonly HashSet<int> reservedIndices = [];
    private readonly HashSet<string> reportedDiagnostics = [];
    private int nextIndex;

    /// <summary>
    /// Creates a local navigation context beginning at index 1.
    /// </summary>
    public ControllerNavigationContext() : this(1) { }

    /// <summary>
    /// Creates a local navigation context beginning at the specified index.
    /// </summary>
    public ControllerNavigationContext(int firstIndex) {
        if (firstIndex is < 1 or > byte.MaxValue + 1) throw new ArgumentOutOfRangeException(nameof(firstIndex));
        nextIndex = firstIndex;
    }

    /// <summary>
    /// Gets the next index available in this local context.
    /// </summary>
    public int NextIndex => nextIndex;

    /// <summary>
    /// Creates an independent local index scope for a child path that cannot compete with this context.
    /// </summary>
    public ControllerNavigationContext CreateLocalScope(int firstIndex) => new(firstIndex);

    /// <summary>
    /// Advances this context beyond a child scope's allocated indices without assigning those indices to competing nodes.
    /// </summary>
    public bool TryReserveThrough(int exclusiveNextIndex) {
        if (exclusiveNextIndex is < 1 or > byte.MaxValue + 1) {
            ReportOnce("capacity", "Controller navigation index capacity exceeded in a local context; affected components are left unlinked.");
            return false;
        }

        nextIndex = Math.Max(nextIndex, exclusiveNextIndex);
        return true;
    }

    /// <summary>
    /// Assigns the next available nonzero byte index to a component.
    /// </summary>
    public bool TryAssignIndex(ComponentNode component) {
        if (component is null) throw new ArgumentNullException(nameof(component));

        if (assignedIndices.TryGetValue(component, out var existingIndex)) {
            component.NavIndex = existingIndex;
            return true;
        }

        if (nextIndex > byte.MaxValue) {
            component.NavIndex = 0;
            ClearLinks(component);
            ReportOnce("capacity", "Controller navigation index capacity exceeded in a local context; affected components are left unlinked.");
            return false;
        }

        var index = nextIndex++;
        assignedIndices.Add(component, index);
        componentsByIndex.Add(index, component);
        component.NavIndex = index;
        ClearLinks(component);
        return true;
    }

    /// <summary>
    /// Clears a component's navigation index and directional links.
    /// </summary>
    public void ClearNavigation(ComponentNode component) {
        if (component is null) throw new ArgumentNullException(nameof(component));

        if (assignedIndices.Remove(component, out var existingIndex)) {
            componentsByIndex.Remove(existingIndex);
        }

        component.NavIndex = 0;
        ClearLinks(component);
    }

    /// <summary>
    /// Reserves a contiguous range for a dynamic navigation group.
    /// </summary>
    public bool TryReserveRange(int count, out int firstIndex) {
        if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));

        firstIndex = nextIndex;
        if (count is 0) return true;

        var lastIndex = nextIndex + count - 1;
        if (lastIndex > byte.MaxValue) {
            firstIndex = 0;
            ReportOnce("capacity", "Controller navigation index capacity exceeded in a local context; affected components are left unlinked.");
            return false;
        }

        for (var index = nextIndex; index <= lastIndex; index++) {
            reservedIndices.Add(index);
        }

        nextIndex = lastIndex + 1;
        return true;
    }

    /// <summary>
    /// Assigns a component to an index previously reserved by this context.
    /// </summary>
    public bool TryAssignReservedIndex(ComponentNode component, int index) {
        if (component is null) throw new ArgumentNullException(nameof(component));

        if (!reservedIndices.Contains(index) || index is < 1 or > byte.MaxValue) {
            component.NavIndex = 0;
            ClearLinks(component);
            ReportOnce("invalid-reserved-index", "Controller navigation index assignment omitted because the index was not reserved in this local context.");
            return false;
        }

        if (assignedIndices.TryGetValue(component, out var existingIndex)) {
            if (existingIndex == index) return true;
            componentsByIndex.Remove(existingIndex);
            assignedIndices.Remove(component);
        }

        if (componentsByIndex.TryGetValue(index, out var existingComponent) && existingComponent != component) {
            component.NavIndex = 0;
            ClearLinks(component);
            ReportOnce("ambiguous-index", $"Controller navigation index {index} is assigned to multiple components in a local context; the affected component is left unlinked.");
            return false;
        }

        assignedIndices[component] = index;
        componentsByIndex[index] = component;
        component.NavIndex = index;
        ClearLinks(component);
        return true;
    }

    /// <summary>
    /// Sets a directional link only when both the source and target have valid nonzero indices.
    /// Passing no target explicitly clears the direction.
    /// </summary>
    public bool TrySetLink(ComponentNode source, ControllerNavigationDirection direction, ComponentNode? target) {
        if (source is null) throw new ArgumentNullException(nameof(source));

        if (target is null) {
            SetDirection(source, direction, 0);
            return true;
        }

        if (source.NavIndex is < 1 or > byte.MaxValue || target.NavIndex is < 1 or > byte.MaxValue) {
            SetDirection(source, direction, 0);
            ReportOnce("invalid-link", "Controller navigation link omitted because its source or target does not have a valid nonzero NavIndex.");
            return false;
        }

        if (componentsByIndex.TryGetValue(target.NavIndex, out var indexedTarget) && indexedTarget != target) {
            SetDirection(source, direction, 0);
            ReportOnce($"ambiguous-target:{target.NavIndex}", $"Controller navigation target {target.NavIndex} resolves to multiple components in a local context; the affected direction is left unset.");
            return false;
        }

        SetDirection(source, direction, target.NavIndex);
        return true;
    }

    /// <summary>
    /// Sets a directional link to a target index only when that index resolves to a component in this context.
    /// </summary>
    public bool TrySetLink(ComponentNode source, ControllerNavigationDirection direction, int targetIndex) {
        if (targetIndex is 0) {
            SetDirection(source, direction, 0);
            return true;
        }

        if (targetIndex is < 1 or > byte.MaxValue || !componentsByIndex.TryGetValue(targetIndex, out var target)) {
            SetDirection(source, direction, 0);
            ReportOnce($"unresolved-index:{targetIndex}", $"Controller navigation link omitted because target NavIndex {targetIndex} does not resolve to a component in this local context.");
            return false;
        }

        return TrySetLink(source, direction, target);
    }

    /// <summary>
    /// Reports an ambiguous directional target once for this local context.
    /// </summary>
    public void ReportAmbiguousTarget(string description) {
        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        ReportOnce($"ambiguous:{description}", $"Controller navigation target is ambiguous in a local context: {description}. The affected direction is left unset.");
    }

    private void ReportOnce(string key, string message) {
        if (!reportedDiagnostics.Add(key)) return;
        IPluginLog.Get().Warning(message);
    }

    private static void ClearLinks(ComponentNode component) {
        component.NavLeft = 0;
        component.NavRight = 0;
        component.NavUp = 0;
        component.NavDown = 0;
    }

    private static void SetDirection(ComponentNode component, ControllerNavigationDirection direction, int targetIndex) {
        switch (direction) {
            case ControllerNavigationDirection.Left:
                component.NavLeft = targetIndex;
                break;
            case ControllerNavigationDirection.Right:
                component.NavRight = targetIndex;
                break;
            case ControllerNavigationDirection.Up:
                component.NavUp = targetIndex;
                break;
            case ControllerNavigationDirection.Down:
                component.NavDown = targetIndex;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
        }
    }
}