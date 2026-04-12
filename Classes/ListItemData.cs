using System;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiToolKit.Classes;

/// <summary>
/// Only one or the other field will be valid, be sure to check for null.
/// </summary>
public unsafe class ListItemData {
    public AtkComponentListItemPopulator.ListItemInfo* ItemInfo { get; init; }
    public AtkComponentListItemRenderer* ItemRenderer { get; init; }
    public AtkResNode** NodeList { get; init; }
    public int ItemIndex { get; init; }

    /// <summary>
    /// Gets a number stored in the ItemInfo
    /// </summary>
    /// <param name="index">Index to extract number from.</param>
    /// <exception cref="Exception">If this is a wrapper around ItemRenderer, then ItemInfo will be null.</exception>
    public uint GetNumber(int index) {
        if (ItemInfo is null) throw new Exception("ItemInfo is null, unable to extract number.");
        return ItemInfo->ListItem->UIntValues[index];
    }

    public T* GetNode<T>(int index) where T : unmanaged {
        if (NodeList is null) return null;

        return (T*) NodeList[index];
    }
}
