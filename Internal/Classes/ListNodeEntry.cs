using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.BaseTypes;
using KamiToolKit.Controllers;

namespace KamiToolKit.Internal.Classes;

/// <summary>
/// Records information for use with <see cref="NativeListController{T}"/>.
/// </summary>
internal unsafe class ListNodeEntry(NodeBase node, AtkComponentListItemRenderer* renderer) {
    public readonly NodeBase Node = node;
    public readonly AtkComponentListItemRenderer* Renderer = renderer;
}
