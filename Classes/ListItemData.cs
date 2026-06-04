using System;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiToolKit.Classes;

/// <summary>
/// Wrapper around a ListItemInfo/ListItemRenderer for native AtkComponentList's.
/// </summary>
/// <remarks>
/// Only one of (ItemInfo) or (ItemRenderer) will be valid at a time depending on the specific list being interacted with.
/// </remarks>
public unsafe class ListItemData {

    /// <summary>
    /// Pointer to the lists ItemInfo. If this is valid, then you can use <see cref="GetNumber"/> or <see cref="GetString"/>
    /// To get various data values for this ListItem.
    /// </summary>
    /// <remarks>
    /// Only one of (ItemInfo) or (ItemRenderer) will be valid at a time depending on the specific list being interacted with.
    /// </remarks>
    public AtkComponentListItemPopulator.ListItemInfo* ItemInfo { get; init; }

    /// <summary>
    /// Pointer to the lists item node.
    /// </summary>
    /// <remarks>
    /// Only one of (ItemInfo) or (ItemRenderer) will be valid at a time depending on the specific list being interacted with.
    /// </remarks>
    public AtkComponentListItemRenderer* ItemRenderer { get; init; }

    /// <summary>
    /// The node list array for this element. There's no way to know how many entries are actually valid.
    /// </summary>
    /// <remarks>
    /// It is recommended to decompile the populator function for this list, to see which indexes of the NodeList are used for what ui parts.
    /// </remarks>
    public AtkResNode** NodeList { get; init; }

    /// <summary>
    /// This item index in the list. This will also be the index used for accessing data from wherever this list stores the data.
    /// </summary>
    public int ItemIndex { get; init; }

    /// <summary>
    /// The NodeId for this list item. This is mostly used for tracking purposes.
    /// </summary>
    public uint NodeId { get; init; }

    /// <summary>
    /// Gets a number stored in the ItemInfo.
    /// </summary>
    /// <param name="index">Index to extract number from.</param>
    /// <exception cref="Exception">If this is a wrapper around ItemRenderer, then ItemInfo will be null.</exception>
    public uint GetNumber(int index) {
        if (ItemInfo is null) throw new Exception("ItemInfo is null, unable to extract number.");
        return ItemInfo->ListItem->UIntValues[index];
    }

    /// <summary>
    /// Gets a string stored in the ItemInfo.
    /// </summary>
    /// <param name="index">Index to extract string from.</param>
    /// <exception cref="Exception">If this is a wrapper around ItemRenderer, then ItemInfo will be null.</exception>
    public string GetString(int index) {
        if (ItemInfo is null) throw new Exception("ItemInfo is null, unable to extract string.");
        return ItemInfo->ListItem->StringValues[index].ToString();
    }

    /// <summary>
    /// Gets a node from the <see cref="NodeList"/> typed as the provided type.
    /// </summary>
    /// <remarks>
    /// This will not perform any type checking to ensure that the returned node is valid.
    /// </remarks>
    public T* GetNode<T>(int index) where T : unmanaged {
        if (NodeList is null) return null;

        return (T*)NodeList[index];
    }
}
