using KamiToolKit.Premade.Classes;

namespace KamiToolKit.Premade.Addon;

/// <summary>
/// Represents a window with a list of items and a search bar, that allows configuring items when they are selected.
/// </summary>
public abstract class ConfigListAddon<T> : NativeAddon where T : IConfigListItem {
    
}
