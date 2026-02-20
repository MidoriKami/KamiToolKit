using System;
using System.Linq;
using System.Numerics;
using System.Reflection;
using FFXIVClientStructs.Attributes;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiToolKit.Extensions;

public static unsafe class AtkUnitBaseExtensions {

    public static string GetAddonTypeName<T>() where T : unmanaged {
        var type = typeof(T);
        var attribute = type.GetCustomAttributes().OfType<AddonAttribute>().FirstOrDefault();

        if (attribute is null) throw new Exception("Unable to find AddonAttribute to resolve addon name.");
        var addonName = attribute.AddonIdentifiers.FirstOrDefault();

        if (addonName is null) throw new Exception("Addon attribute names are empty.");
        return addonName;
    }

    extension(ref AtkUnitBase addon) {
        public Vector2 Size {
            get => addon.GetSize();
            set => addon.Resize(value);
        }

        public Vector2 RootSize => addon.GetRootSize();
        public Vector2 Position => new(addon.X, addon.Y);

        private Vector2 GetSize() {
            var width = stackalloc short[1];
            var height = stackalloc short[1];

            addon.GetSize(width, height, false);
            return new Vector2(*width, *height);
        }

        private Vector2 GetRootSize() {
            if (addon.RootNode is null) return Vector2.Zero;
        
            return new Vector2(addon.RootNode->Width, addon.RootNode->Height);
        }

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
    }
}
