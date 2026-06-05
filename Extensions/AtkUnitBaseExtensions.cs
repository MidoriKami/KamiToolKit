using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiToolKit.Extensions;

/// <summary>
/// Extension methods for AtkUnitBase.
/// </summary>
public static unsafe class AtkUnitBaseExtensions {
    extension(ref AtkUnitBase addon) {

        /// <summary>
        /// Gets or sets the addon size, if setting will update the addons state.
        /// </summary>
        public Vector2 Size {
            get => addon.GetSize();
            set => addon.Resize(value);
        }

        /// <summary>
        /// Gets if the addon is actually visible, by checking various visibility flags.
        /// </summary>
        public bool IsActuallyVisible
            => addon.GetIsActuallyVisible();

        /// <summary>
        /// Gets the size of the root node.
        /// </summary>
        public Vector2 RootSize
            => addon.GetRootSize();

        /// <summary>
        /// Gets the addons position.
        /// </summary>
        public Vector2 Position
            => new(addon.X, addon.Y);

        /// <summary>
        /// Resizes the target addon to the new size, making sure to adjust various WindowNode properties
        /// to make the window appear and behave normally.
        /// </summary>
        /// <param name="newSize">The new size of the addon</param>
        public void Resize(Vector2 newSize) {
            var windowNode = addon.WindowNode;
            if (windowNode is null) return;

            addon.WindowNode->SetWidth((ushort)newSize.X);
            addon.WindowNode->SetHeight((ushort)newSize.Y);

            if (addon.WindowHeaderCollisionNode is not null) {
                addon.WindowHeaderCollisionNode->SetWidth((ushort)(newSize.X - 14.0f));
            }

            addon.SetSize((ushort)newSize.X, (ushort)newSize.Y);

            addon.WindowNode->Component->UldManager.UpdateDrawNodeList();
            addon.UpdateCollisionNodeList(false);
        }

        private Vector2 GetSize() {
            var width = stackalloc ushort[1];
            var height = stackalloc ushort[1];

            addon.GetSize(width, height, false);
            return new Vector2(*width, *height);
        }

        private Vector2 GetRootSize() {
            if (addon.RootNode is null) return Vector2.Zero;

            return new Vector2(addon.RootNode->Width, addon.RootNode->Height);
        }

        private bool GetIsActuallyVisible() {
            if (!addon.IsVisible) return false;
            if (addon.RootNode is null) return false;
            if (!addon.RootNode->IsVisible()) return false;
            if ((addon.VisibilityFlags & 5) is not 0) return false;

            return true;
        }
    }
}
