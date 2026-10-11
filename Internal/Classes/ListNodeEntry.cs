using KamiToolKit.BaseTypes;
using KamiToolKit.Controllers;

namespace KamiToolKit.Internal.Classes;

/// <summary>
/// Records information for use with <see cref="NativeListController{T}"/>.
/// </summary>
internal unsafe class ListNodeEntry(NodeBase node, Experimental.ListItemRenderer* renderer) {
    public readonly NodeBase Node = node;
    public readonly Experimental.ListItemRenderer* Renderer = renderer;
}
