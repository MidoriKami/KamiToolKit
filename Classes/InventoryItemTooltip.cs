using FFXIVClientStructs.FFXIV.Client.Game;

namespace KamiToolKit.Classes;

/// <summary>
/// Data object representing an item in <see cref="Inventory"/> in slot <see cref="Slot"/> for use in Item Tooltips.
/// </summary>
public record InventoryItemTooltip(InventoryType Inventory, short Slot);
