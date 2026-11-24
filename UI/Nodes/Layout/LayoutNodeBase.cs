using System.Numerics;
using KamiToolKit.Core.Classes;
using KamiToolKit.Core.Interfaces;

namespace KamiToolKit.UI.Nodes;

/// <summary>
/// Abstract base class providing shared layout update infrastructure:
/// size-change detection, recursive guard, and performance logging.
/// </summary>
public abstract class LayoutNodeBase : ResNode, ILayoutUpdatable {

    /// <summary>
    /// Updates the contained layout. Subclasses should override <see cref="DoUpdateLayout"/>
    /// to perform the actual layout work.
    /// </summary>
    public virtual void UpdateLayout() {
        if (SuppressRecalculateLayout) return;

        // If we're already inside a layout update, just mark dirty for the final pass.
        if (isUpdatingLayout) {
            layoutDirty = true;
            return;
        }

        isUpdatingLayout = true;
        try {
            DoUpdateLayout();
        }
        finally {
            isUpdatingLayout = false;
        }

        // If a nested call marked us dirty, do one more pass to propagate the changes.
        if (layoutDirty) {
            layoutDirty = false;
            DoUpdateLayout();
        }
    }

    /// <summary>
    /// Perform the actual layout update. Override this instead of <see cref="UpdateLayout"/>
    /// to get recursive guard handling for free.
    /// </summary>
    protected abstract void DoUpdateLayout();

    /// <summary>Whether a layout update is currently in progress.</summary>
    private bool isUpdatingLayout;

    /// <summary>Set when a nested UpdateLayout call needs the parent to update again after completion.</summary>
    private bool layoutDirty;

    /// <summary>Whether layout recalculation is currently suppressed during batch operations.</summary>
    protected bool SuppressRecalculateLayout;

    /// <summary>
    /// Temporarily suppresses layout recalculation while performing batch operations.
    /// </summary>
    protected ScopeGuard SuppressLayoutRecalculation() {
        SuppressRecalculateLayout = true;
        return new ScopeGuard(() => SuppressRecalculateLayout = false);
    }

    /// <summary>
    /// Automatically updates the layout when this node's size changes.
    /// </summary>
    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        // Only update layout if the size has actually changed, to prevent unnecessary updates.
        if (Size != lastLayoutSize) {
            lastLayoutSize = Size;
            UpdateLayout();
        }
    }

    /// <summary>The last size that triggered a layout update.</summary>
    private Vector2 lastLayoutSize;
}
