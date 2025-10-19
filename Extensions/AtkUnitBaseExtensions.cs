using System;
using System.Linq;
using System.Numerics;
using System.Reflection;
using FFXIVClientStructs.Attributes;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.System;

namespace KamiToolKit.Extensions;

public static unsafe class AtkUnitBaseExtensions {

    /// <summary>
    ///     Gets a pointer to the specified node id
    ///     Automatically adds base id, so instead of specifying "100,000,021", you only need to provide "21"
    /// </summary>
    /// <remarks>
    ///     This will only search for nodes that are directly attached to the addon,
    ///     if your desired node is attached to another component you'll need to use AtkUldManager.GetCustomNodeById on the
    ///     parent component
    /// </remarks>
    /// <param name="addon">Addon's whose node list will be searched.</param>
    /// <param name="nodeId">Node ID to search for</param>
    /// <returns>Pointer to desired node, or null</returns>
    public static AtkResNode* GetCustomNodeById(ref this AtkUnitBase addon, uint nodeId)
        => nodeId >= NodeBase.NodeIdBase ? addon.GetNodeById(nodeId) : addon.GetNodeById(nodeId + NodeBase.NodeIdBase);

    public static string GetAddonTypeName<T>() where T : unmanaged {
        var type = typeof(T);
        var attribute = type.GetCustomAttributes().OfType<AddonAttribute>().FirstOrDefault();

        if (attribute is null) throw new Exception("无法找到 AddonAttribute，无法解析 Addon 名称。");
        var addonName = attribute.AddonIdentifiers.FirstOrDefault();

        if (addonName is null) throw new Exception("AddonAttribute 中未提供名称。");
        return addonName;
    }

    public static Vector2 Size(ref this AtkUnitBase addon)
        => new(addon.RootNode->Width, addon.RootNode->Height);

    public static Vector2 Position(ref this AtkUnitBase addon)
        => new(addon.X, addon.Y);
}
