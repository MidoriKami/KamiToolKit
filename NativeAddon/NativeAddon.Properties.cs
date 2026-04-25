using System;
using System.Linq;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Nodes;
using Lumina.Text.ReadOnly;

namespace KamiToolKit;

public unsafe partial class NativeAddon {
    public ResNode RootNode { get; private set; } = null!;
    
    public static implicit operator AtkUnitBase*(NativeAddon addon) => addon.InternalAddon;
    
    public Func<WindowNodeBase>? CreateWindowNode { get; init; }

    public required string InternalName {
        get;
        init => field = new string(value.Replace(" ", "").Take(31).ToArray());
    }

    public required ReadOnlySeString Title { get; set; }

    public ReadOnlySeString? Subtitle { get; set; }

    public int OpenWindowSoundEffectId { get; set; } = 23;

    public Vector2 Size { get; set; } = new(400.0f, 400.0f);

    public Vector2 ContentStartPosition => (WindowNode?.ContentStartPosition ?? Vector2.Zero) + ContentPadding;

    public Vector2 ContentSize => (WindowNode?.ContentSize ?? Vector2.Zero) - ContentPadding * 2.0f - new Vector2(0.0f, 4.0f);

    public Vector2 ContentPadding { get; set; } = new(8.0f, 0.0f);

    public int DepthLayer { get; init; } = 5;

    public bool IsOpen => InternalAddon is not null && InternalAddon->IsVisible;

    public int AddonId => InternalAddon is null ? 0 : InternalAddon->Id;

    public bool RememberClosePosition { get; set; } = true;
    
    internal Vector2 LastClosePosition = Vector2.Zero;

    internal bool IsOverlayAddon { get; init; }

    public bool OpenInBounds { get; init; } = true;

    /// <summary>
    /// Pre-allocates addon for use in AddonFactories. Once this is set, KTK can no longer control the addon's lifecycle.
    /// Only set this if you know what you are doing, or you <em>will</em> cause memory leaks and crashes.
    /// </summary>
    public bool IsAddonFactoryReplacement {
        get;
        init {
            field = value;
            
            if (value) {
                AllocateAddon();
            }
        }
    }
}
