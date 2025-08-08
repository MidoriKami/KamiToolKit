using FFXIVClientStructs.FFXIV.Component.GUI;
using ListItemInfo = FFXIVClientStructs.FFXIV.Component.GUI.AtkComponentListItemPopulator.ListItemInfo;

namespace KamiToolKit.Classes;

public unsafe class ListPopulatorData<T> where T : unmanaged {
    public T* Addon { get; init; }
    public ListItemInfo* ItemInfo { get; init; }
    public AtkResNode** NodeList { get; init; }
    public uint Index { get; init; }
}
